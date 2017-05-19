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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Navigation;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for newDeckDialog.xaml
    /// </summary>
    public partial class newDeckDialog : Window
    {
        private SqlConnection thisConnection;
        public int deck_id;
       
        public newDeckDialog()
        {
            InitializeComponent();
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();
            String deck_name = txtAnswer.Text;
            string getData = "SELECT * FROM Deck WHERE creator = '" + App.User +"' AND name = '" + deck_name + "'";
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            if (dr.Read())
            {
                MessageBox.Show("You already have a deck with that name!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            dr.Close();
            getData = "INSERT INTO Deck(name,creator) VALUES ('" + deck_name + "', '" + App.User + "')";
            new SqlCommand(getData, thisConnection).ExecuteNonQuery();

            getData = "SELECT id FROM Deck WHERE creator = '" + App.User + "' AND name = '" + deck_name + "'";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            dr.Read();

            deck_id = dr.GetInt32(0);
            dr.Close();
            thisConnection.Close();
            this.DialogResult = true;
        }

    }
}
