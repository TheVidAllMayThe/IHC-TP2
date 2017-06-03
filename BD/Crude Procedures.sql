USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_AbilitySelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_AbilitySelect] 
END 
GO
CREATE PROC [dbo].[usp_AbilitySelect] 
    @card int,
    @Ability varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [Ability], [action] 
	FROM   [dbo].[Ability] 
	WHERE  ([card] = @card OR @card IS NULL) 
	       AND ([Ability] = @Ability OR @Ability IS NULL) 

	COMMIT
GO

IF OBJECT_ID('[dbo].[usp_AbilityInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_AbilityInsert] 
END 
GO
CREATE PROC [dbo].[usp_AbilityInsert] 
    @card int,
    @Ability varchar(255),
    @action bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Ability] ([card], [Ability], [action])
	SELECT @card, @Ability, @action
	
	-- Begin Return Select <- do not remove
	SELECT [card], [Ability], [action]
	FROM   [dbo].[Ability]
	WHERE  [card] = @card
	       AND [Ability] = @Ability
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_AbilityUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_AbilityUpdate] 
END 
GO
CREATE PROC [dbo].[usp_AbilityUpdate] 
    @card int,
    @Ability varchar(255),
    @action bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Ability]
	SET    [card] = @card, [Ability] = @Ability, [action] = @action
	WHERE  [card] = @card
	       AND [Ability] = @Ability
	
	-- Begin Return Select <- do not remove
	SELECT [card], [Ability], [action]
	FROM   [dbo].[Ability]
	WHERE  [card] = @card
	       AND [Ability] = @Ability	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_AbilityDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_AbilityDelete] 
END 
GO
CREATE PROC [dbo].[usp_AbilityDelete] 
    @card int,
    @Ability varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Ability]
	WHERE  [card] = @card
	       AND [Ability] = @Ability

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_CardSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardSelect] 
END 
GO
CREATE PROC [dbo].[usp_CardSelect] 
    @id int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [id], [name], [rarity], [edition], [artist], [imageName], [gathererID], [multiverseID], [manaCost], [text], [cmc] 
	FROM   [dbo].[Card] 
	WHERE  ([id] = @id OR @id IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInsert] 
END 
GO
CREATE PROC [dbo].[usp_CardInsert] 
    @name varchar(MAX),
    @rarity varchar(255),
    @edition varchar(255) = NULL,
    @artist varchar(MAX) = NULL,
    @imageName varchar(MAX) = NULL,
    @gathererID int = NULL,
    @multiverseID int = NULL,
    @manaCost varchar(100) = NULL,
    @text text = NULL,
    @cmc int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Card] ([name], [rarity], [edition], [artist], [imageName], [gathererID], [multiverseID], [manaCost], [text], [cmc])
	SELECT @name, @rarity, @edition, @artist, @imageName, @gathererID, @multiverseID, @manaCost, @text, @cmc
	
	-- Begin Return Select <- do not remove
	SELECT [id], [name], [rarity], [edition], [artist], [imageName], [gathererID], [multiverseID], [manaCost], [text], [cmc]
	FROM   [dbo].[Card]
	WHERE  [id] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardUpdate] 
END 
GO
CREATE PROC [dbo].[usp_CardUpdate] 
    @id int,
    @name varchar(MAX),
    @rarity varchar(255),
    @edition varchar(255) = NULL,
    @artist varchar(MAX) = NULL,
    @imageName varchar(MAX) = NULL,
    @gathererID int = NULL,
    @multiverseID int = NULL,
    @manaCost varchar(100) = NULL,
    @text text = NULL,
    @cmc int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Card]
	SET    [name] = @name, [rarity] = @rarity, [edition] = @edition, [artist] = @artist, [imageName] = @imageName, [gathererID] = @gathererID, [multiverseID] = @multiverseID, [manaCost] = @manaCost, [text] = @text, [cmc] = @cmc
	WHERE  [id] = @id
	
	-- Begin Return Select <- do not remove
	SELECT [id], [name], [rarity], [edition], [artist], [imageName], [gathererID], [multiverseID], [manaCost], [text], [cmc]
	FROM   [dbo].[Card]
	WHERE  [id] = @id	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardDelete] 
