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
        int deck_id;

        private void Add_Card(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Card_listing card = button.DataContext as Card_listing;

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "UPDATE CardInDeck SET amount = amount + 1 WHERE deck = " + card.Deck + " AND card = " + card.Id + " AND isSideboard = " + (card.IsSideDeck ? "1" : "0");
            new SqlCommand(getData, thisConnection).ExecuteNonQuery();

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
            deck_title.Content = dr.GetString(0);
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
        public Deck()
        {
            InitializeComponent();
            deck_id = 1;
            showDeck();
        }
    }
}
