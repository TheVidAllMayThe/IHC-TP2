using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        private static String user = "ola123@ua.pt";

        public static string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public static void Add_Card(int cardID, String deckName, int deckID, int amount, bool sideboard, String cardName)
        {

            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "EXEC usp_addCardToDeck " + cardID + ", " + deckID + ", " + amount + ", " + (sideboard ? 1 : 0);
            try
            {
                new SqlCommand(getData, thisConnection).ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message.Split('.')[2], "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            thisConnection.Close();

            MessageBox.Show("Successfully added " + amount + " " + cardName + " to " + deckName);
        }

    }

}
