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
using System.Data;

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
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM udf_userDecks(@user)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@user", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@user"].Value = App.User;

                    // open connection and execute stored procedure
                    conn.Open();
                    SqlDataReader queryCommandReader = cmd.ExecuteReader();

                    while (queryCommandReader.Read())
                    {
                        if (queryCommandReader["name"].ToString() == txtAnswer.Text)
                        {
                            MessageBox.Show("You already have a deck with that name!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Question);
                            conn.Close();
                            return;
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("usp_addDeck", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@user", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@deck_name", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@r", SqlDbType.Int).Direction = ParameterDirection.Output;

                    // set parameter values
                    cmd.Parameters["@deck_name"].Value = txtAnswer.Text;
                    cmd.Parameters["@user"].Value = App.User;
                    
                    cmd.ExecuteNonQuery();

                    // read output value from @NewId
                    deck_id = Convert.ToInt32(cmd.Parameters["@r"].Value);
                    conn.Close();
                    this.DialogResult = true;
                }

            }
        }
    }
}