END 
GO
CREATE PROC [dbo].[usp_CardDelete] 
    @id int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Card]
	WHERE  [id] = @id

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_CardInDeckSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInDeckSelect] 
END 
GO
CREATE PROC [dbo].[usp_CardInDeckSelect] 
    @card int,
    @deck int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [deck], [amount], [isSideboard] 
	FROM   [dbo].[CardInDeck] 
	WHERE  ([card] = @card OR @card IS NULL) 
	       AND ([deck] = @deck OR @deck IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInDeckInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInDeckInsert] 
END 
GO
CREATE PROC [dbo].[usp_CardInDeckInsert] 
    @card int,
    @deck int,
    @amount int,
    @isSideboard bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[CardInDeck] ([card], [deck], [amount], [isSideboard])
	SELECT @card, @deck, @amount, @isSideboard
	
	-- Begin Return Select <- do not remove
	SELECT [card], [deck], [amount], [isSideboard]
	FROM   [dbo].[CardInDeck]
	WHERE  [card] = @card
	       AND [deck] = @deck
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInDeckUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInDeckUpdate] 
END 
GO
CREATE PROC [dbo].[usp_CardInDeckUpdate] 
    @card int,
    @deck int,
    @amount int,
    @isSideboard bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[CardInDeck]
	SET    [card] = @card, [deck] = @deck, [amount] = @amount, [isSideboard] = @isSideboard
	WHERE  [card] = @card
	       AND [deck] = @deck
	
	-- Begin Return Select <- do not remove
	SELECT [card], [deck], [amount], [isSideboard]
	FROM   [dbo].[CardInDeck]
	WHERE  [card] = @card
	       AND [deck] = @deck	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInDeckDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInDeckDelete] 
END 
GO
CREATE PROC [dbo].[usp_CardInDeckDelete] 
    @card int,
    @deck int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[CardInDeck]
	WHERE  [card] = @card
	       AND [deck] = @deck

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_CardInListingSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInListingSelect] 
END 
GO
CREATE PROC [dbo].[usp_CardInListingSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [Listing], [Card], [Price_Per_Unit], [Units], [Condition] 
	FROM   [dbo].[CardInListing] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInListingInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInListingInsert] 
END 
GO
CREATE PROC [dbo].[usp_CardInListingInsert] 
    @Listing int,
    @Card int,
    @Price_Per_Unit money,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[CardInListing] ([Listing], [Card], [Price_Per_Unit], [Units], [Condition])
	SELECT @Listing, @Card, @Price_Per_Unit, @Units, @Condition
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [Listing], [Card], [Price_Per_Unit], [Units], [Condition]
	FROM   [dbo].[CardInListing]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInListingUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInListingUpdate] 
END 
GO
CREATE PROC [dbo].[usp_CardInListingUpdate] 
    @ID int,
    @Listing int,
    @Card int,
    @Price_Per_Unit money,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[CardInListing]
	SET    [Listing] = @Listing, [Card] = @Card, [Price_Per_Unit] = @Price_Per_Unit, [Units] = @Units, [Condition] = @Condition
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [Listing], [Card], [Price_Per_Unit], [Units], [Condition]
	FROM   [dbo].[CardInListing]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInListingDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInListingDelete] 
END 
GO
CREATE PROC [dbo].[usp_CardInListingDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[CardInListing]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_CardInTradeSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInTradeSelect] 
END 
GO
CREATE PROC [dbo].[usp_CardInTradeSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [Trade], [Card], [Units], [Condition] 
	FROM   [dbo].[CardInTrade] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInTradeInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInTradeInsert] 
