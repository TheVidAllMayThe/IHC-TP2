
Alter VIEW DeckColors
AS
	SELECT color, deck
	FROM ColorIdentity 
	INNER JOIN (SELECT Card, deck 
				FROM CardInDeck) AS cards 
	ON cards.card=ColorIdentity.card 
	WHERE isManaColor=1 group by color, deck;

GO

Alter VIEW CardDetailed 
AS
	SELECT id, name, TypeOfCard.type, Magic.udf_subType(id) AS subtype, cmc, edition, rarity, multiverseID
	FROM(	
		SELECT Card.id, cmc, Card.name AS name, rarity, Edition.name AS edition, multiverseID
		FROM Card
		JOIN Edition
		ON Card.edition = Edition.code) AS c1
	JOIN TypeOfCard
	ON TypeOfCard.card = c1.id;

GO

Alter VIEW DeckCard
AS
	SELECT deck, card, name, type, amount, multiverseID, isSideboard
	FROM (SELECT deck, card, amount, isSideboard
		FROM CardInDeck) AS cid
	JOIN (SELECT id, name, multiverseID, type
		FROM CardDetailed) AS c
	ON cid.card = c.id