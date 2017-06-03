USE Magic;

GO

CREATE PROC addCardToDeck(@cardId int, @deck int, @amount int, @sideboard BIT)
AS
	IF EXISTS(SELECT * FROM CardInDeck WHERE card = @cardId AND deck = @deck)
		UPDATE CardInDeck SET amount = amount + @amount WHERE deck = @deck AND card = @cardId AND isSideboard = @sideboard;
	ELSE
		INSERT INTO CardInDeck(deck,card,amount,isSideboard) VALUES (@deck, @cardId, @amount, @sideboard);

GO

CREATE PROC rate @user VARCHAR(255), @deckID INT, @rating FLOAT
AS
	IF EXISTS(SELECT * FROM RatedBy WHERE deck = @deckID AND [user] = @user)
		UPDATE RatedBy SET rating = @rating where (deck=@deckID and [user] = @user);
	ELSE
		INSERT INTO RatedBy VALUES (@deckID, @user, @rating);

GO

CREATE PROC register(@user VARCHAR(255), @password TEXT)
AS
	INSERT INTO [User] VALUES (@user, @password);

GO

CREATE PROC startGame(@player1 VARCHAR(255), @deck1 INT, @player2 VARCHAR(255), @deck2 INT)
AS
	DECLARE @game_id INT, @gameturnphase_id INT;
	INSERT INTO Game(Player1, Player2) VALUES(@player1, @player2);
	SET @game_id = ( SELECT SCOPE_IDENTITY() );
	INSERT INTO Player([User], Game, Deck) VALUES(@player1, @game_id, @deck1);
	INSERT INTO Player([User], Game, Deck) VALUES(@player2, @game_id, @deck2);

	DECLARE @temp_table TABLE([MaxQty] [int] NULL);
	INSERT INTO @temp_table
	SELECT
		hundred.number*100 +
		ten.number*10 +
		one.number
	FROM(
		SELECT 1 as number union select 2 union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 0) AS one
	CROSS JOIN (SELECT 1 as number union select 2 union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 0) AS ten
	CROSS JOIN (SELECT 1 as number union select 2 union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 0) AS hundred
	WHERE (hundred.number*100 + ten.number*10 + one.number) > 0
	ORDER BY (hundred.number*100 + ten.number*10 + one.number) ASC;

	INSERT INTO CardInGame(Card, Player, Game)
		SELECT card, @player1 AS Player, @game_id AS Game
		FROM CardInDeck AS a
		JOIN @temp_table AS b
		ON b.MaxQty <= a.amount
		WHERE deck = @deck1 AND isSideboard = 0;

	INSERT INTO CardInGame(Card, Player, Game)
		SELECT card, @player2 AS Player, @gameturnphase_id AS Game
		FROM CardInDeck AS a
		JOIN @temp_table AS b
		ON b.MaxQty <= a.amount
		WHERE deck = @deck2 AND isSideboard = 0;

GO

CREATE PROC playCard(@player VARCHAR(255), @game INT, @card INT)
AS
	UPDATE CardInGame SET Place = 'Board' WHERE Player = @player AND Game = @game AND Card = @card;

GO

CREATE PROC drawCard(@player VARCHAR(255), @game_turn_phase INT)
AS
	DECLARE @cards TABLE(
		ID INT IDENTITY(1,1) NOT NULL,
		card INT NOT NULL
	);

	DECLARE @n INT, @card INT;

	INSERT INTO @cards(card)
	SELECT Card
	FROM CardInGame
	WHERE Player = @player AND Game = @game AND Place = 'Pile';

	SELECT @n = SUM(ID)
	FROM @cards;

	SELECT @card = card FROM @cards WHERE ID = ABS(CHECKSUM(NewId())) % (@n + 1);
	
	UPDATE CardInGame SET Place = 'Hand' WHERE Player = @player AND GameTurnPhase = @game_turn_phase AND Card = @card;