END 
GO
CREATE PROC [dbo].[usp_CardInTradeInsert] 
    @Trade int,
    @Card int,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[CardInTrade] ([Trade], [Card], [Units], [Condition])
	SELECT @Trade, @Card, @Units, @Condition
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [Trade], [Card], [Units], [Condition]
	FROM   [dbo].[CardInTrade]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInTradeUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInTradeUpdate] 
END 
GO
CREATE PROC [dbo].[usp_CardInTradeUpdate] 
    @ID int,
    @Trade int,
    @Card int,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[CardInTrade]
	SET    [Trade] = @Trade, [Card] = @Card, [Units] = @Units, [Condition] = @Condition
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [Trade], [Card], [Units], [Condition]
	FROM   [dbo].[CardInTrade]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CardInTradeDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CardInTradeDelete] 
END 
GO
CREATE PROC [dbo].[usp_CardInTradeDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[CardInTrade]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_ColorIdentitySelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ColorIdentitySelect] 
END 
GO
CREATE PROC [dbo].[usp_ColorIdentitySelect] 
    @card int,
    @color varchar(20)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [color], [isManaColor] 
	FROM   [dbo].[ColorIdentity] 
	WHERE  ([card] = @card OR @card IS NULL) 
	       AND ([color] = @color OR @color IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ColorIdentityInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ColorIdentityInsert] 
END 
GO
CREATE PROC [dbo].[usp_ColorIdentityInsert] 
    @card int,
    @color varchar(20),
    @isManaColor bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[ColorIdentity] ([card], [color], [isManaColor])
	SELECT @card, @color, @isManaColor
	
	-- Begin Return Select <- do not remove
	SELECT [card], [color], [isManaColor]
	FROM   [dbo].[ColorIdentity]
	WHERE  [card] = @card
	       AND [color] = @color
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ColorIdentityUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ColorIdentityUpdate] 
END 
GO
CREATE PROC [dbo].[usp_ColorIdentityUpdate] 
    @card int,
    @color varchar(20),
    @isManaColor bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[ColorIdentity]
	SET    [card] = @card, [color] = @color, [isManaColor] = @isManaColor
	WHERE  [card] = @card
	       AND [color] = @color
	
	-- Begin Return Select <- do not remove
	SELECT [card], [color], [isManaColor]
	FROM   [dbo].[ColorIdentity]
	WHERE  [card] = @card
	       AND [color] = @color	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ColorIdentityDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ColorIdentityDelete] 
END 
GO
CREATE PROC [dbo].[usp_ColorIdentityDelete] 
    @card int,
    @color varchar(20)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[ColorIdentity]
	WHERE  [card] = @card
	       AND [color] = @color

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_CreatureSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CreatureSelect] 
END 
GO
CREATE PROC [dbo].[usp_CreatureSelect] 
    @card int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [power], [toughness] 
	FROM   [dbo].[Creature] 
	WHERE  ([card] = @card OR @card IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CreatureInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CreatureInsert] 
END 
GO
CREATE PROC [dbo].[usp_CreatureInsert] 
    @card int,
    @power int = NULL,
    @toughness int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Creature] ([card], [power], [toughness])
	SELECT @card, @power, @toughness
	
	-- Begin Return Select <- do not remove
	SELECT [card], [power], [toughness]
	FROM   [dbo].[Creature]
	WHERE  [card] = @card
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CreatureUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CreatureUpdate] 
END 
GO
CREATE PROC [dbo].[usp_CreatureUpdate] 
    @card int,
    @power int = NULL,
    @toughness int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Creature]
	SET    [card] = @card, [power] = @power, [toughness] = @toughness
	WHERE  [card] = @card
	
	-- Begin Return Select <- do not remove
	SELECT [card], [power], [toughness]
	FROM   [dbo].[Creature]
	WHERE  [card] = @card	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_CreatureDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_CreatureDelete] 
