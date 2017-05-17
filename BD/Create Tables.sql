CREATE DATABASE Magic;

go

use Magic;

go

CREATE TABLE [User] (
email VARCHAR(255),
[password] TEXT,
username VARCHAR(MAX),
dateOfBirth DATE,
PRIMARY KEY (email)
);

CREATE TABLE Edition (
[name] VARCHAR(MAX) not null,
code VARCHAR(255),
gathererCode VARCHAR(255),
releaseDate DATE,
legality VARCHAR(MAX),
mkm_id INT,
PRIMARY KEY (code)
);

CREATE TABLE [Card] ( 
artist VARCHAR(MAX),
id INTEGER IDENTITY(1,1),
imageName VARCHAR(MAX),
gathererID int,
multiverseID int,
manaCost varchar(100),
[name] VARCHAR(MAX) not null,
rarity VARCHAR(255) not null,
[text] TEXT,
edition VARCHAR(255),
cmc int,
PRIMARY KEY (id),
FOREIGN KEY (edition) REFERENCES Edition(code) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE TABLE Creature(
[card] int,
power int,
toughness int,
primary key ([card]),
foreign key ([card]) references [Card](ID));

CREATE TABLE ColorIdentity(
[card] INT,
color VARCHAR(20),
isManaColor bit not null,
PRIMARY KEY ([card], color),
FOREIGN KEY ([card]) REFERENCES [Card] (id)
);



CREATE TABLE SubtypeOfCard(
card INTEGER,
subtype VARCHAR(255),
PRIMARY KEY (card, subtype),
FOREIGN KEY (card) REFERENCES Card(id)
);

CREATE TABLE TypeOfCard(
card INTEGER,
type VARCHAR(255),
PRIMARY KEY (card, type),
FOREIGN KEY (card) REFERENCES Card(id)
);

CREATE TABLE Deck(
[name] VARCHAR(255) not null,
id INTEGER IDENTITY(1,1),
creator VARCHAR(255) not null,
rating float,
UNIQUE(creator, [name]),
PRIMARY KEY (id),
FOREIGN KEY (creator) REFERENCES [User](email)
);

CREATE TABLE CardInDeck(
deck INTEGER,
[card] INTEGER,
amount INTEGER not null,
isSideboard BIT not null,
PRIMARY KEY (deck, [card], isSideboard),
FOREIGN KEY (deck) REFERENCES Deck (id),
FOREIGN KEY ([card]) REFERENCES [Card] (id)
);

CREATE TABLE TagOfDeck(
deck INTEGER,
tag VARCHAR(50),
FOREIGN KEY (deck) REFERENCES Deck(id),
PRIMARY KEY (deck, tag)
);

CREATE TABLE RatedBy(
deck INT,
[user] VARCHAR(255),
rating FLOAT not null,
FOREIGN KEY (deck) REFERENCES Deck(id),
FOREIGN KEY ([user]) REFERENCES [User](email),
PRIMARY KEY (deck, [user])
);

CREATE TABLE Flavor(
card INTEGER,
flavor TEXT,
PRIMARY KEY (card),
FOREIGN KEY (card) REFERENCES Card(id)
);


CREATE TABLE Ability(
card INTEGER,
Ability VARCHAR(255),
action bit,
PRIMARY KEY (card, Ability),
FOREIGN KEY (card) REFERENCES Card(id)
);

/*
CREATE TABLE Player(
lp TINYINT not null,
pc TINYINT not null,
b TINYINT not null,
g TINYINT not null,
w TINYINT not null,
u TINYINT not null,
r TINYINT not null,
uncolored TINYINT not null,
[user] VARCHAR(255) not null,
deck INT not null,
PRIMARY KEY ([user]),
FOREIGN KEY ([user]) REFERENCES [User](email),
FOREIGN KEY (deck) REFERENCES Deck(id)
);

CREATE TABLE Pile(
card INTEGER,
Player VARCHAR(255),
PRIMARY KEY (Player, card),
FOREIGN KEY (card) REFERENCES CardInDeck([card]),
FOREIGN KEY (Player) REFERENCES Player([user])
);

CREATE TABLE Board(
card INTEGER,
Player VARCHAR(255),
PRIMARY KEY (Player, card),
FOREIGN KEY (card) REFERENCES CardInDeck([card]),
FOREIGN KEY (Player) REFERENCES Player([user])
);

CREATE TABLE Graveyard(
card INTEGER,
Player VARCHAR(255),
PRIMARY KEY (Player, card),
FOREIGN KEY (card) REFERENCES CardInDeck([card]),
FOREIGN KEY (Player) REFERENCES Player([user])
);

CREATE TABLE Revealed(
card INTEGER,
Player VARCHAR(255),
PRIMARY KEY (Player, card),
FOREIGN KEY (card) REFERENCES CardInDeck([card]),
FOREIGN KEY (Player) REFERENCES Player([user])
);

CREATE TABLE Exiled(
card INTEGER,
Player VARCHAR(255),
PRIMARY KEY (Player, card),
FOREIGN KEY (card) REFERENCES CardInDeck([card]),
FOREIGN KEY (Player) REFERENCES Player([user])
);



CREATE TABLE Stack(
ability VARCHAR(255),
phase VARCHAR(255),
playerPriority VARCHAR(255),
[card] int,
PRIMARY KEY (ability, phase),
FOREIGN KEY ([card], ability) REFERENCES Ability([card], ability),
FOREIGN KEY (playerPriority) REFERENCES Player([user])
);
*/
go

CREATE VIEW DeckBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card) AS c
ON cid.card = c.id

