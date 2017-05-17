using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for addCardDialog.xaml
    /// </summary>
    public partial class addCardDialog : Window
    {
        
        public addCardDialog(BitmapImage image)
        {
            InitializeComponent();
            CardImage.Source = image;

            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();
            string querry = "SELECT id, name FROM Deck where creator = '" + "ola123@ua.pt" + "'";
            SqlCommand decksSelect = new SqlCommand(querry, thisConnection);
            SqlDataReader querryCommandReader = decksSelect.ExecuteReader();

            List<string> decknames = new List<string>();
            List<int> deckids = new List<int>();

            while (querryCommandReader.Read())
            {
                decknames.Add(querryCommandReader["name"].ToString());
                deckids.Add((int)querryCommandReader["id"]);
            }

            decksCombo.ItemsSource = decknames;
            btnDialogOk.IsEnabled = false;
        }

        public addCardDialog(BitmapImage image, String deckname)
        {
            InitializeComponent();
            CardImage.Source = image;
            decksCombo.Text = deckname;
            decksCombo.IsEnabled = false;
            btnDialogOk.IsEnabled = true;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

        }

       


        private void lessCard_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(numberOfCards.Text) - 1 != 0))
                numberOfCards.Text = "" + (int.Parse(numberOfCards.Text) - 1);
        }

        private void moreCard_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(numberOfCards.Text) + 1 != 5))
                numberOfCards.Text = "" + (int.Parse(numberOfCards.Text) + 1);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^1-4]");

            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void decksCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDialogOk.IsEnabled = true;
        }
    }
}