END 
GO
CREATE PROC [dbo].[usp_CreatureDelete] 
    @card int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Creature]
	WHERE  [card] = @card

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_DeckSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_DeckSelect] 
END 
GO
CREATE PROC [dbo].[usp_DeckSelect] 
    @id int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [id], [name], [creator], [rating] 
	FROM   [dbo].[Deck] 
	WHERE  ([id] = @id OR @id IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_DeckInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_DeckInsert] 
END 
GO
CREATE PROC [dbo].[usp_DeckInsert] 
    @name varchar(255),
    @creator varchar(255),
    @rating float = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Deck] ([name], [creator], [rating])
	SELECT @name, @creator, @rating
	
	-- Begin Return Select <- do not remove
	SELECT [id], [name], [creator], [rating]
	FROM   [dbo].[Deck]
	WHERE  [id] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_DeckUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_DeckUpdate] 
END 
GO
CREATE PROC [dbo].[usp_DeckUpdate] 
    @id int,
    @name varchar(255),
    @creator varchar(255),
    @rating float = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Deck]
	SET    [name] = @name, [creator] = @creator, [rating] = @rating
	WHERE  [id] = @id
	
	-- Begin Return Select <- do not remove
	SELECT [id], [name], [creator], [rating]
	FROM   [dbo].[Deck]
	WHERE  [id] = @id	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_DeckDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_DeckDelete] 
END 
GO
CREATE PROC [dbo].[usp_DeckDelete] 
    @id int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Deck]
	WHERE  [id] = @id

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_EditionSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_EditionSelect] 
END 
GO
CREATE PROC [dbo].[usp_EditionSelect] 
    @code varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [code], [name], [legality], [mkm_id], [gathererCode], [releaseDate] 
	FROM   [dbo].[Edition] 
	WHERE  ([code] = @code OR @code IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_EditionInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_EditionInsert] 
END 
GO
CREATE PROC [dbo].[usp_EditionInsert] 
    @code varchar(255),
    @name varchar(MAX),
    @legality varchar(MAX),
    @mkm_id int = NULL,
    @gathererCode varchar(255) = NULL,
    @releaseDate date = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Edition] ([code], [name], [legality], [mkm_id], [gathererCode], [releaseDate])
	SELECT @code, @name, @legality, @mkm_id, @gathererCode, @releaseDate
	
	-- Begin Return Select <- do not remove
	SELECT [code], [name], [legality], [mkm_id], [gathererCode], [releaseDate]
	FROM   [dbo].[Edition]
	WHERE  [code] = @code
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_EditionUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_EditionUpdate] 
END 
GO
CREATE PROC [dbo].[usp_EditionUpdate] 
    @code varchar(255),
    @name varchar(MAX),
    @legality varchar(MAX),
    @mkm_id int = NULL,
    @gathererCode varchar(255) = NULL,
    @releaseDate date = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Edition]
	SET    [code] = @code, [name] = @name, [legality] = @legality, [mkm_id] = @mkm_id, [gathererCode] = @gathererCode, [releaseDate] = @releaseDate
	WHERE  [code] = @code
	
	-- Begin Return Select <- do not remove
	SELECT [code], [name], [legality], [mkm_id], [gathererCode], [releaseDate]
	FROM   [dbo].[Edition]
	WHERE  [code] = @code	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_EditionDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_EditionDelete] 
END 
GO
CREATE PROC [dbo].[usp_EditionDelete] 
    @code varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Edition]
	WHERE  [code] = @code

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_FlavorSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_FlavorSelect] 
END 
GO
CREATE PROC [dbo].[usp_FlavorSelect] 
    @card int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [flavor] 
	FROM   [dbo].[Flavor] 
	WHERE  ([card] = @card OR @card IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_FlavorInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_FlavorInsert] 
