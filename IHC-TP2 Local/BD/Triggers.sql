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

CREATE TRIGGER valid_amount_sidedeck ON CardInDeck
AFTER UPDATE, INSERT
AS
	IF EXISTS(SELECT * FROM ( SELECT sum(amount) AS s, deck FROM CardInDeck WHERE deck IN(SELECT deck FROM inserted) AND isSideboard = 1 GROUP BY deck) AS c WHERE c.s > 15)
	BEGIN
		RAISERROR('Cannot have more than 15 cards in sidedeck', 11, 1);
		ROLLBACK TRAN;
	END
GO

CREATE TRIGGER listing_has_match ON CardInListing
AFTER UPDATE, INSERT
AS
	DECLARE @cardinlisting1 INT, @cardinlisting2 INT, @amount INT, @user1 VARCHAR(255), @user2 VARCHAR(255), @sell BIT;
	DECLARE cur CURSOR LOCAL for
		SELECT i.ID, cil.ID, i.Units, i.[User], cil.[User], cil.Sell
		FROM (
			SELECT inserted.ID, inserted.Card, inserted.Price_Per_Unit, inserted.Units, inserted.Condition, Sell, [User]
			FROM inserted
			JOIN Listing
			ON inserted.Listing = Listing.id) AS i
		JOIN (
			SELECT CardInListing.ID, Card, Price_Per_Unit, Units, Condition, Sell, [User]
			FROM CardInListing
			JOIN Listing
			ON CardInListing.Listing = Listing.id) AS cil
		ON i.Card = cil.Card AND i.Sell = 0 AND cil.Sell = 1 AND cil.Price_Per_Unit <= i.Price_Per_Unit AND cil.Units >= i.Units AND cil.Condition = i.Condition
	
	BEGIN TRAN;	
	OPEN cur
	FETCH NEXT FROM cur INTO @cardinlisting1, @cardinlisting2, @amount, @user1, @user2, @sell

	while @@FETCH_STATUS = 0 BEGIN

		EXEC usp_buyOrSellCard @cardinlisting2, @amount, @user1, 1;
		EXEC usp_buyOrSellCard @cardinlisting1, @amount, @user2, 0;

		FETCH NEXT FROM cur INTO @cardinlisting1, @cardinlisting2, @amount, @user1, @user2, @sell
	END

	close cur
	deallocate cur

	DECLARE cur CURSOR LOCAL for
		SELECT i.ID, cil.ID, i.Units, i.[User], cil.[User], cil.Sell
		FROM (
			SELECT inserted.ID, inserted.Card, inserted.Price_Per_Unit, inserted.Units, inserted.Condition, Sell, [User]
			FROM inserted
			JOIN Listing
			ON inserted.Listing = Listing.id) AS i
		JOIN (
			SELECT CardInListing.ID, Card, Price_Per_Unit, Units, Condition, Sell, [User]
			FROM CardInListing
			JOIN Listing
			ON CardInListing.Listing = Listing.id) AS cil
		ON i.Card = cil.Card AND i.Sell = 1 AND cil.Sell = 0 AND cil.Price_Per_Unit >= i.Price_Per_Unit AND cil.Units >= i.Units AND cil.Condition = i.Condition
		
	OPEN cur
	FETCH NEXT FROM cur INTO @cardinlisting1, @cardinlisting2, @amount, @user1, @user2, @sell

	while @@FETCH_STATUS = 0 BEGIN

		EXEC usp_buyOrSellCard @cardinlisting2, @amount, @user1, 0;
		EXEC usp_buyOrSellCard @cardinlisting1, @amount, @user2, 1;

		FETCH NEXT FROM cur INTO @cardinlisting1, @cardinlisting2, @amount, @user1, @user2, @sell
	END

	close cur
	deallocate cur

	COMMIT TRAN;
GO

CREATE TRIGGER update_rating_at_update ON RatedBy
AFTER UPDATE, INSERT
AS
	DECLARE @deckID INT;
	SELECT @deckID = inserted.deck FROM inserted;
	UPDATE Deck SET rating = (SELECT avg(rating) from RatedBy where deck = @deckID) where id = @deckID;

go
CREATE TRIGGER unit_check ON CardInListing
AFTER UPDATE
AS
	BEGIN TRAN;
	IF EXISTS(SELECT * from Inserted where Units < 0)
	BEGIN
		RAISERROR('Can''t buy that many cards', 11, 0);
		ROLLBACK
	END
	
	IF EXISTS(SELECT * from INSERTED where Units = 0)
	BEGIN
		DECLARE @id INT ;
		SELECT @id = ID FROM INSERTED;
		DELETE FROM CardInListing WHERE ID = @id;
	END
	COMMIT

GO

CREATE TRIGGER preventEditionDeleteOrUpdate ON Edition
INSTEAD OF DELETE, UPDATE
AS
	RAISERROR('CANT UPDATE OR DELETE EXISTING EDITION',11,0);
	
GO

CREATE TRIGGER preventCardInListingHistory ON CardInListingHistory
INSTEAD OF DELETE, UPDATE
AS
	RAISERROR('CAN''T UPDATE OR DELETE EXISTING CARD',11,0);
GO

