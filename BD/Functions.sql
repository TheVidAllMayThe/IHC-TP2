

USE Magic;

GO

CREATE FUNCTION udf_isRegistered(@email varchar(255)) RETURNS BIT
AS
BEGIN
	IF EXISTS(SELECT * FROM [User] WHERE email = @email)
		RETURN 1;
	RETURN 0;
END

GO

CREATE FUNCTION udf_userDecks(@user VARCHAR(255)) RETURNS TABLE
AS
	RETURN(SELECT id, name FROM Deck WHERE creator = @user);

GO

CREATE FUNCTION udf_handDeck(@deck INT) RETURNS TABLE
AS
	RETURN (SELECT amount, multiverseID FROM CardInDeck JOIN Card ON CardInDeck.card = Card.id AND deck = @deck AND isSideBoard = 0);

GO

CREATE FUNCTION udf_getDeckCards(@deck INT, @isSideboard BIT, @type VARCHAR(255)) RETURNS TABLE
AS
	RETURN (SELECT card, name, deck, amount, isnull(multiverseID,0) as multiverseID
			FROM DeckCard
			WHERE deck = @deck AND (isSideboard = @isSideboard OR @isSideboard is NULL) AND type = @type)
	

GO

CREATE FUNCTION udf_manaCurve (@deckID int) Returns Table
AS
	RETURN (SELECT cmc, SUM(amount) AS n
			FROM (SELECT card, amount FROM CardInDeck WHERE deck = @deckID) AS cid
			JOIN (SELECT cmc, id FROM Card) AS c
			ON cid.card = c.id AND c.cmc IS NOT NULL
			GROUP BY cmc)
GO

CREATE FUNCTION udf_cardTypeDistribution (@deckID int) Returns @table TABLE(rarity VARCHAR(255), perc real)
AS
	BEGIN
		DECLARE @total INT;
		SELECT @total = SUM(amount)
		FROM CardInDeck
		WHERE deck = @deckID;

		INSERT INTO @table SELECT type, 100*SUM(amount)/@total
		FROM (SELECT card, amount FROM CardInDeck WHERE deck = @deckID) AS cid
		JOIN TypeOfCard
		ON cid.card = TypeOfCard.card
		GROUP BY type;
		RETURN;
	END
GO

CREATE FUNCTION udf_manaDistribution (@deckID int) Returns @table TABLE(color VARCHAR(20), perc real)
AS
	BEGIN
		DECLARE @total INT;
		SELECT @total = SUM(amount)
		FROM CardInDeck
		WHERE deck = @deckID;

		INSERT INTO @table SELECT color, 100*SUM(amount)/@total
		FROM (SELECT card, amount FROM CardInDeck WHERE deck = @deckID) AS cid
		JOIN ColorIdentity
		ON cid.card = ColorIdentity.card
		GROUP BY color;
		RETURN;
	END

GO

CREATE FUNCTION udf_manasourceDistribution (@deckID int) Returns @table TABLE(color VARCHAR(20), perc real)
AS
	BEGIN
		DECLARE @total INT;
		SELECT @total = SUM(amount)
		FROM CardInDeck
		JOIN TypeOfCard
		ON deck = @deckID AND TypeOfCard.type = 'Land' AND CardInDeck.card = TypeOfCard.card;

		INSERT INTO @table SELECT color, 100*SUM(amount)/@total
		FROM (
			SELECT cardInDeck.card, amount
			FROM CardInDeck 
			JOIN TypeOfCard
			ON CardInDeck.card = TypeOfCard.card AND TypeOfCard.type = 'Land' AND deck = @deckID) AS cid
		JOIN ColorIdentity
		ON cid.card = ColorIdentity.card
		GROUP BY color;
		RETURN;
	END
go

