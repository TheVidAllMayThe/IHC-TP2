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
using System.Data;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Deck.xaml
    /// </summary>
    /// 

    public class DeckListing
    {
        private int _deckID;
        private string _deckName;
        private int _winsOrLosses;

        public int DeckID
        {
            get
            {
                return _deckID;
            }

            set
            {
                _deckID = value;
            }
        }

        public string DeckName
        {
            get
            {
                return _deckName;
            }

            set
            {
                _deckName = value;
            }
        }

        public int WinsOrLosses
        {
            get
            {
                return _winsOrLosses;
            }

            set
            {
                _winsOrLosses = value;
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
        private string creator;
        static Random rnd;
        Image[] stars;
        BitmapImage fullStar;
        BitmapImage emptyStar;
        
        public Deck(int deck_id)
        {
            InitializeComponent();
            this.deck_id = deck_id;
            
            emptyStar = new BitmapImage(new Uri("/images/empty_star.png", UriKind.Relative));
            fullStar = new BitmapImage(new Uri("/images/full_star.png", UriKind.Relative));
            starting_hand_cards = 7;
            rnd = new Random();
            showDeck();

            setWinsOrLossesLists();
            stars = new Image[5];
            stars[0] = star0;
            stars[1] = star1;
            stars[2] = star2;
            stars[3] = star3;
            stars[4] = star4;

            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_DeckSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@id", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@id"].Value = this.deck_id;
                    // open connection and execute stored procedure
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    creator = dr["creator"].ToString();
                    if (!App.User.Equals(creator))
                    {
                        addButton.Visibility = Visibility.Hidden;
                    }
                    dr.Close();
                }
            }
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

            using (SqlConnection conn = new SqlConnection(@cs))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM udf_handDeck(@deck)", conn))
            {
                cmd.CommandType = CommandType.Text;

                // set up the parameters
                cmd.Parameters.Add("@deck", SqlDbType.Int);

                // set parameter values
                cmd.Parameters["@deck"].Value = deck_id;
                // open connection and execute stored procedure

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                List<int> list = new List<int>();
                while (dr.Read())
                {
                    for (int i = 0; i < dr.GetInt32(0); i++) list.Add(dr.GetInt32(1));
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

                // read output value from @NewId
                conn.Close();
            }
        }
        private void addButton_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Viewbox button = sender as Viewbox;
            Card_listing card = button.DataContext as Card_listing;
            
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            using (SqlCommand cmd = new SqlCommand("usp_addCardToDeck", conn))
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                // set up the parameters
                cmd.Parameters.Add("@deck", SqlDbType.Int);
                cmd.Parameters.Add("@cardId", SqlDbType.Int);
                cmd.Parameters.Add("@amount", SqlDbType.Int);
                cmd.Parameters.Add("@sideboard", SqlDbType.Bit);

                // set parameter values
                cmd.Parameters["@deck"].Value = card.Deck;
                cmd.Parameters["@cardId"].Value = card.Id;
                cmd.Parameters["@amount"].Value = 1;
                cmd.Parameters["@sideboard"].Value = card.IsSideDeck;
                
                //try
                //{
                    cmd.ExecuteNonQuery();
                //}catch (SqlException sqle)
                //{
                  //  Console.WriteLine(sqle.ToString());
                   // MessageBox.Show(sqle.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                   // return;
                //}
            }
            showDeck();
        }

        private void removeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Viewbox button = sender as Viewbox;
            Card_listing card = button.DataContext as Card_listing;

            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            using (SqlCommand cmd = new SqlCommand("usp_addCardToDeck", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                // set up the parameters
                cmd.Parameters.Add("@deck", SqlDbType.Int);
                cmd.Parameters.Add("@cardId", SqlDbType.Int);
                cmd.Parameters.Add("@amount", SqlDbType.Int);
                cmd.Parameters.Add("@sideboard", SqlDbType.Bit);

                // set parameter values
                cmd.Parameters["@deck"].Value = card.Deck;
                cmd.Parameters["@cardId"].Value = card.Id;
                cmd.Parameters["@amount"].Value = -1;
                cmd.Parameters["@sideboard"].Value = card.IsSideDeck;
                
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException sqle)
                {
                    MessageBox.Show(sqle.Message.Split('.')[2], "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                conn.Close();
            }
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
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_DeckSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@id", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@id"].Value = deck_id;
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    creator = dr["creator"].ToString();
                    deck_title.Content = dr["name"];
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("usp_DeckSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@id", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@id"].Value = deck_id;
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    creator = dr["creator"].ToString();
                    deck_title.Content = dr["name"];
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = DBNull.Value;
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_cards.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Land";
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_lands.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Creature";
                    
          
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_creatures.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Sorcery";
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_sorceries.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Artifact";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_artifacts.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Instant";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_instants.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Enchantment";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    deck_number_of_enchantments.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_numberOfCardsInDeck(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 1;
                    cmd.Parameters["@type"].Value = DBNull.Value;

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    side_deck_number_of_cards.Content = dr.GetInt32(0).ToString();
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Land";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_lands.ItemsSource = temp;
                    else deck_lands_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Creature";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_creatures.ItemsSource = temp;
                    else deck_creatures_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Sorcery";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_sorceries.ItemsSource = temp;
                    else deck_sorceries_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Artifact";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_artifacts.ItemsSource = temp;
                    else deck_artifacts_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Enchantment";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_enchantments.ItemsSource = temp;
                    else deck_enchantments_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 0;
                    cmd.Parameters["@type"].Value = "Instant";

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) deck_instants.ItemsSource = temp;
                    else deck_instants_not_owner.ItemsSource = temp;
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT card, name, amount, deck, multiverseID FROM udf_getDeckCards(@deck, @sideboard, @type)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);
                    cmd.Parameters.Add("@sideboard", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    cmd.Parameters["@sideboard"].Value = 1;
                    cmd.Parameters["@type"].Value = DBNull.Value;

                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    ObservableCollection<Card_listing> temp = new ObservableCollection<Card_listing>();
                    while (dr.Read())
                    {
                        temp.Add(new Card_listing { Id = dr.GetInt32(0), Name = dr.GetString(1), Amount = dr.GetInt32(2), Deck = dr.GetInt32(3), IsSideDeck = false, MultiverseId = dr.GetInt32(4) });
                    }
                    if (App.User.Equals(creator)) side_deck.ItemsSource = temp;
                    else side_deck_not_owner.ItemsSource = temp;

                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_avgDeckPrice(@deck)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;


                    avgDeckPrice.Content = float.Parse(cmd.ExecuteScalar().ToString());
                }
            }
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
                string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(@cs))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_RatedBySelect", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // set up the parameters
                        cmd.Parameters.Add("@deck", SqlDbType.Int);
                        cmd.Parameters.Add("@user", SqlDbType.VarChar, 255);

                        // set parameter values
                        cmd.Parameters["@deck"].Value = deck_id;
                        cmd.Parameters["@user"].Value = App.User;
                        try
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Read();
                            string userRating = dr["rating"].ToString();
                            if (int.Parse(userRating) != ratingToGive)
                            {
                                MessageBoxResult result = MessageBox.Show("Previously you rated this deck with " + int.Parse(userRating) + " stars. Are you sure you wanna rate this deck with " + ratingToGive + " stars?", "Rating", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_rate '" + App.User + "', " + deck_id + ", " + ratingToGive);
                                }
                            }

                            dr.Close();
                    }
                        catch (InvalidOperationException io)
                        {
                            Console.WriteLine(io);
                            DatabaseControl.ExecuteNonQuerryCommand("EXEC usp_rate '" + App.User + "', " + deck_id + ", " + ratingToGive);
                            MessageBox.Show("You gave this deck " + ratingToGive + " stars.", "Rating", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        setCurrentRating();
                    }
                }
        }

        private void setCurrentRating()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_DeckSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@id", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@id"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
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
                    dr.Close();
                }
            }
        }

        private void showDetailCharts()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT cmc, n FROM udf_manaCurve(@deckID)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deckID", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deckID"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    List<KeyValuePair<int, int>> manacurveList = new List<KeyValuePair<int, int>>();
                    while (dr.Read())
                    {
                        manacurveList.Add(new KeyValuePair<int, int>(dr.GetInt32(0), dr.GetInt32(1)));
                    }
                    dr.Close();
                    manacurveChart.DataContext = manacurveList;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT rarity, perc FROM udf_cardTypeDistribution(@deckID)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deckID", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deckID"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    List<KeyValuePair<String, float>> cardtypeDistributionList = new List<KeyValuePair<String, float>>();
                    while (dr.Read())
                    {
                        cardtypeDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
                    }
                    dr.Close();
                    cardtypedistributionChart.DataContext = cardtypeDistributionList;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT color, perc FROM udf_manaDistribution(@deckID)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deckID", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deckID"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    List<KeyValuePair<String, float>> manaDistributionList = new List<KeyValuePair<String, float>>();
                    while (dr.Read())
                    {
                        manaDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
                    }
                    dr.Close();
                    manadistributionChart.DataContext = manaDistributionList;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT color, perc FROM udf_manasourceDistribution(@deckID)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@deckID", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deckID"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    List<KeyValuePair<String, float>> manasourceDistributionList = new List<KeyValuePair<String, float>>();
                    while (dr.Read())
                    {
                        manasourceDistributionList.Add(new KeyValuePair<String, float>(dr.GetString(0), dr.GetFloat(1)));
                    }
                    dr.Close();
                    manasourcedistributionChart.DataContext = manasourceDistributionList;
                }
                using (SqlCommand cmd = new SqlCommand("usp_CardDetailedInDeck", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@deck", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@deck"].Value = deck_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    Label amount;
                    Label name;
                    Label type;
                    Label cmc;
                    Label edition;
                    Label rarity;
                    for (int i = 0; dr.Read(); i++)
                    {
                        amount = new Label();
                        name = new Label();
                        type = new Label();
                        cmc = new Label();
                        edition = new Label();
                        rarity = new Label();
                        amount.Foreground = Brushes.LightGray;
                        name.Foreground = Brushes.LightGray;
                        type.Foreground = Brushes.LightGray;
                        cmc.Foreground = Brushes.LightGray;
                        edition.Foreground = Brushes.LightGray;
                        rarity.Foreground = Brushes.LightGray;

                        amount.Content = dr.GetInt32(0);
                        name.Content = dr.GetString(2);
                        type.Content = dr.GetString(3) + " " + dr["subtype"];
                        cmc.Content = dr["cmc"] == null ? "" : dr["cmc"];
                        edition.Content = dr.GetString(5);
                        rarity.Content = dr["rarity"] == null ? "" : dr["rarity"];

                        card_detailed.RowDefinitions.Add(new RowDefinition());

                        card_detailed.Children.Add(amount);
                        Grid.SetColumn(amount, 0);
                        Grid.SetRow(amount, i + 1);

                        card_detailed.Children.Add(name);
                        Grid.SetColumn(name, 1);
                        Grid.SetRow(name, i + 1);

                        card_detailed.Children.Add(type);
                        Grid.SetColumn(type, 2);
                        Grid.SetRow(type, i + 1);

                        card_detailed.Children.Add(cmc);
                        Grid.SetColumn(cmc, 3);
                        Grid.SetRow(cmc, i + 1);

                        card_detailed.Children.Add(edition);
                        Grid.SetColumn(edition, 4);
                        Grid.SetRow(edition, i + 1);

                        card_detailed.Children.Add(rarity);
                        Grid.SetColumn(rarity, 5);
                        Grid.SetRow(rarity, i + 1);
                    }
                    dr.Close();
                }
                conn.Close();
            }
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

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 0.8;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 1;
        }

        private void setWinsOrLossesLists(){
            ObservableCollection<DeckListing> winsList = new ObservableCollection<DeckListing>();
            ObservableCollection<DeckListing> lossesList = new ObservableCollection<DeckListing>();
            ObservableCollection<DeckListing> decksL = new ObservableCollection<DeckListing>();

            SqlDataReader dr = DatabaseControl.getDataReader("EXEC usp_getWins " + this.deck_id);
            while (dr.Read())
            {
                winsList.Add(new DeckListing { DeckID = int.Parse(dr["ID"].ToString()), DeckName = dr["Name"].ToString(), WinsOrLosses = int.Parse(dr["Amount"].ToString()) });
            }
            dr = DatabaseControl.getDataReader("EXEC usp_getLosses " + this.deck_id);
            while (dr.Read())
            {
                lossesList.Add(new DeckListing { DeckID = int.Parse(dr["ID"].ToString()), DeckName = dr["Name"].ToString(), WinsOrLosses = int.Parse(dr["Amount"].ToString()) });
            }

            dr = DatabaseControl.getDataReader("EXEC usp_DeckSelect " + "null");
            while (dr.Read())
            {
                decksL.Add(new DeckListing { DeckID = int.Parse(dr["ID"].ToString()), DeckName = dr["Name"].ToString()});
            }

            wins.ItemsSource = winsList;
            losses.ItemsSource = lossesList;
            decks.ItemsSource = decksL;

        }

        private void winButton(object sender, RoutedEventArgs e)
        {
            if (decks.SelectedIndex == -1) return;
            DatabaseControl.ExecuteNonQuerryCommand("Exec usp_addWin " + this.deck_id + ", " + ((DeckListing)decks.SelectedItem).DeckID);
            Console.WriteLine("Exec usp_addWin " + this.deck_id + ", " + ((DeckListing)decks.SelectedItem).DeckID);
            setWinsOrLossesLists();

        }

        private void loseButton(object sender, RoutedEventArgs e)
        {
            if (decks.SelectedIndex == -1) return;
            DatabaseControl.ExecuteNonQuerryCommand("Exec usp_addWin " + ((DeckListing)decks.SelectedItem).DeckID + ", " + this.deck_id);
            setWinsOrLossesLists();

        }
    }
}
