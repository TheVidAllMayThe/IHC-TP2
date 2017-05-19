using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    
    public partial class Card : Page
    {
        private int deck_id;
        private int card_id;
        private SqlConnection thisConnection;

        public Card(int card_id, int deck_id = -1)
        {
            this.deck_id = deck_id;
            this.card_id = card_id;
            InitializeComponent();
            show_card();
        }

        public void add_buttonClick(object sender, RoutedEventArgs e)
        {
            addCardDialog add = new addCardDialog(((BitmapImage)image.Source), rarity.Content.ToString() == "Basic Land");
            add.ShowDialog();
            if (add.DialogResult == true)
                App.Add_Card(card_id, add.DeckName, add.DeckID, add.Amount, add.SideBoard, name.Content.ToString());
        }

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 0.8;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 1;
        }


        public void show_card()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT name, text, rarity, artist, edition, multiverseID FROM Card WHERE id = " + this.card_id;
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            
            name.Content = dr.GetString(0) == null? "---" : dr.GetString(0);
            text.Content = dr.GetString(1) == null ? "---" : dr.GetString(1);
            rarity.Content = dr.GetString(2) == null ? "---" : dr.GetString(2);
            artist.Content = dr.GetString(3) == null ? "---" : dr.GetString(3);
            String editionKey = dr.GetString(4) == null ? "---" : dr.GetString(4);
            int multiverseID = dr.GetInt32(5) == 0 ? 0 : dr.GetInt32(5);
            dr.Close();

            if (multiverseID != null)
            {
                string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + multiverseID + @"&type=card";
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                image.Source = bi;
            }


            getData = "SELECT flavor FROM Flavor WHERE card = " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) flavor.Content = dr.GetString(0);
            else flavor.Content = "---";
            while (dr.Read()) type.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT type FROM TypeOfCard WHERE card = " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) type.Content = dr.GetString(0);
            else type.Content = "---";
            while (dr.Read()) type.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT subtype FROM SubtypeOfCard WHERE card = " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if(dr.Read()) subtype.Content = dr.GetString(0);
            while (dr.Read()) subtype.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT power, toughness FROM Creature WHERE card = " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read())
            {
                power.Content = dr.GetInt32(0);
                toughness.Content = dr.GetInt32(1);
            }
            else
            {
                power.Content = "---";
                toughness.Content = "---";
            }
            dr.Close();

            getData = "SELECT legality FROM Edition WHERE code = '" + editionKey+"'";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            legality.Content = dr.GetString(0);
        }
    }
}
