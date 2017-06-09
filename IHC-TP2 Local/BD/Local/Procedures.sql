USE Magic;

GO

CREATE PROC usp_addCardToDeck(@cardId int, @deck int, @amount int, @sideboard BIT)
AS
	IF EXISTS(SELECT * FROM CardInDeck WHERE card = @cardId AND deck = @deck AND isSideboard = @sideboard)
		UPDATE CardInDeck SET amount = amount + @amount WHERE deck = @deck AND card = @cardId AND isSideboard = @sideboard;
	ELSE
		INSERT INTO CardInDeck(deck,card,amount,isSideboard) VALUES (@deck, @cardId, @amount, @sideboard);

GO

CREATE PROC usp_CardDetailedInDeck(@deck INT)
AS
	SELECT amount, id, name, type, cmc, edition, rarity, subtype FROM CardDetailed JOIN CardInDeck ON CardDetailed.id = CardInDeck.card AND CardInDeck.deck = @deck;

GO

CREATE PROC usp_deleteDeck(@deck int)
AS
	DELETE FROM CardInDeck WHERE deck = @deck;
	DELETE FROM Wins WHERE Winner = @deck OR Loser = @deck;
	DELETE FROM Deck WHERE ID = @deck;

GO

CREATE PROC usp_deleteListing(@listing INT)
AS
	IF EXISTS(SELECT * FROM CardInListingHistory WHERE Listing = @listing)
	BEGIN
		RAISERROR('Card(s) from this listing have already been bought or sold',11,0);
		ROLLBACK TRAN;
	END
	DELETE FROM CardInListing WHERE Listing = @listing;
	DELETE FROM Listing WHERE ID = @listing;

GO

CREATE PROC usp_CardSelect @id int
AS 
	
	SELECT id, name, rarity, edition, artist, imageName, gathererID, multiverseID, manaCost, text, cmc 
	FROM   Card
	WHERE  (id = @id OR @id IS NULL) 

GO

CREATE PROC usp_FlavorSelect @card int
AS 
	SELECT card, flavor 
	FROM   Flavor 
	WHERE  (card = @card OR @card IS NULL) 

GO

CREATE PROC usp_rate @user VARCHAR(255), @deckID INT, @rating FLOAT
AS
	IF EXISTS(SELECT * FROM RatedBy WHERE deck = @deckID AND [user] = @user)
		UPDATE RatedBy SET rating = @rating where (deck=@deckID and [user] = @user);
	ELSE
		INSERT INTO RatedBy VALUES (@deckID, @user, @rating);

GO

CREATE PROC usp_register(@user VARCHAR(255), @password TEXT)
AS
	INSERT INTO [User](email, password) VALUES (@user, @password);

GO

CREATE PROC usp_addDeck(@deck_name VARCHAR(255), @user VARCHAR(255), @r INT OUTPUT)
AS
	INSERT INTO Deck(name,creator) VALUES (@deck_name, @user);
	SELECT @r = id FROM Deck WHERE creator = @user AND name = @deck_name;

GO

CREATE PROC usp_login (@user VARCHAR(255), @pass VARCHAR(max), @r BIT OUTPUT) 
AS
BEGIN
	DECLARE @passw VARCHAR(max) = HASHBYTES('SHA2_512', @pass)
	IF dbo.udf_isRegistered(@user) = 0
	BEGIN
		EXEC usp_register @user, @passw
		SET @r = 1;
		RETURN;
	END
	
	DECLARE @passw2 VARCHAR(max);
	SELECT @passw2 = password FROM [User] WHERE email = @user
	IF @passw = @passw2
	BEGIN
		SET @r = 1;
		RETURN;
	END
	SET @r = 0;
	RETURN;
END

GO

CREATE PROC usp_sellingListingsSelect
AS
	SELECT * FROM Listing WHERE Sell = 1;


GO

CREATE PROC usp_buyOrSellCard (@cardinlisting INT, @amount INT, @user VARCHAR(255), @sell BIT)
AS
	BEGIN TRAN
		
		DECLARE @primaryuser VARCHAR(255), @isSell BIT, @priceperunit MONEY;
		
		SELECT @primaryuser = [User], @isSell = Sell, @priceperunit = Price_Per_Unit
		FROM (SELECT ID, [User], Sell FROM LISTING) AS l
		JOIN (SELECT listing, Price_Per_Unit FROM CardInListing WHERE ID = @cardinlisting) AS cid
		ON l.ID = cid.listing;

		IF (@isSell != @sell)
		BEGIN
			RAISERROR('You cannot buy/sell these cards', 11, 0);
			ROLLBACK TRAN;
		END
		
		INSERT INTO CardInListingHistory (Listing, Card, Price_Per_Unit, Units, Condition, EndDate, SecondaryUser) SELECT Listing, Card, Price_Per_Unit, @amount, Condition, getdate(), @user FROM CardInListing;
		
		DECLARE @max INT;
		SELECT @max = Units FROM CardInListing WHERE ID = @cardinlisting;
		IF (@amount > @max)
		BEGIN
			RAISERROR('Not a valid amount of cards', 11, 0);
			ROLLBACK TRAN;
		END
		
		UPDATE CardInListing SET Units = Units - @amount WHERE ID = @cardinlisting;

		IF (@sell = 1)
		BEGIN
			UPDATE [User] SET balance += @priceperunit * @amount WHERE email = @primaryuser;
			UPDATE [User] SET balance -= @priceperunit * @amount WHERE email = @user;
		END
		ELSE
		BEGIN
			UPDATE [User] SET balance -= @priceperunit * @amount WHERE email = @primaryuser;
			UPDATE [User] SET balance += @priceperunit * @amount WHERE email = @user;
		END
	COMMIT TRAN;

go

GO
CREATE PROC usp_addCardToListing @listing int, @card int, @price Money, @condition varchar(20)
AS
	IF EXISTS(SELECT * FROM CardInListing where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price)
	BEGIN
		DECLARE @amount int;
		SELECT @amount = units FROM CardInListing where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price;
		UPDATE CardInListing SET Units = @amount + 1 where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price;
	END
	ELSE
		INSERT INTO CardInListing VALUES (@listing, @card, @price, 1, @condition);

go
CREATE PROC usp_rmCardToListing @listing int, @card int, @price Money, @condition varchar(20)
AS
	IF EXISTS(SELECT * FROM CardInListing where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price and Units > 1)
	BEGIN
		DECLARE @amount int;
		SELECT @amount = units FROM CardInListing where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price;
		UPDATE CardInListing SET Units = @amount - 1 where Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price;
	END
	ELSE
		DELETE FROM CardInListing WHERE Listing = @listing and Card = @card and Condition = @condition and Price_Per_Unit = @price;

go

CREATE PROC [dbo].[usp_getLosses] @deckID INT
AS
	SELECT * FROM Wins JOIN Deck ON Wins.Winner = Deck.ID WHERE Loser=@deckID;
GO

CREATE PROC [dbo].[usp_getWins] @deckID INT
AS
	SELECT * FROM Wins JOIN Deck ON Wins.Loser = Deck.ID WHERE Winner=@deckID;


GO


CREATE PROC usp_addWin @winner INT, @loser int
AS
	IF EXISTS(SELECT * FROM Wins WHERE (@winner = Winner AND @loser = Loser))
	BEGIN
		UPDATE Wins set Amount+=1 WHERE @winner = Winner AND @loser = Loser;
		Return;
	END

	INSERT INTO Wins VALUES (@winner, @loser, 1);


