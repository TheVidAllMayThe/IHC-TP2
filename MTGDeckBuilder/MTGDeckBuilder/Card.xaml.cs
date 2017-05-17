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
using System.Data.SqlClient;
using System.Configuration;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    
    public partial class Card : Page
    {
        private static int card_id;
        private SqlConnection thisConnection;

        public static int Card_id
        {
            get
            {
                return card_id;
            }

            set
            {
                card_id = value;
            }
        }

        public void show_card()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT name, text, rarity, artist, edition, multiverseID FROM Card WHERE id = " + Card_id;
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            name.Content = dr.GetString(0);
            text.Content = dr.GetString(1);
            rarity.Content = dr.GetString(2);
            artist.Content = dr.GetString(3);
            String editionKey = dr.GetString(4);
            int multiverseID = dr.GetInt32(5);
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


            getData = "SELECT flavor FROM Flavor WHERE card = " + Card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) flavor.Content = dr.GetString(0);
            else flavor.Content = "---";
            while (dr.Read()) type.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT type FROM TypeOfCard WHERE card = " + Card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read()) type.Content = dr.GetString(0);
            else type.Content = "---";
            while (dr.Read()) type.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT subtype FROM SubtypeOfCard WHERE card = " + Card_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if(dr.Read()) subtype.Content = dr.GetString(0);
            while (dr.Read()) subtype.Content += ", " + dr.GetString(0);
            dr.Close();

            getData = "SELECT power, toughness FROM Creature WHERE card = " + Card_id;
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
        public Card()
        {
            InitializeComponent();
            show_card();
        }
    }
}