CREATE FUNCTION udf_search_decks(@name VARCHAR(255), @card VARCHAR(MAX), @green BIT, @blue BIT, @white BIT, @red BIT, @black BIT, @minLands INT, @maxLands INT, @minCreatures INT, @maxCreatures INT, @minSpells INT, @maxSpells INT, @minArtifacts INT, @maxArtifacts INT, @minEnchantments INT, @maxEnchantments INT, @minInstants INT, @maxInstants INT) RETURNS @table TABLE(id INT, name VARCHAR(255), creator VARCHAR(255), rating FLOAT)
AS
	BEGIN
		INSERT INTO @table SELECT distinct id, name, creator, rating
		FROM(
			SELECT id, name, creator, rating
			FROM(
				SELECT id, name, creator, rating
				FROM(
					SELECT id, name, creator, rating
					FROM(
						SELECT id, name, creator, rating
							FROM(
								SELECT d.id, d.name, d.creator, d.rating 
								FROM(
									SELECT id, name, creator, rating
									FROM Deck WHERE (upper(name) LIKE upper('%' + @name + '%') OR @name is NULL)) AS d
								JOIN (
									SELECT name, deck, amount
									FROM CardInDeck
									JOIN Card
									ON CardInDeck.card = Card.id) as cid
								ON d.id = cid.deck AND (upper(cid.name) LIKE upper('%'+@card+'%') OR @card IS NULL)) AS one
							JOIN (
								SELECT deck, color FROM DeckColors) AS two
							ON one.id = two.deck AND (two.color = 'G' OR @green is NULL)) AS two
						JOIN (
							SELECT deck, color FROM DeckColors) AS three
						ON two.id = three.deck AND (three.color = 'U' OR @blue is NULL)) AS three
					JOIN (
						SELECT deck, color FROM DeckColors) AS four
					ON three.id = four.deck AND (four.color = 'W' or @white is NULL)) AS four
				JOIN(
					SELECT deck, color FROM DeckColors) AS five
				ON four.id = five.deck AND (five.color = 'R' or @red is NULL)) AS five
			JOIN(
				SELECT deck, color FROM DeckColors) AS six
			ON five.id = six.deck AND (six.color = 'B' or @black is NULL)
			WHERE (dbo.udf_amount_of_type_on_deck(deck, 'Land') >= @MinLands OR @MinLands is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Land') <= @MaxLands OR @MaxLands is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Enchantment') >= @minEnchantments OR @minEnchantments is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Land') <= @maxEnchantments OR @maxEnchantments is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Creature') >= @minCreatures OR @minCreatures is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Creature') >= @maxCreatures OR @maxCreatures is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Spell') >= @MinSpells OR @MinSpells is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Land') <= @maxSpells OR @maxSpells is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Artifact') >= @minArtifacts OR @minArtifacts is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Artifact') >= @maxArtifacts OR @maxArtifacts is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Instant') >= @minInstants OR @minInstants is NULL) AND (dbo.udf_amount_of_type_on_deck(deck, 'Instant') >= @maxInstants OR @maxInstants is NULL);
		RETURN;
	END

GO

CREATE FUNCTION udf_amount_of_type_on_deck(@deck INT, @type VARCHAR(MAX)) RETURNS INT
AS
	BEGIN
		DECLARE @amount INT;
		SELECT @amount = sum(amount)
		FROM (SELECT card, amount FROM CardInDeck WHERE deck = @deck) AS cid
		JOIN TypeOfCard
		ON cid.card = TypeOfCard.card AND TypeOfCard.type = @type;
		IF @amount is NULL
			RETURN 0;
		RETURN @amount;
	END

GO

