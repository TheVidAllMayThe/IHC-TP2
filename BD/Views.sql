USE Magic;

GO

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

CREATE VIEW CardDetailed AS
SELECT id, name, TypeOfCard.type + ' ' + SubtypeOfCard.subtype AS type, cmc, edition, rarity
FROM(	
	SELECT Card.id, cmc, Card.name AS name, rarity, Edition.name AS edition
	FROM Card
	JOIN Edition
	ON Card.edition = Edition.code) AS c1
JOIN TypeOfCard
ON TypeOfCard.card = c1.id
JOIN SubtypeOfCard
ON SubtypeOfCard.card = c1.id;