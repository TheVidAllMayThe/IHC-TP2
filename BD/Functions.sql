USE Magic;

GO

CREATE FUNCTION login (@user VARCHAR(255), @pass TEXT) Returns bit
AS
BEGIN
	IF EXISTS(SELECT * FROM [User] WHERE email = @user AND password LIKE @pass) RETURN 1;
	RETURN 0;
END

GO

CREATE FUNCTION isRegistered(@email varchar(255)) Returns bit
AS
BEGIN
	IF EXISTS(SELECT * FROM [User] WHERE email = @email)
		RETURN 1;
	RETURN 0;
END

GO

CREATE FUNCTION getDeckCards(@deck INT, @isSideboard BIT, @type VARCHAR(255)) Returns Table
AS
	RETURN (SELECT card, name, deck, amount, multiverseID
			FROM DeckCard
			WHERE deck = @deck AND (isSideboard = @isSideboard OR @isSideboard = NULL) AND type = @type)
	

GO

CREATE FUNCTION manaCurve (@deckID int) Returns Table
AS
	RETURN (SELECT cmc, SUM(amount) AS n
			FROM (SELECT card, amount FROM CardInDeck WHERE deck = @deckID) AS cid
			JOIN (SELECT cmc, id FROM Card) AS c
			ON cid.card = c.id AND c.cmc IS NOT NULL
			GROUP BY cmc)
GO

CREATE FUNCTION cardTypeDistribution (@deckID int) Returns @table TABLE(rarity VARCHAR(255), perc real)
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

CREATE FUNCTION manaDistribution (@deckID int) Returns @table TABLE(color VARCHAR(20), perc real)
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

CREATE FUNCTION manasourceDistribution (@deckID int) Returns @table TABLE(color VARCHAR(20), perc real)
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

GO

CREATE FUNCTION search_cards (@name Varchar(255), @type VARCHAR(255), @green BIT, @blue BIT, @white BIT, @red BIT, @black BIT, @abilities VARCHAR(255), @edition VARCHAR(255), @MinPower INT, @MaxPower INT, @MinTough INT, @MaxTough INT, @MinCMC Int, @MaxCMC Int, @Rarity VARCHAR(255)) Returns Table
AS
	RETURN(
		SELECT distinct seven.id, seven.multiverseID, seven.name as cardName, seven.editionName, seven.rarity, seven.cmc 
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
			ON six.edition =Edition.code AND (UPPER(' '+Edition.name+' ') LIKE UPPER('% '+@edition+' %') or @edition is null)) AS seven
		LEFT JOIN Creature
		ON seven.id = Creature.card AND (Creature.power >= @MinPower  or @MinPower is null) AND (Creature.power <= @MaxPower  or @MaxPower is null) AND (Creature.toughness >= @MinTough  or @MinTough is null) AND (Creature.toughness <= @MaxTough  or @MaxTough is null) AND (Seven.cmc >= @MinCMC or @MinCMC = null) AND (Seven.cmc <= @MaxCMC or @MaxCMC = null) AND (Seven.rarity = @Rarity or @Rarity is null) AND upper(' '+seven.name+' ') Like upper('% '+@name+' %') or @name is null);

GO

CREATE FUNCTION subType(@card INT) RETURNS VARCHAR(MAX)
AS
	BEGIN
		DECLARE @subtype VARCHAR(MAX);
		SELECT @subtype =  COALESCE(@subtype + ', ', '') + subtype
		FROM SubtypeOfCard
		WHERE card = @card;
		RETURN @subtype;
	END

GO

CREATE FUNCTION manaPool(@game_turn_phase INT, @player VARCHAR(255), @color VARCHAR(20)) RETURNS INT
AS
	BEGIN
		declare @amount INT;
		SELECT @amount = count(*)
		FROM (
			SELECT card, color 
			FROM ColorIdentity
			WHERE color = @color) AS ci
		JOIN( 
			SELECT Card
			FROM CardInGame
			WHERE GameTurnPhase = @game_turn_phase AND Player = @player AND Place = 'Board') AS c
		ON ci.card = c.Card;
		RETURN @amount;
	END