END 
GO
CREATE PROC [dbo].[usp_FlavorInsert] 
    @card int,
    @flavor text
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Flavor] ([card], [flavor])
	SELECT @card, @flavor
	
	-- Begin Return Select <- do not remove
	SELECT [card], [flavor]
	FROM   [dbo].[Flavor]
	WHERE  [card] = @card
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_FlavorUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_FlavorUpdate] 
END 
GO
CREATE PROC [dbo].[usp_FlavorUpdate] 
    @card int,
    @flavor text
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Flavor]
	SET    [card] = @card, [flavor] = @flavor
	WHERE  [card] = @card
	
	-- Begin Return Select <- do not remove
	SELECT [card], [flavor]
	FROM   [dbo].[Flavor]
	WHERE  [card] = @card	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_FlavorDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_FlavorDelete] 
END 
GO
CREATE PROC [dbo].[usp_FlavorDelete] 
    @card int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Flavor]
	WHERE  [card] = @card

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_ListingSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingSelect] 
END 
GO
CREATE PROC [dbo].[usp_ListingSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [User], [StartDate], [EndDate], [Sell] 
	FROM   [dbo].[Listing] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingInsert] 
END 
GO
CREATE PROC [dbo].[usp_ListingInsert] 
    @User varchar(255),
    @StartDate date,
    @EndDate date = NULL,
    @Sell bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[Listing] ([User], [StartDate], [EndDate], [Sell])
	SELECT @User, @StartDate, @EndDate, @Sell
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [Sell]
	FROM   [dbo].[Listing]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingUpdate] 
END 
GO
CREATE PROC [dbo].[usp_ListingUpdate] 
    @ID int,
    @User varchar(255),
    @StartDate date,
    @EndDate date = NULL,
    @Sell bit
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[Listing]
	SET    [User] = @User, [StartDate] = @StartDate, [EndDate] = @EndDate, [Sell] = @Sell
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [Sell]
	FROM   [dbo].[Listing]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingDelete] 
END 
GO
CREATE PROC [dbo].[usp_ListingDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[Listing]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_ListingBidSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingBidSelect] 
END 
GO
CREATE PROC [dbo].[usp_ListingBidSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [User], [StartDate], [EndDate], [MinimumBid] 
	FROM   [dbo].[ListingBid] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingBidInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingBidInsert] 
END 
GO
CREATE PROC [dbo].[usp_ListingBidInsert] 
    @User varchar(255),
    @StartDate date,
    @EndDate date,
    @MinimumBid money
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[ListingBid] ([User], [StartDate], [EndDate], [MinimumBid])
	SELECT @User, @StartDate, @EndDate, @MinimumBid
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [MinimumBid]
	FROM   [dbo].[ListingBid]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingBidUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingBidUpdate] 
END 
GO
CREATE PROC [dbo].[usp_ListingBidUpdate] 
    @ID int,
    @User varchar(255),
    @StartDate date,
    @EndDate date,
    @MinimumBid money
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[ListingBid]
	SET    [User] = @User, [StartDate] = @StartDate, [EndDate] = @EndDate, [MinimumBid] = @MinimumBid
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [MinimumBid]
	FROM   [dbo].[ListingBid]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingBidDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingBidDelete] 
END 
GO
CREATE PROC [dbo].[usp_ListingBidDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[ListingBid]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_ListingTradeSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingTradeSelect] 
END 
GO
CREATE PROC [dbo].[usp_ListingTradeSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [User], [StartDate], [EndDate], [AcceptedOffer] 
	FROM   [dbo].[ListingTrade] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingTradeInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingTradeInsert] 
END 
GO
CREATE PROC [dbo].[usp_ListingTradeInsert] 
    @User varchar(255),
    @StartDate date,
    @EndDate date = NULL,
    @AcceptedOffer int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[ListingTrade] ([User], [StartDate], [EndDate], [AcceptedOffer])
	SELECT @User, @StartDate, @EndDate, @AcceptedOffer
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [AcceptedOffer]
	FROM   [dbo].[ListingTrade]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingTradeUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingTradeUpdate] 
