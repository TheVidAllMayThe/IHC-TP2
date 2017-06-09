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
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            try {
                using (SqlConnection conn = new SqlConnection(@cs))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_addCardToDeck", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // set up the parameters
                        cmd.Parameters.Add("@cardId", SqlDbType.Int);
                        cmd.Parameters.Add("@deck", SqlDbType.VarChar, 255);
                        cmd.Parameters.Add("@amount", SqlDbType.Int);
                        cmd.Parameters.Add("@sideboard", SqlDbType.Bit);

                        // set parameter values
                        cmd.Parameters["@cardId"].Value = cardID;
                        cmd.Parameters["@deck"].Value = deckID;
                        cmd.Parameters["@amount"].Value = amount;
                        cmd.Parameters["@sideboard"].Value = sideboard ? 1 : 0;
                        // open connection and execute stored procedure

                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }catch(SqlException sqle) { MessageBox.Show("Can't have more than 4 equal cards in a deck!"); return;}
            
            MessageBox.Show("Successfully added " + amount + " " + cardName + " to " + deckName);
        }

    }

}
