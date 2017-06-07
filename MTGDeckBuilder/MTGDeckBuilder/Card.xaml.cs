using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Data;

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

            string getData = "EXEC usp_CardSelect @id";
            SqlCommand command = new SqlCommand(getData, thisConnection);
            command.Parameters.Add("@id", SqlDbType.Int);
            command.Parameters["@id"].Value = this.card_id;

            SqlDataReader dr = command.ExecuteReader();
            dr.Read();

            name.Content = dr["name"] == null ? "---" : dr["name"].ToString();
            text.Content = dr["text"] == null ? "---" : dr["text"].ToString();
            rarity.Content = dr["rarity"] == null ? "---" : dr["rarity"].ToString();
            artist.Content = dr["artist"] == null ? "---" : dr["artist"].ToString();
            string editionKey = dr["edition"] == null ? "---" : dr["edition"].ToString();
            int multiverseID = dr["multiverseID"] == null ? 0 : int.Parse(dr["multiverseID"].ToString());
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


            getData = "EXEC usp_FlavorSelect " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) flavor.Content = dr["flavor"];
            else flavor.Content = "---";
            dr.Close();

            getData = "EXEC usp_TypeOfCardSelect " + card_id + ", NULL";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) type.Content = dr["type"];
            else type.Content = "---";
            while (dr.Read()) type.Content += ", " + dr["type"];
            dr.Close();

            getData = "EXEC usp_SubtypeOfCardSelect " + card_id + ", NULL";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if(dr.Read()) subtype.Content = dr["subtype"];
            while (dr.Read()) subtype.Content += ", " + dr["subtype"];
            dr.Close();

            getData = "EXEC usp_CreatureSelect " + card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read())
            {
                power.Content = dr["power"];
                toughness.Content = dr["toughness"];
            }
            else
            {
                power.Content = "---";
                toughness.Content = "---";
            }
            dr.Close();

            getData = "EXEC usp_EditionSelect " + editionKey;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            legality.Content = (dr["legality"] == null ? "---": dr["legality"]);
        }
    }
}
