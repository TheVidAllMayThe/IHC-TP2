USE Magic;

GO

CREATE PROC addCardToDeck(@cardId int, @deck int, @amount int, @sideboard BIT)
AS
	IF EXISTS(SELECT * FROM CardInDeck WHERE card = @cardId AND deck = @deck AND isSideboard = @sideboard)
	BEGIN
		UPDATE CardInDeck SET amount = amount + @amount WHERE deck = @deck AND card = @cardId AND isSideboard = @sideboard;
	END;
	ELSE
	BEGIN
		INSERT INTO CardInDeck(deck,card,amount,isSideboard) VALUES (@deck, @cardId, @amount, @sideboard);
	END;

GO

CREATE PROC rate @user VARCHAR(255), @deckID INT, @rating FLOAT
as
	IF EXISTS(SELECT * FROM RatedBy WHERE deck = @deckID AND [user] = @user)
	BEGIN
		UPDATE RatedBy SET rating = @rating where (deck=@deckID and [user] = @user);
	END
	ELSE
	BEGIN
		INSERT INTO RatedBy VALUES (@deckID, @user, @rating);
	END

GO

CREATE PROC [dbo].[register](@email varchar(255), @password TEXT)
as
	INSERT INTO [User] VALUES (@email, @password, NULL, NULL);