CREATE FUNCTION udf_search_cards (@name VARCHAR(255), @type VARCHAR(255), @green BIT, @blue BIT, @white BIT, @red BIT, @black BIT, @ability VARCHAR(255), @edition VARCHAR(255), @MinPower INT, @MaxPower INT, @MinTough INT, @MaxTough INT, @MinCMC Int, @MaxCMC Int, @Rarity VARCHAR(255)) RETURNS @table TABLE(id INT, multiverseID INT, cardName VARCHAR(MAX), editionName VARCHAR(255), rarity VARCHAR(255), cmc INT)
AS
	BEGIN
		IF (@MinPower is not NULL OR @MaxPower is not NULL OR @MinTough is not NULL OR @MaxTough is NOT NULL)
			INSERT INTO @table SELECT distinct seven.id, seven.multiverseID, seven.name as cardName, seven.editionName, seven.rarity, seven.cmc 
				FROM(
					SELECT six.id, six.multiverseID, six.name, six.rarity, six.cmc, Edition.name as editionName
					FROM(
						SELECT five.id, five.multiverseID, five.name, five.rarity, five.cmc, five.edition
						FROM(
							SELECT four.id, four.multiverseID, four.name, four.rarity, four.cmc, four.edition
							FROM(
								SELECT three.id, three.multiverseID, three.name, three.rarity, three.cmc, three.edition
								FROM(
									SELECT two.id, two.multiverseID, two.name, two.rarity, two.cmc ,two.edition
									FROM (
							
										SELECT one.id, one.multiverseID, one.name, one.rarity, one.cmc, one.edition
										FROM (
											SELECT Card.edition, Card.id,Card.name, Card.multiverseID, Card.name as cardName, Card.name as editionName, Card.rarity, Card.cmc 
											FROM Card
											JOIN TypeOfCard
											ON Card.id = TypeOfCard.card AND (TypeOfCard.type = @type OR @type is null)) AS one
										JOIN ColorIdentity
										ON one.id = ColorIdentity.card AND ((ColorIdentity.color = 'G' and ColorIdentity.isManaColor = 1) OR @green is null)) AS two
									JOIN ColorIdentity
									ON two.id = ColorIdentity.card AND ((ColorIdentity.color = 'U' and ColorIdentity.isManaColor = 1) OR @blue is null)) AS three
								JOIN ColorIdentity
								ON three.id = ColorIdentity.card AND ((ColorIdentity.color = 'W' and ColorIdentity.isManaColor = 1) OR @white is null)) AS four
							JOIN ColorIdentity
							ON four.id = ColorIdentity.card AND ((ColorIdentity.color = 'R' and ColorIdentity.isManaColor = 1) OR @red is null)) AS five
						JOIN ColorIdentity
						ON five.id = ColorIdentity.card AND ((ColorIdentity.color = 'B' and ColorIdentity.isManaColor = 1) OR @black is null)) AS six
					JOIN Edition
					ON six.edition =Edition.code AND (UPPER(''+Edition.name+'') LIKE UPPER('%'+@edition+'%') or @edition is null)) AS seven
				JOIN Creature
				ON seven.id = Creature.card AND (seven.cmc >= @MinCMC or @MinCMC is null) AND (seven.cmc <= @MaxCMC or @MaxCMC is null) AND (Creature.power >= @MinPower  or @MinPower is null) AND (Creature.power <= @MaxPower  or @MaxPower is null) AND (Creature.toughness >= @MinTough  or @MinTough is null) AND (Creature.toughness <= @MaxTough  or @MaxTough is null)  where (upper(''+seven.name+'') Like upper('%'+@name+'%') or @name is null) AND (Seven.rarity = @Rarity or @Rarity is null) AND (@ability is NULL OR upper(@ability) IN (SELECT upper(Ability) FROM Ability WHERE card = seven.id));
		ELSE
			INSERT INTO @table SELECT distinct six.id, six.multiverseID, six.name, Edition.name as editionName, six.rarity, six.cmc
					FROM(
						SELECT five.id, five.multiverseID, five.name, five.rarity, five.cmc, five.edition
						FROM(
							SELECT four.id, four.multiverseID, four.name, four.rarity, four.cmc, four.edition
							FROM(
								SELECT three.id, three.multiverseID, three.name, three.rarity, three.cmc, three.edition
								FROM(
									SELECT two.id, two.multiverseID, two.name, two.rarity, two.cmc ,two.edition
									FROM (
							
										SELECT one.id, one.multiverseID, one.name, one.rarity, one.cmc, one.edition
										FROM (
											SELECT Card.edition, Card.id,Card.name, Card.multiverseID, Card.name as cardName, Card.name as editionName, Card.rarity, Card.cmc 
											FROM Card
											JOIN TypeOfCard
											ON Card.id = TypeOfCard.card AND (TypeOfCard.type = @type OR @type is null)) AS one
										JOIN ColorIdentity
										ON one.id = ColorIdentity.card AND ((ColorIdentity.color = 'G' and ColorIdentity.isManaColor = 1) OR @green is null)) AS two
									JOIN ColorIdentity
									ON two.id = ColorIdentity.card AND ((ColorIdentity.color = 'U' and ColorIdentity.isManaColor = 1) OR @blue is null)) AS three
								JOIN ColorIdentity
								ON three.id = ColorIdentity.card AND ((ColorIdentity.color = 'W' and ColorIdentity.isManaColor = 1) OR @white is null)) AS four
							JOIN ColorIdentity
							ON four.id = ColorIdentity.card AND ((ColorIdentity.color = 'R' and ColorIdentity.isManaColor = 1) OR @red is null)) AS five
						JOIN ColorIdentity
						ON five.id = ColorIdentity.card AND ((ColorIdentity.color = 'B' and ColorIdentity.isManaColor = 1) OR @black is null)) AS six
					JOIN Edition
					ON six.edition =Edition.code AND (six.cmc >= @MinCMC or @MinCMC is null) AND (six.cmc <= @MaxCMC or @MaxCMC is null) AND (UPPER(''+Edition.name+'') LIKE UPPER('%'+@edition+'%') or @edition is null) WHERE (upper(six.name) Like upper('%'+@name+'%') or @name is null) AND (Six.rarity = @Rarity or @Rarity is null) AND (@ability is NULL OR upper(@ability) IN (SELECT upper(Ability) FROM Ability WHERE card = six.id));
		RETURN;
	END

