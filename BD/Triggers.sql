USE Magic;

GO

CREATE TRIGGER valid_amount_cards ON CardInDeck
AFTER UPDATE, INSERT
AS
	IF EXISTS(SELECT * FROM inserted JOIN Card ON inserted.card = Card.id AND rarity != 'Basic Land' AND amount > 4)
	BEGIN
		RAISERROR('Cannot have more than 4 of the same non basic lands on a deck',0,0);
		ROLLBACK TRAN;	
	END;
	ELSE IF EXISTS(SELECT * FROM inserted WHERE amount < 1)
		DELETE FROM CardInDeck WHERE card IN(SELECT card FROM inserted WHERE amount = 0);

GO

CREATE TRIGGER update_rating_at_update ON RatedBy
AFTER UPDATE, INSERT
AS
	DECLARE @deckID INT;
	SELECT @deckID = inserted.deck FROM inserted;
	UPDATE Deck SET rating = (SELECT avg(rating) from RatedBy where deck = @deckID) where id = @deckID;