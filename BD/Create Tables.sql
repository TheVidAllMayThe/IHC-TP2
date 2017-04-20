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
gathererCode INT,
releaseDate DATE,
legality VARCHAR(MAX),
mkm_id INT,
PRIMARY KEY (code)
);

CREATE TABLE [Card] ( 
artist VARCHAR(MAX),
id INTEGER,
imageName VARCHAR(MAX),
manaCost TINYINT,
[name] VARCHAR(MAX) not null,
rarity VARCHAR(255) not null,
[text] TEXT,
edition VARCHAR(255),
PRIMARY KEY (id),
FOREIGN KEY (edition) REFERENCES Edition(code) ON DELETE SET NULL ON UPDATE CASCADE
);

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
name VARCHAR(MAX) not null,
id INTEGER,
creator VARCHAR(255) not null,
rating float,
PRIMARY KEY (id),
FOREIGN KEY (creator) REFERENCES [User](email)
);

CREATE TABLE CardInDeck(
deck INTEGER,
[card] INTEGER unique,
isSideboard BIT not null,
PRIMARY KEY (deck, [card]),
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
flavor VARCHAR(255),
PRIMARY KEY (card, flavor),
FOREIGN KEY (card) REFERENCES Card(id)
);


CREATE TABLE Ability(
card INTEGER,
Ability VARCHAR(255),
PRIMARY KEY (card, Ability),
FOREIGN KEY (card) REFERENCES Card(id)
);

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

go