go

CREATE VIEW SideDeckBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card) AS c
ON cid.card = c.id


go

CREATE VIEW LandMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Land') AS c
ON cid.card = c.id

go

CREATE VIEW LandSideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Land') AS c
ON cid.card = c.id

go

CREATE VIEW CreatureMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Creature') AS c
ON cid.card = c.id

go

CREATE VIEW CreatureSideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Creature') AS c
ON cid.card = c.id

go

CREATE VIEW SorceryMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Sorcery') AS c
ON cid.card = c.id

go

CREATE VIEW SorcerySideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Sorcery') AS c
ON cid.card = c.id

go


CREATE VIEW ArtifactMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Artifact') AS c
ON cid.card = c.id

go

CREATE VIEW ArtifactSideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Artifact') AS c
ON cid.card = c.id

go

CREATE VIEW InstantMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Instant') AS c
ON cid.card = c.id

go

CREATE VIEW InstantSideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Instant') AS c
ON cid.card = c.id

GO

CREATE VIEW EnchantmentMainBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 0) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Enchantment') AS c
ON cid.card = c.id

go

CREATE VIEW EnchantmentSideBoard AS
SELECT deck, card, name, amount, multiverseID
FROM (SELECT deck, card, amount
	FROM CardInDeck
	WHERE isSideboard = 1) AS cid
JOIN (SELECT id, name, multiverseID
	FROM Card
	JOIN TypeOfCard
	ON Card.id = TypeOfCard.card AND TypeOfCard.type = 'Enchantment') AS c
ON cid.card = c.id

go

CREATE FUNCTION search_cards (@name Varchar(255), @type VARCHAR(255), @green BIT, @blue BIT, @white BIT, @red BIT, @black BIT, @abilities VARCHAR(255), @edition VARCHAR(255), @MinPower INT, @MaxPower INT, @MinTough INT, @MaxTough INT, @MinCMC Int, @MaxCMC Int, @Rarity VARCHAR(255)) Returns Table
AS
	RETURN(SELECT distinct seven.id, seven.multiverseID, seven.name as cardName, seven.editionName, seven.rarity, seven.cmc 
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
		ON six.edition =Edition.code AND (Edition.name = @edition OR @edition is null)) AS seven
	LEFT JOIN Creature
	ON seven.id = Creature.card AND (Creature.power >= @MinPower  or @MinPower is null) AND (Creature.power <= @MaxPower  or @MaxPower is null) AND (Creature.toughness >= @MinTough  or @MinTough is null) AND (Creature.toughness <= @MaxTough  or @MaxTough is null) AND (Seven.cmc >= @MinCMC or @MinCMC = null) AND (Seven.cmc <= @MaxCMC or @MaxCMC = null) AND (Seven.rarity = @Rarity or @Rarity is null) where upper(' '+seven.name+' ') Like upper('% '+@name+' %') or @name is null);

go

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

go

CREATE TRIGGER valid_amount_cards ON CardInDeck
AFTER UPDATE, INSERT
AS
	IF EXISTS(SELECT * FROM inserted JOIN Card ON inserted.card = Card.id AND rarity != 'Basic Land' AND amount > 4)
	BEGIN
		RAISERROR('Cannot have more than 4 of the same non basic lands on a deck',0,0);
		ROLLBACK TRAN;	
	END;