END 
GO
CREATE PROC [dbo].[usp_ListingTradeUpdate] 
    @ID int,
    @User varchar(255),
    @StartDate date,
    @EndDate date = NULL,
    @AcceptedOffer int = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[ListingTrade]
	SET    [User] = @User, [StartDate] = @StartDate, [EndDate] = @EndDate, [AcceptedOffer] = @AcceptedOffer
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [StartDate], [EndDate], [AcceptedOffer]
	FROM   [dbo].[ListingTrade]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_ListingTradeDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_ListingTradeDelete] 
END 
GO
CREATE PROC [dbo].[usp_ListingTradeDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[ListingTrade]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_OfferBidSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferBidSelect] 
END 
GO
CREATE PROC [dbo].[usp_OfferBidSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [User], [Bid], [Offer] 
	FROM   [dbo].[OfferBid] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferBidInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferBidInsert] 
END 
GO
CREATE PROC [dbo].[usp_OfferBidInsert] 
    @User varchar(255),
    @Bid int,
    @Offer money
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[OfferBid] ([User], [Bid], [Offer])
	SELECT @User, @Bid, @Offer
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [Bid], [Offer]
	FROM   [dbo].[OfferBid]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferBidUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferBidUpdate] 
END 
GO
CREATE PROC [dbo].[usp_OfferBidUpdate] 
    @ID int,
    @User varchar(255),
    @Bid int,
    @Offer money
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[OfferBid]
	SET    [User] = @User, [Bid] = @Bid, [Offer] = @Offer
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [Bid], [Offer]
	FROM   [dbo].[OfferBid]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferBidDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferBidDelete] 
END 
GO
CREATE PROC [dbo].[usp_OfferBidDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[OfferBid]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_OfferTradeSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferTradeSelect] 
END 
GO
CREATE PROC [dbo].[usp_OfferTradeSelect] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [ID], [User], [Trade], [Card], [Units], [Condition] 
	FROM   [dbo].[OfferTrade] 
	WHERE  ([ID] = @ID OR @ID IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferTradeInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferTradeInsert] 
END 
GO
CREATE PROC [dbo].[usp_OfferTradeInsert] 
    @User varchar(255),
    @Trade int,
    @Card int,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[OfferTrade] ([User], [Trade], [Card], [Units], [Condition])
	SELECT @User, @Trade, @Card, @Units, @Condition
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [Trade], [Card], [Units], [Condition]
	FROM   [dbo].[OfferTrade]
	WHERE  [ID] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferTradeUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferTradeUpdate] 
END 
GO
CREATE PROC [dbo].[usp_OfferTradeUpdate] 
    @ID int,
    @User varchar(255),
    @Trade int,
    @Card int,
    @Units int,
    @Condition varchar(20) = NULL
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[OfferTrade]
	SET    [User] = @User, [Trade] = @Trade, [Card] = @Card, [Units] = @Units, [Condition] = @Condition
	WHERE  [ID] = @ID
	
	-- Begin Return Select <- do not remove
	SELECT [ID], [User], [Trade], [Card], [Units], [Condition]
	FROM   [dbo].[OfferTrade]
	WHERE  [ID] = @ID	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_OfferTradeDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_OfferTradeDelete] 
END 
GO
CREATE PROC [dbo].[usp_OfferTradeDelete] 
    @ID int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[OfferTrade]
	WHERE  [ID] = @ID

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_RatedBySelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_RatedBySelect] 
END 
GO
CREATE PROC [dbo].[usp_RatedBySelect] 
    @deck int,
    @user varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [deck], [user], [rating] 
	FROM   [dbo].[RatedBy] 
	WHERE  ([deck] = @deck OR @deck IS NULL) 
	       AND ([user] = @user OR @user IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_RatedByInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_RatedByInsert] 
