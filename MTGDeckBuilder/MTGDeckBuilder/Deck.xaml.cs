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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Deck.xaml
    /// </summary>
    /// 
    public class Card_detailed
    {
        private string _name;
        private string _type;
        private int _cmc;
        private string _edition;
        private string _rarity;
        private int _amount;

        public string Name
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

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public int Cmc
        {
            get
            {
                return _cmc;
            }

            set
            {
                _cmc = value;
            }
        }

        public string Edition
        {
            get
            {
                return _edition;
            }

            set
            {
                _edition = value;
            }
        }

        public string Rarity
        {
            get
            {
                return _rarity;
            }

            set
            {
                _rarity = value;
            }
        }

        public int Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                _amount = value;
            }
        }
    }
    public class Card_listing
    {
        private int _deck;
        private int _id;
        private string _name;
        private int _amount;
        private bool _isSideDeck;
        private int _multiverseId;


        public int Deck { get { return _deck; } set { _deck = value; } }
        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public bool IsSideDeck { get { return _isSideDeck; } set { _isSideDeck = value; } }
        public int MultiverseId { get { return _multiverseId; } set { _multiverseId = value; } }
    }

    public partial class Deck : Page
    {
        SqlConnection thisConnection;
        private int deck_id;
        private int starting_hand_cards;
        static Random rnd;
        Image[] stars;
        BitmapImage fullStar;
        BitmapImage emptyStar;
        
        public Deck(int deck_id)
        {
            emptyStar = new BitmapImage(new Uri("/images/empty_star.png", UriKind.Relative));
            fullStar = new BitmapImage(new Uri("/images/full_star.png", UriKind.Relative));
            this.deck_id = deck_id;
            InitializeComponent();
            starting_hand_cards = 7;
            rnd = new Random();
            showDeck();
            stars = new Image[5];
            stars[0] = star0;
            stars[1] = star1;
            stars[2] = star2;
            stars[3] = star3;
            stars[4] = star4;

            setCurrentRating();
        }

        private void New_Hand(object sender, RoutedEventArgs e)
        {
            starting_hand_cards = 7;
            show_hand();
        }

        private void Mulligan(object sender, RoutedEventArgs e)
        {
            if(starting_hand_cards > 1) starting_hand_cards -= 1;
            show_hand();
        }

        private void search_cards(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
            {
                SearchCards c = new SearchCards(deck_id, deck_title.Content.ToString());
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(c);
            }
        }

        private void show_hand()
        {
            upper_panel.Children.Clear();
            lower_panel.Children.Clear();
            Image img;
            string fullFilePath;
            BitmapImage bi;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            String getData = "SELECT amount, multiverseID FROM CardInDeck JOIN Card ON CardInDeck.card = Card.id AND deck = " + deck_id + " AND isSideBoard = 0";
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            List<int> list = new List<int>();
            while (dr.Read())
            {
                for(int i=0; i< dr.GetInt32(0); i++) list.Add(dr.GetInt32(1));
            }
            
            int picked;
            for (int i = 0; i < 4 && i < starting_hand_cards && list.Count() > 0; i++)
            {
                img = new Image();
                img.Margin = new Thickness(10, 10, 10, 10);
                picked = rnd.Next(list.Count);
                if (picked != null)
                {
                    fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + list[picked] + @"&type=card";
                }
                else
                {
                    fullFilePath = "magic_the_gathering.png";
                }
                list.RemoveAt(picked);
                bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                img.Source = bi;
                upper_panel.Children.Add(img);
            }
            for (int i = 4; i < starting_hand_cards && list.Count() > 0; i++)
            {
                img = new Image();
                img.Margin = new Thickness(10, 10, 10, 10);
                picked = rnd.Next(list.Count);
                if (picked != null)
                {
                    fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + list[picked] + @"&type=card";
                }
                else
                {
                    fullFilePath = "magic_the_gathering.png";
                }
                list.Remove(picked);
                bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                img.Source = bi;
                lower_panel.Children.Add(img);
            }
            dr.Close();
        }
        private void Add_Card(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Card_listing card = button.DataContext as Card_listing;

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "UPDATE CardInDeck SET amount = amount + 1 WHERE deck = " + card.Deck + " AND card = " + card.Id + " AND isSideboard = " + (card.IsSideDeck ? "1" : "0");
            try
            {
                new SqlCommand(getData, thisConnection).ExecuteNonQuery();
            }
            catch (SqlException sqle)
            {
                MessageBox.Show(sqle.Message.Split('.')[2], "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            thisConnection.Close();
            showDeck();
        }

        private void Remove_Card(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Card_listing card = button.DataContext as Card_listing;

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "UPDATE CardInDeck SET amount = amount - 1 WHERE deck = " + card.Deck + " AND card = " + card.Id + " AND isSideboard = " + (card.IsSideDeck ? "1" : "0");
            new SqlCommand(getData, thisConnection).ExecuteNonQuery();

            thisConnection.Close();
            card.Amount -= 1;
            showDeck();
        }

        private void showImage(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            Card_listing card = label.DataContext as Card_listing;

            if (card.MultiverseId != null)
            {
                string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.MultiverseId + @"&type=card";
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                image.Source = bi;
            }
        }

        public void showDeck()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT name FROM Deck WHERE id = " + deck_id;
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_title.Content = dr["name"];
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM CardInDeck WHERE deck = " + deck_id + " AND isSideBoard = 0";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_cards.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM LandMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_lands.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM LandMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_lands.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM CreatureMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_creatures.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM CreatureMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_creatures.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM SorceryMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_sorceries.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM SorceryMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_sorceries.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM ArtifactMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_artifacts.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM ArtifactMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_artifacts.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM InstantMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_instants.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM InstantMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_instants.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM EnchantmentMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_enchantments.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM EnchantmentMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            deck_enchantments.ItemsSource = temp;
            dr.Close();

            getData = "SELECT isnull(SUM(amount),0) FROM CardInDeck WHERE deck = " + deck_id + " AND isSideBoard = 1";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            side_deck_number_of_cards.Content = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT card, name, amount, deck, multiverseID FROM SideDeckBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<Card_listing>();
            while (dr.Read())
            {
                temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
            }
            side_deck.ItemsSource = temp;
            dr.Close();

            thisConnection.Close();
        }

        private void star_MouseEnter(object sender, MouseEventArgs e)
        {
            int starNumber = int.Parse(((Image)sender).Name.Substring(4));
            for (int i = 0; i <= starNumber; i++)
            {
                stars[i].Source = fullStar;
            }

            for (int i = starNumber + 1; i < 5; i++)
            {
                stars[i].Source = emptyStar;
            }
        }

        private void star_MouseLeave(object sender, MouseEventArgs e)
        {
            setCurrentRating();
        }

        private void star_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int ratingToGive = int.Parse(((Image)sender).Name.Substring(4)) + 1;
           try {
                SqlDataReader dr = DatabaseControl.getDataReader("Select rating from RatedBy where deck=" + deck_id + " and [user]='" + App.User + "'");
                dr.Read();
                Console.WriteLine("Select rating from RatedBy where deck=" + deck_id + " and [user]='" + App.User + "'");
                string userRating = dr["rating"].ToString();
            if (int.Parse(userRating) == ratingToGive)
                {
                    MessageBox.Show("You already rated this deck with the same rating!", "Rating", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                MessageBoxResult result = MessageBox.Show("Previously you rated this deck with " + int.Parse(userRating) + " stars. Are you sure you wanna rate this deck with " + ratingToGive + " stars?", "Rating", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        DatabaseControl.ExecuteNonQuerryCommand("EXEC rate '" + App.User + "', " + deck_id + ", " + ratingToGive);
                    }
                }
            }catch(InvalidOperationException io) {
                Console.WriteLine(io);
                DatabaseControl.ExecuteNonQuerryCommand("EXEC rate '" + App.User + "', " + deck_id + ", " + ratingToGive);
                MessageBox.Show("You gave this deck " + ratingToGive + " stars.", "Rating", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            setCurrentRating();
        }

        private void setCurrentRating()
        {
            SqlDataReader dr = DatabaseControl.getDataReader("Select rating from Deck where id=" + deck_id);
            dr.Read();
            int currentRating = dr["rating"].ToString().Equals("") ? 0 : int.Parse(dr["rating"].ToString());
            for (int i = 0; i < currentRating; i++)
            {
                stars[i].Source = fullStar;
            }

            for (int i = (currentRating == 0 ? 0 : currentRating); i < 5; i++)
            {
                stars[i].Source = emptyStar;
            }
        }

        private void showDetailCharts()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT cmc, n FROM manaCurve(" + deck_id + ")";
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            List<KeyValuePair<int, int>> manacurveList = new List<KeyValuePair<int, int>>();
            while (dr.Read())
            {
                manacurveList.Add(new KeyValuePair<int, int>(dr.GetInt32(0), dr.GetInt32(1)));
            }
            dr.Close();

            getData = "SELECT rarity, perc FROM cardTypeDistribution(" + deck_id + ")";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            List<KeyValuePair<String, float>> cardtypeDistributionList = new List<KeyValuePair<String, float>>();
            while (dr.Read())
            {
                cardtypeDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
            }
            dr.Close();

            getData = "SELECT color, perc FROM manaDistribution(" + deck_id + ")";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            List<KeyValuePair<String, float>> manaDistributionList = new List<KeyValuePair<String, float>>();
            while (dr.Read())
            {
                manaDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
            }
            dr.Close();


            getData = "SELECT color, perc FROM manasourceDistribution(" + deck_id + ")";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            List<KeyValuePair<String, float>> manasourceDistributionList = new List<KeyValuePair<String, float>>();
            while (dr.Read())
            {
                manasourceDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
            }
            dr.Close();
            
            manacurveChart.DataContext = manacurveList;
            cardtypedistributionChart.DataContext = cardtypeDistributionList;
            manadistributionChart.DataContext = manaDistributionList;
            manasourcedistributionChart.DataContext = manasourceDistributionList;


            getData = "SELECT amount, id, name, type, cmc, edition, rarity FROM CardDetailed JOIN CardInDeck ON CardDetailed.id = CardInDeck.card AND CardInDeck.deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            ObservableCollection<Card_detailed> temp = new ObservableCollection<Card_detailed>();
            while (dr.Read())
            {
                temp.Add(new Card_detailed { Amount = dr.GetInt32(0), Name = dr.GetString(2), Type = dr.GetString(3), Cmc = dr["cmc"] == null ? dr.GetInt32(4):0, Edition = dr.GetString(5), Rarity = dr["rarity"] == null? "":dr.GetString(6) });
            }
            deck_cards_detailed.ItemsSource = temp;
            dr.Close();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (details.IsSelected)
            {
                showDetailCharts();
            }
                
            if (starting_hand.IsSelected)
            {
                show_hand();
            }
        }

        public void add_buttonClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 0.8;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 1;
        }

        private void addButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Viewbox button = sender as Viewbox;
            Card_listing card = button.DataContext as Card_listing;

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "UPDATE CardInDeck SET amount = amount + 1 WHERE deck = " + card.Deck + " AND card = " + card.Id + " AND isSideboard = " + (card.IsSideDeck ? "1" : "0");
            try
            {
                new SqlCommand(getData, thisConnection).ExecuteNonQuery();
            }
            catch (SqlException sqle)
            {
                MessageBox.Show(sqle.Message.Split('.')[2], "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            thisConnection.Close();
            showDeck();
        }

        private void removeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Viewbox button = sender as Viewbox;
            Card_listing card = button.DataContext as Card_listing;

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "UPDATE CardInDeck SET amount = amount - 1 WHERE deck = " + card.Deck + " AND card = " + card.Id + " AND isSideboard = " + (card.IsSideDeck ? "1" : "0");
            new SqlCommand(getData, thisConnection).ExecuteNonQuery();

            thisConnection.Close();
            card.Amount -= 1;
            showDeck();
        }
    }
}
