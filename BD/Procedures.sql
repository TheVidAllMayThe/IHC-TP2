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
	INSERT INTO Player([User], Game, Deck) VALUES(@player1, @game_id, @deck1)
	INSERT INTO Player([User], Game, Deck) VALUES(@player2, @game_id, @deck2)
	INSERT INTO GameTurnPhase(Game) VALUES(@game_id);
	SET @gameturnphase_id = ( SELECT SCOPE_IDENTITY() );
	INSERT INTO CardInGame(Card, Player, GameTurnPhase, Amount) SELECT card, @player1 AS Player, @gameturnphase_id AS GameTurnPhase, amount FROM CardInDeck WHERE deck = @deck1 AND isSideboard = 0;
	INSERT INTO CardInGame(Card, Player, GameTurnPhase, Amount) SELECT card, @player2 AS Player, @gameturnphase_id AS GameTurnPhase, amount FROM CardInDeck WHERE deck = @deck2 AND isSideboard = 0;

GO

CREATE PROC playCard(@player VARCHAR(255), @game_turn_phase INT, @card INT)
AS
	UPDATE CardInGame SET Amount = Amount - 1 WHERE Player = @player AND GameTurnPhase = @game_turn_phase AND Card = @card AND Place != 'Board';
	IF EXISTS(SELECT * FROM CardInGame WHERE Player = @player AND GameTurnPhase = @game_turn_phase AND Card = @card AND Place = 'Board')
		UPDATE CardInGame SET Amount = Amount + 1 WHERE Player = @player AND GameTurnPhase = @game_turn_phase AND Card = @card AND Place = 'Board';
	ELSE 
		INSERT INTO CardInGame(Card, Player, GameTurnPhase, Amount) VALUES (@card, @player, @game_turn_phase, 1);

GO

CREATE PROC drawCard(@player VARCHAR(255), @game_turn_phase INT)
AS
	ABS(CHECKSUM(NewId())) % 14
	SELECT Card FROM CardInGame WHERE Player = @player AND GameTurnPhase = @game_turn_phase AND Place = 'Pile';