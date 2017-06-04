using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
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

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void txtSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Foreground = Brushes.White;
            if (((TextBox)sender).Text.Trim().Equals("Email"))
            {
                ((TextBox)sender).Text = "";
            }
        }

        private void txtSearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text.Trim().Equals(""))
            {
                ((TextBox)sender).Text = "Email";
                ((TextBox)sender).Foreground = Brushes.LightGray;
            }
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((PasswordBox)sender).Password.Trim().Equals(""))
            {
                ((PasswordBox)sender).Password = "Password";
                ((PasswordBox)sender).Foreground = Brushes.LightGray;
            }
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            ((PasswordBox)sender).Foreground = Brushes.White;
            if (((PasswordBox)sender).Password.Trim().Equals("Password"))
            {
                ((PasswordBox)sender).Password = "";
            }
        }

        private void search_KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (username.Text.Trim().Equals(""))
                        throw new FormatException();
                    var test = new MailAddress(username.Text);
                    
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("You have inserted an invalid email!", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (password.Password.Length < 6)
                {
                    MessageBox.Show("You have inserted an invalid password!", "Invalid Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(@cs))
                using (SqlCommand cmd = new SqlCommand("usp_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@user", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@pass", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@r", SqlDbType.Int).Direction = ParameterDirection.Output;

                    // set parameter values
                    cmd.Parameters["@user"].Value = username.Text;
                    cmd.Parameters["@pass"].Value = password.Password;
                    // open connection and execute stored procedure
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // read output value from @NewId
                    int id = Convert.ToInt32(cmd.Parameters["@r"].Value);
                    conn.Close();
                    Console.WriteLine(id);


                    bool isRegistered = id == 1;

                    if (isRegistered)
                    {
                        App.User = username.Text;
                        NavigationService.Navigate(new Uri("Home.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("You have inserted a wrong Email/Password combo!", "Wrong Credentials", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
        }
    }
}