GO

CREATE FUNCTION udf_subType(@card INT) RETURNS VARCHAR(MAX)
AS
	BEGIN
		DECLARE @subtype VARCHAR(MAX);
		SELECT @subtype =  COALESCE(@subtype + ', ', '') + subtype
		FROM SubtypeOfCard
		WHERE card = @card;
		RETURN @subtype;
	END

GO

CREATE FUNCTION udf_onGoingListings(@sell BIT) RETURNS TABLE
AS
	RETURN(
		SELECT ID, listingid, [User], StartDate, card, cardname, priceperunit, condition, units
		FROM (
			SELECT ID AS listingid, [User], StartDate
			FROM Listing
			WHERE Sell = @sell) AS l
			JOIN (
				SELECT CardInListing.ID, Card, Condition, Units, CardName, Price_Per_unit AS priceperunit, listing
				FROM CardInListing
				JOIN (
					SELECT ID, name as CardName
					FROM Card) AS c
				ON CardInListing.Card = c.id) AS ca
			ON l.listingid = ca.Listing)

GO

CREATE FUNCTION udf_finishedListings(@sell BIT) RETURNS TABLE
AS
	RETURN(
		SELECT ID, listingid, [User], StartDate, card, cardname, priceperunit, condition, units
		FROM (
			SELECT ID AS listingid, [User], StartDate
			FROM Listing
			WHERE Sell = @sell) AS l
			JOIN (
				SELECT CardInListingHistory.ID, Card, Condition, Units, CardName, Price_Per_unit AS priceperunit, listing
				FROM CardInListingHistory
				JOIN (
					SELECT ID, name as CardName
					FROM Card) AS c
				ON CardInListingHistory.Card = c.id) AS ca
			ON l.listingid = ca.Listing)
	
GO
use Magic;
go
CREATE FUNCTION udf_totalListingPrice(@listingID INT) RETURNS FLOAT
AS
	BEGIN
		DECLARE @sum float;
		SELECT @sum = SUM(Price_Per_Unit*Units) FROM CardInListing WHERE Listing = @listingID;
		IF @sum is not null
			return @sum;
		return 0.0
	END

GO

CREATE FUNCTION udf_userListings(@user VARCHAR(255), @sell BIT) RETURNS TABLE
AS
	RETURN(SELECT * FROM Listing WHERE [User] = @user AND Sell = @sell);

GO

USE Magic;
go
CREATE FUNCTION udf_allCardsInListings(@sell BIT) RETURNS TABLE
AS
	RETURN(SELECT CardInListing.*, Listing.StartDate, Listing.[User], Card.name AS CardName FROM CardInListing JOIN Listing ON CardInListing.Listing = Listing.ID AND Listing.Sell = @sell JOIN Card ON Card.id = CardInListing.Card);

GO

CREATE FUNCTION udf_allCardsInHistoryListings(@sell BIT) RETURNS TABLE
AS
	RETURN(SELECT CardInListingHistory.*, Listing.StartDate, Listing.[User], Card.name AS CardName FROM CardInListingHistory JOIN Listing ON CardInListingHistory.Listing = Listing.ID AND Listing.Sell = @sell JOIN Card ON Card.id = CardInListingHistory.Card);
GO



CREATE FUNCTION udf_cardInListing(@listing INT) RETURNS TABLE
AS
	RETURN(SELECT CardInListing.*, Card.name, Card.id as cardID FROM CardInListing JOIN Card ON CardInListing.Card = Card.ID WHERE (Listing = @listing OR @listing = NULL));

GO




use Magic;
go

CREATE FUNCTION udf_cardInListingHistory(@listing INT) RETURNS TABLE
AS
	RETURN (SELECT CardInListingHistory.*, Card.name, Card.id as cardID FROM CardInListingHistory JOIN Card ON CardInListingHistory.Card = Card.ID WHERE (Listing = @listing or @listing = NULL));

