/*ALTER USER [test] WITH DEFAULT_SCHEMA = Magic;

REVOKE select,update,delete,insert,execute,references on schema::Magic to test;
GRANT execute on schema::Magic TO test

GRANT SELECT ON udf_userDecks TO test;
GRANT SELECT ON udf_handDeck TO test;
GRANT SELECT ON udf_handDeck TO test;
GRANT SELECT ON udf_getDeckCards TO test;
GRANT SELECT ON udf_manaCurve TO test;
GRANT SELECT ON udf_cardTypeDistribution TO test;
GRANT SELECT ON udf_manaDistribution TO test;
GRANT SELECT ON udf_manasourceDistribution TO test;
GRANT SELECT ON udf_search_decks TO test;
GRANT SELECT ON udf_search_cards TO test;
GRANT SELECT ON udf_onGoingListings TO test;
GRANT SELECT ON udf_finishedListings TO test;
GRANT SELECT ON udf_userListings TO test;
GRANT SELECT ON udf_allCardsInListings TO test;
GRANT SELECT ON udf_allCardsInHistoryListings TO test;
GRANT SELECT ON udf_cardInListing TO test;
GRANT SELECT ON udf_cardInListingHistory TO test;

*/