END 
GO
CREATE PROC [dbo].[usp_RatedByInsert] 
    @deck int,
    @user varchar(255),
    @rating float
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[RatedBy] ([deck], [user], [rating])
	SELECT @deck, @user, @rating
	
	-- Begin Return Select <- do not remove
	SELECT [deck], [user], [rating]
	FROM   [dbo].[RatedBy]
	WHERE  [deck] = @deck
	       AND [user] = @user
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_RatedByUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_RatedByUpdate] 
END 
GO
CREATE PROC [dbo].[usp_RatedByUpdate] 
    @deck int,
    @user varchar(255),
    @rating float
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[RatedBy]
	SET    [deck] = @deck, [user] = @user, [rating] = @rating
	WHERE  [deck] = @deck
	       AND [user] = @user
	
	-- Begin Return Select <- do not remove
	SELECT [deck], [user], [rating]
	FROM   [dbo].[RatedBy]
	WHERE  [deck] = @deck
	       AND [user] = @user	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_RatedByDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_RatedByDelete] 
END 
GO
CREATE PROC [dbo].[usp_RatedByDelete] 
    @deck int,
    @user varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[RatedBy]
	WHERE  [deck] = @deck
	       AND [user] = @user

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_SubtypeOfCardSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_SubtypeOfCardSelect] 
END 
GO
CREATE PROC [dbo].[usp_SubtypeOfCardSelect] 
    @card int,
    @subtype varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [subtype] 
	FROM   [dbo].[SubtypeOfCard] 
	WHERE  ([card] = @card OR @card IS NULL) 
	       AND ([subtype] = @subtype OR @subtype IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_SubtypeOfCardInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_SubtypeOfCardInsert] 
END 
GO
CREATE PROC [dbo].[usp_SubtypeOfCardInsert] 
    @card int,
    @subtype varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[SubtypeOfCard] ([card], [subtype])
	SELECT @card, @subtype
	
	-- Begin Return Select <- do not remove
	SELECT [card], [subtype]
	FROM   [dbo].[SubtypeOfCard]
	WHERE  [card] = @card
	       AND [subtype] = @subtype
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_SubtypeOfCardUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_SubtypeOfCardUpdate] 
END 
GO
CREATE PROC [dbo].[usp_SubtypeOfCardUpdate] 
    @card int,
    @subtype varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[SubtypeOfCard]
	SET    [card] = @card, [subtype] = @subtype
	WHERE  [card] = @card
	       AND [subtype] = @subtype
	
	-- Begin Return Select <- do not remove
	SELECT [card], [subtype]
	FROM   [dbo].[SubtypeOfCard]
	WHERE  [card] = @card
	       AND [subtype] = @subtype	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_SubtypeOfCardDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_SubtypeOfCardDelete] 
END 
GO
CREATE PROC [dbo].[usp_SubtypeOfCardDelete] 
    @card int,
    @subtype varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[SubtypeOfCard]
	WHERE  [card] = @card
	       AND [subtype] = @subtype

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_TagOfDeckSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TagOfDeckSelect] 
END 
GO
CREATE PROC [dbo].[usp_TagOfDeckSelect] 
    @deck int,
    @tag varchar(50)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [deck], [tag] 
	FROM   [dbo].[TagOfDeck] 
	WHERE  ([deck] = @deck OR @deck IS NULL) 
	       AND ([tag] = @tag OR @tag IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TagOfDeckInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TagOfDeckInsert] 
