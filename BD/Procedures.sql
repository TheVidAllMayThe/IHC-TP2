USE Magic;

GO

CREATE PROC addCardToDeck(@cardId int, @deck int, @amount int, @sideboard BIT)
AS
	IF EXISTS(SELECT * FROM CardInDeck WHERE card = @cardId AND deck = @deck)
		UPDATE CardInDeck SET amount = amount + @amount WHERE deck = @deck AND card = @cardId AND isSideboard = @sideboard;
	ELSE
		INSERT INTO CardInDeck(deck,card,amount,isSideboard) VALUES (@deck, @cardId, @amount, @sideboard);

GO

CREATE PROC rate @user VARCHAR(255), @deckID INT, @rating FLOAT
AS
	IF EXISTS(SELECT * FROM RatedBy WHERE deck = @deckID AND [user] = @user)
		UPDATE RatedBy SET rating = @rating where (deck=@deckID and [user] = @user);
	ELSE
		INSERT INTO RatedBy VALUES (@deckID, @user, @rating);

GO

CREATE PROC register(@user VARCHAR(255), @password TEXT)
AS
	INSERT INTO [User] VALUES (@user, @password);

GO

CREATE PROC usp_sellingListingsSelect
AS
	SELECT * FROM Listing WHERE Sell = 1;