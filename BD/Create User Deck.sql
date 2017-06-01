USE Magic;

GO

INSERT INTO [User](email, password, username)
VALUES ('ola123@ua.pt', 'test123', 'ola123');

INSERT INTO Deck(name, creator)
VALUES ('My First Deck', 'ola123@ua.pt');

INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,5,0,4);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,756,0,4);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,500,0,4);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,300,0,4);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,200,0,1);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,190,0,3);

INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,954,1,3);
INSERT INTO CardInDeck(deck, card, isSideboard, amount)
VALUES (1,190,1,4);