END 
GO
CREATE PROC [dbo].[usp_TagOfDeckInsert] 
    @deck int,
    @tag varchar(50)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[TagOfDeck] ([deck], [tag])
	SELECT @deck, @tag
	
	-- Begin Return Select <- do not remove
	SELECT [deck], [tag]
	FROM   [dbo].[TagOfDeck]
	WHERE  [deck] = @deck
	       AND [tag] = @tag
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TagOfDeckUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TagOfDeckUpdate] 
END 
GO
CREATE PROC [dbo].[usp_TagOfDeckUpdate] 
    @deck int,
    @tag varchar(50)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[TagOfDeck]
	SET    [deck] = @deck, [tag] = @tag
	WHERE  [deck] = @deck
	       AND [tag] = @tag
	
	-- Begin Return Select <- do not remove
	SELECT [deck], [tag]
	FROM   [dbo].[TagOfDeck]
	WHERE  [deck] = @deck
	       AND [tag] = @tag	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TagOfDeckDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TagOfDeckDelete] 
END 
GO
CREATE PROC [dbo].[usp_TagOfDeckDelete] 
    @deck int,
    @tag varchar(50)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[TagOfDeck]
	WHERE  [deck] = @deck
	       AND [tag] = @tag

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_TypeOfCardSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TypeOfCardSelect] 
END 
GO
CREATE PROC [dbo].[usp_TypeOfCardSelect] 
    @card int,
    @type varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [card], [type] 
	FROM   [dbo].[TypeOfCard] 
	WHERE  ([card] = @card OR @card IS NULL) 
	       AND ([type] = @type OR @type IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TypeOfCardInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TypeOfCardInsert] 
END 
GO
CREATE PROC [dbo].[usp_TypeOfCardInsert] 
    @card int,
    @type varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[TypeOfCard] ([card], [type])
	SELECT @card, @type
	
	-- Begin Return Select <- do not remove
	SELECT [card], [type]
	FROM   [dbo].[TypeOfCard]
	WHERE  [card] = @card
	       AND [type] = @type
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TypeOfCardUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TypeOfCardUpdate] 
END 
GO
CREATE PROC [dbo].[usp_TypeOfCardUpdate] 
    @card int,
    @type varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[TypeOfCard]
	SET    [card] = @card, [type] = @type
	WHERE  [card] = @card
	       AND [type] = @type
	
	-- Begin Return Select <- do not remove
	SELECT [card], [type]
	FROM   [dbo].[TypeOfCard]
	WHERE  [card] = @card
	       AND [type] = @type	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_TypeOfCardDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_TypeOfCardDelete] 
END 
GO
CREATE PROC [dbo].[usp_TypeOfCardDelete] 
    @card int,
    @type varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[TypeOfCard]
	WHERE  [card] = @card
	       AND [type] = @type

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

USE [Magic];
GO

IF OBJECT_ID('[dbo].[usp_UserSelect]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_UserSelect] 
END 
GO
CREATE PROC [dbo].[usp_UserSelect] 
    @email varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  

	BEGIN TRAN

	SELECT [email], [password] 
	FROM   [dbo].[User] 
	WHERE  ([email] = @email OR @email IS NULL) 

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_UserInsert]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_UserInsert] 
END 
GO
CREATE PROC [dbo].[usp_UserInsert] 
    @email varchar(255),
    @password text
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [dbo].[User] ([email], [password])
	SELECT @email, @password
	
	-- Begin Return Select <- do not remove
	SELECT [email], [password]
	FROM   [dbo].[User]
	WHERE  [email] = @email
	-- End Return Select <- do not remove
               
	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_UserUpdate]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_UserUpdate] 
END 
GO
CREATE PROC [dbo].[usp_UserUpdate] 
    @email varchar(255),
    @password text
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [dbo].[User]
	SET    [email] = @email, [password] = @password
	WHERE  [email] = @email
	
	-- Begin Return Select <- do not remove
	SELECT [email], [password]
	FROM   [dbo].[User]
	WHERE  [email] = @email	
	-- End Return Select <- do not remove

	COMMIT
GO
IF OBJECT_ID('[dbo].[usp_UserDelete]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[usp_UserDelete] 
END 
GO
CREATE PROC [dbo].[usp_UserDelete] 
    @email varchar(255)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	DELETE
	FROM   [dbo].[User]
	WHERE  [email] = @email

	COMMIT
GO
----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------

