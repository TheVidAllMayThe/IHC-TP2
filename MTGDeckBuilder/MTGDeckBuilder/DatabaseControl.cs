using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGDeckBuilder
{
    class DatabaseControl
    {
        public static SqlDataReader getDataReader(string querry)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();
            return new SqlCommand(querry, thisConnection).ExecuteReader();
        }

        public static void ExecuteNonQuerryCommand(string command)
        {
            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();
            new SqlCommand(command, thisConnection).ExecuteNonQuery();
        }
       
    }
}
