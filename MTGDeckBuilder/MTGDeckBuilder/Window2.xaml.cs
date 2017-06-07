using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>

    public class Listing
    {
        private int _id;
        private string _user;
        private string _startDate;
        private String _endDate;
        private int _sell;
        private double _totalPrice;

        public string User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
            }
        }
        public string StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
            }
        }
        public string EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
            }
        }
        public int Sell
        {
            get
            {
                return _sell;
            }

            set
            {
                _sell = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public double TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
            }
        }
    }
    public class CardInListing
    {
        private int _id;
        private int _listingid;
        private string _user;
        private int _card;
        private string _cardname;
        private double _priceperunit;
        private string _condition;
        private int _units;
        private string _startdate;
        private int _amountToBuy;


        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int Listingid
        {
            get
            {
                return _listingid;
            }

            set
            {
                _listingid = value;
            }
        }

        public int Card
        {
            get
            {
                return _card;
            }

            set
            {
                _card = value;
            }
        }

        public string Cardname
        {
            get
            {
                return _cardname;
            }

            set
            {
                _cardname = value;
            }
        }

        public double Priceperunit
        {
            get
            {
                return _priceperunit;
            }

            set
            {
                _priceperunit = value;
            }
        }

        public string Condition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }
        public int Units
        {
            get
            {
                return _units;
            }

            set
            {
                _units = value;
            }
        }   
        public string User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
            }
        }
        
        public string Startdate
        {
            get
            {
                return _startdate;
            }

            set
            {
                _startdate = value;
            }
        }

        public int AmountToBuy
        {
            get
            {
                return _amountToBuy;
            }

            set
            {
                _amountToBuy = value;
            }
        }
    }

    public class Card2
    {
        private string _name;
        private int _id;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
        public new string Name
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
    }
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            updateVisual();
        }

        private void buyCard(object sender, RoutedEventArgs e)
        {

            if (listBox_s.SelectedIndex == -1) return;
            CardInListing l = listBox_s.SelectedItem as CardInListing;
            try
            {
                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_buyOrSellCard " + l.Id + ", " + AmountToBuy.Text + ", '" + App.User + "'," + 1);
            } catch (SqlException sqlE) { MessageBox.Show("Invalid amount of cards"); }
            updateVisual();
        }

        private void sellCard(object sender, RoutedEventArgs e)
        {

            if (listBox_b.SelectedIndex == -1) return;

            CardInListing l = listBox_b.SelectedItem as CardInListing;
            try {

                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_buyOrSellCard " + l.Id + ", " + AmountToSell.Text + ", '" + App.User + "'," + 0);

            } catch(SqlException sqlE) { MessageBox.Show("Invalid amount of cards");}
            updateVisual();
        }

        private void updateVisual()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            SqlDataReader dr = DatabaseControl.getDataReader("EXEC usp_UserSelect '" + App.User + "'");
            dr.Read();
            money.Content = "Money: " + dr["balance"];

            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            //String getData = "SELECT * FROM listings(1, 0)";
            String getData = "SELECT * FROM udf_allCardsInListings(1)";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            ObservableCollection<CardInListing> temp = new ObservableCollection<CardInListing>();
            while (dr.Read())
            {
                temp.Add(new CardInListing { User =  dr["User"].ToString() ,Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), Startdate = dr["StartDate"].ToString(), Card = int.Parse(dr["Card"].ToString()), Cardname = dr["CardName"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
            }
            //selling.ItemsSource = temp;
            listBox_s.ItemsSource = temp;

            getData = "SELECT * FROM udf_allCardsInListings(0)";
            //getData = "SELECT * FROM listings(0, 0)";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<CardInListing>();

            while (dr.Read())
            {
                temp.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), User = dr["User"].ToString(), Startdate = dr["StartDate"].ToString(), Card = int.Parse(dr["Card"].ToString()), Cardname = dr["cardname"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
            }

            listBox_b.ItemsSource = temp;
            ;
            dr.Close();

            getData = "EXEC usp_sellingListingsSelect";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            ObservableCollection<Listing> listings = new ObservableCollection<Listing>(); 
            while (dr.Read()) {

                SqlDataReader dr2 = new SqlCommand("SELECT dbo.udf_totalListingPrice(" + dr["ID"] + ")", thisConnection).ExecuteReader();
                dr2.Read();
                listings.Add(new Listing { Id = int.Parse(dr["ID"].ToString()), StartDate = (dr["StartDate"] == null ? "null" : dr["StartDate"].ToString()), TotalPrice = (dr2.GetValue(0) == null ? 0.0 : dr2.GetDouble(0))});
            }

            //listBox_ls.ItemsSource = listings;


            if (listBox_cards.ItemsSource == null)
            {
                getData = "EXEC usp_CardSelect null";
                dr = new SqlCommand(getData, thisConnection).ExecuteReader();
                ObservableCollection<Card2> cards = new ObservableCollection<Card2>();
                for (int i = 0; (dr.Read()); i++)
                {
                    cards.Add(new Card2 { Id = int.Parse(dr["id"].ToString()), Name = dr["name"].ToString() });
                }

                listBox_cards.ItemsSource = cards;
            }
            
            getData = "SELECT * FROM udf_userListings('" + App.User + "', " + 1 + ")";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            ObservableCollection<Listing> userSellingListings = new ObservableCollection<Listing>();

            for (int i = 0; (dr.Read()); i++)
            {
                SqlDataReader dr2 = new SqlCommand("SELECT dbo.udf_totalListingPrice(" + dr["ID"] + ")", thisConnection).ExecuteReader();
                Console.WriteLine("SELECT dbo.udf_totalListingPrice(" + dr["ID"] + ")");
                dr2.Read();
                double totalPrice = dr2.GetDouble(0); 
                    
                userSellingListings.Add(new Listing { Id = int.Parse(dr["ID"].ToString()), StartDate = (dr["StartDate"] == null ? "null" : dr["StartDate"].ToString()), TotalPrice = totalPrice});
            }

            listBox_myS.ItemsSource = userSellingListings;


            getData = "SELECT * FROM udf_userListings('" + App.User + "', " + 0 + ")";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            ObservableCollection<Listing> userBuyingListings = new ObservableCollection<Listing>();

            for (int i = 0; (dr.Read()); i++)
            {
                userBuyingListings.Add(new Listing { Id = int.Parse(dr["ID"].ToString()), StartDate = (dr["StartDate"] == null ? "null" : dr["StartDate"].ToString()), TotalPrice = 0});
            }

            listBox_myB.ItemsSource = userBuyingListings;

        }

        private void newSellListing(object sender, RoutedEventArgs e)
        {
            insertListing(true);
        }
        private void newBuyListing(object sender, RoutedEventArgs e)
        {
            insertListing(false);
        }

        public void insertListing(bool sell)
        {
            DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_ListingInsert '" + App.User + "', null, " + (sell ? 1 : 0));
            updateVisual();
        }

        private void listBox_myS_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            sellinglist.Visibility = Visibility.Visible;
            listingbuy.Visibility = Visibility.Collapsed;
            sellinglistbought.Visibility = Visibility.Visible;
            listingbuysold.Visibility = Visibility.Collapsed;
            updateCardsList();
        }

        private void addCardSell(object sender, RoutedEventArgs e)
        {
            if (listBox_cards.SelectedIndex == -1 || listBox_myS.SelectedIndex == -1) return;
            int index1 = listBox_cards.SelectedIndex, index2 = listBox_myS.SelectedIndex;
            try
            {
                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_addCardToListing " + ((Listing)listBox_myS.SelectedItem).Id + ", " + ((Card2)listBox_cards.SelectedItem).Id + ", " + price.Text + ", '" + condition.Text + "'");
            }catch(SqlException sqlE) { Console.Write("" + sqlE); }

            updateCardsList();
            updateVisual();
            listBox_cards.SelectedIndex = index1;
            listBox_myS.SelectedIndex = index2;
        }

        private void addCardBuy(object sender, RoutedEventArgs e)
        {
            if (listBox_cards.SelectedIndex == -1 || listBox_myB.SelectedIndex == -1) return;
            int index1 = listBox_cards.SelectedIndex, index2 = listBox_myB.SelectedIndex;
            try
            {
                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_addCardToListing " + ((Listing)listBox_myB.SelectedItem).Id + ", " + ((Card2)listBox_cards.SelectedItem).Id + ", " + price.Text + ", '" + condition.Text + "'");
            }
            catch (SqlException sqlE) { Console.Write("" + sqlE); }
            Console.WriteLine("EXEC usp_addCardToListing " + ((Listing)listBox_myB.SelectedItem).Id + ", " + ((Card2)listBox_cards.SelectedItem).Id + ", " + price.Text + ", '" + condition.Text + "'");
            updateCardsList();
            updateVisual();
            listBox_cards.SelectedIndex = index1;
            listBox_myB.SelectedIndex = index2;
        }


        private void updateCardsList()
        {
            //try
            //{
                if (listBox_myS.SelectedIndex != -1)
                {
                    SqlDataReader dr = DatabaseControl.getDataReader("SELECT * FROM udf_cardInListing(" + ((Listing)listBox_myS.SelectedItem).Id + ")");
                    ObservableCollection<CardInListing> cardsInListing = new ObservableCollection<CardInListing>();
                    while (dr.Read())
                    {
                        cardsInListing.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), Card = int.Parse(dr["cardID"].ToString()), Cardname = dr["name"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
                    }
                    listBox_cardsInListingSelling.ItemsSource = cardsInListing;

                    dr = DatabaseControl.getDataReader("SELECT * FROM udf_cardInListingHistory(" + ((Listing)listBox_myS.SelectedItem).Id + ")");
                    ObservableCollection<CardInListing> cardsInListingBought = new ObservableCollection<CardInListing>();
                    while (dr.Read())
                    {
                        cardsInListingBought.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), Card = int.Parse(dr["cardID"].ToString()), Cardname = dr["name"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
                    }
                    listBox_cardsInListingSold.ItemsSource = cardsInListingBought;
                }

                if(listBox_myB.SelectedIndex != -1)
                {
                    SqlDataReader dr = DatabaseControl.getDataReader("SELECT * FROM udf_cardInListing(" + ((Listing)listBox_myB.SelectedItem).Id + ")");
                    ObservableCollection<CardInListing> cardsInListing = new ObservableCollection<CardInListing>();
                    while (dr.Read())
                    {
                        cardsInListing.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), Card = int.Parse(dr["cardID"].ToString()), Cardname = dr["name"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
                    }
                    listBox_cardsInListingBuy.ItemsSource = cardsInListing;

                    dr = DatabaseControl.getDataReader("SELECT * FROM udf_cardInListingHistory(" + ((Listing)listBox_myB.SelectedItem).Id + ")");
                    ObservableCollection<CardInListing> cardsInListingBought = new ObservableCollection<CardInListing>();
                    while (dr.Read())
                    {
                        cardsInListingBought.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["Listing"].ToString()), Card = int.Parse(dr["cardID"].ToString()), Cardname = dr["name"].ToString(), Priceperunit = double.Parse(dr["Price_Per_Unit"].ToString()), Condition = dr["Condition"].ToString(), Units = int.Parse(dr["Units"].ToString()) });
                    }
                    listBox_cardsInListingBuySold.ItemsSource = cardsInListingBought;
                }
            //}
            //catch (SqlException sql) {
            //    Console.WriteLine(sql);
            //}
        }

        private void removeCardSell(object sender, RoutedEventArgs e)
        {
            if (listBox_cardsInListingSelling.SelectedIndex == -1) return;

            try
            {
                Listing listing = ((Listing)listBox_myS.SelectedItem);
                CardInListing cardListing = ((CardInListing)listBox_cardsInListingSelling.SelectedItem);
                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_rmCardToListing " + listing.Id + ", " + cardListing.Card + ", " + cardListing.Priceperunit + ", '" + cardListing.Condition + "'");
            }
            catch (SqlException sqlE) { Console.Write("" + sqlE); }

            updateCardsList();
            updateVisual();
        }

        private void removeCardBuy(object sender, RoutedEventArgs e)
        {
            if (listBox_cardsInListingBuy.SelectedIndex == -1) return;

            //try
            //{
                Listing listing = ((Listing)listBox_myB.SelectedItem);
                CardInListing cardListing = ((CardInListing)listBox_cardsInListingBuy.SelectedItem);
                DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_rmCardToListing " + listing.Id + ", " + cardListing.Card + ", " + cardListing.Priceperunit + ", '" + cardListing.Condition + "'");
            //}
            //catch (SqlException sqlE) { Console.Write("" + sqlE); }

            updateCardsList();
            updateVisual();
        }

        private void listBox_myB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sellinglist.Visibility = Visibility.Collapsed;
            listingbuy.Visibility = Visibility.Visible;
            sellinglistbought.Visibility = Visibility.Collapsed;
            listingbuysold.Visibility = Visibility.Visible;
            updateCardsList();
           
        }
        private void removeListingSell(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_deleteListing", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@listing", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@listing"].Value = ((Listing) listBox_myS.SelectedItem).Id;
                    // open connection and execute stored procedure

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }catch(SqlException sqle)
                    {
                        MessageBox.Show("Card(s) have already been bought or sold from this listing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            updateCardsList();
            updateVisual();

        }
        private void removeListingBuy(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_deleteListing", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@listing", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@listing"].Value = ((Listing)listBox_myB.SelectedItem).Id;
                    // open connection and execute stored procedure

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqle)
                    {
                        MessageBox.Show("Card(s) have already been bought or sold from this listing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            updateCardsList();
            updateVisual();

        }
    }
}
