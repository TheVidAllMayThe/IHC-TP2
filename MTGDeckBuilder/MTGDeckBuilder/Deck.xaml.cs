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

namespace MTGDeckBuilder.Properties
{
    /// <summary>
    /// Interaction logic for Deck.xaml
    /// </summary>
    public partial class Deck : Page
    {
        SqlConnection thisConnection;
        int deck_id;
        public Deck()
        {
            InitializeComponent();
            deck_id = 1;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT name FROM Deck WHERE id = " + deck_id;
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_title.Text = dr.GetString(0);
            dr.Close();

            getData = "SELECT count(*) FROM CardInDeck WHERE deck = " + deck_id +" AND isSideBoard = 0";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_cards.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM LandMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_lands.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM CreatureMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_creatures.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM SorceryMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_sorceries.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM ArtifactMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_artifacts.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM InstantMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_instants.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT count(*) FROM EnchantmentMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();
            deck_number_of_enchantments.Text = dr.GetInt32(0).ToString();
            dr.Close();

            getData = "SELECT name, amount FROM CreatureMainBoard WHERE deck = " + deck_id;
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            
            while (dr.Read()){
                TextBlock tb = new TextBlock();
                tb.Text = dr.GetString(0) + " x" + dr.GetInt32(1);
                deck_creatures.Children.Add(tb);
            }
            dr.Close();
            thisConnection.Close();
        }
    }
}
