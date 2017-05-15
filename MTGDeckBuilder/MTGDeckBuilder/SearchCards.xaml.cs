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
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        SqlConnection thisConnection;
        Border[] borders;
        public Page1()
        {
            InitializeComponent();

            
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "SELECT TOP 6 * FROM Card ORDER BY ID DESC";

            SqlCommand selectCard = new SqlCommand(getData, thisConnection);
            SqlDataReader reader = selectCard.ExecuteReader();
            
            borders = new Border[6];
            

            for(int i = 0; i<6; i++)
            {
                reader.Read();
                borders[i] = new Border();
                borders[i].BorderThickness = new Thickness(1.5);
                Grid grid = new Grid();
                for (int k = 0; k < 2; k++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int k = 0; k < 7; k++)
                    grid.RowDefinitions.Add(new RowDefinition());

                Image img = new Image();
                img.Margin = new Thickness(10, 10, 10, 10);
                if (reader["multiverseID"] != null)
                {
                    string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + reader["multiverseID"] + @"&type=card";
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;

                }

                else {
                    BitmapImage image = new BitmapImage(new Uri("/MTGDeckBuilder;component/Images/magic_the_gathering.png", UriKind.Relative));
                    img.Source = image;
                }

                Label nameOfCard = new Label();

                Label[] content = new Label[6];

                Viewbox viewBox;

                for (int k = 0; k<6; k++) { 
                    content[k] = new Label();
                    content[k].Style = Application.Current.Resources["Card Property Style"] as Style;
                    content[k].Content = "Test";
                    viewBox = new Viewbox();
                    viewBox.Child = content[k];
                    viewBox.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Children.Add(viewBox);
                    Grid.SetRow(viewBox, k + 1);
                    Grid.SetColumn(viewBox, 1);
                }

                nameOfCard.Style = Application.Current.Resources["Card Title Style"] as Style;
                nameOfCard.Content = reader["name"];
                viewBox = new Viewbox();
                viewBox.Child = nameOfCard;
                viewBox.HorizontalAlignment = HorizontalAlignment.Left;
                grid.Children.Add(viewBox);

                Grid.SetRow(viewBox, 0);
                Grid.SetColumn(viewBox, 1);

                grid.Children.Add(img);
                Grid.SetRow(img, 0);
                Grid.SetRowSpan(img, 7);
                Grid.SetColumn(img, 0);

                borders[i].Child = grid;
                mainGrid.Children.Add(borders[i]);
                Grid.SetRow(borders[i], i / 3);
                Grid.SetColumn(borders[i], i %3);
            }
            Console.WriteLine(reader["name"]);


            more_options_border.Visibility = Visibility.Hidden;
           
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Equals("Search"))
            {
                searchBox.Foreground = Brushes.Black;
                searchBox.Text = "";
            }
        }

        private void abilityBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Equals("Search"))
            {
                searchBox.Foreground = Brushes.Black;
                searchBox.Text = "";
            }
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void More_Options_Click(object sender, RoutedEventArgs e)
        {
            var run = More_Options.Inlines.FirstOrDefault() as Run;
            string text = run == null ? string.Empty : run.Text;

            if (text.Equals("More Options ▲"))
            {
                More_Options.Inlines.Clear();
                More_Options.Inlines.Add("More Options ▼");
                more_options_border.Visibility = Visibility.Hidden;
            }

            else
            {
                More_Options.Inlines.Clear();
                More_Options.Inlines.Add("More Options ▲");
                more_options_border.Visibility = Visibility.Visible;
            }


        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (abilities_box.Text.Equals("Abilities (ex: \"Flying\", \"Double Strike\")    "))
            {

                abilities_box.Foreground = Brushes.Black;
                abilities_box.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (abilities_box.Text.Trim().Equals(""))
            {
                abilities_box.Foreground = Brushes.Gray;
                abilities_box.Text = "Abilities (ex: \"Flying\", \"Double Strike\")    ";
            }
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Trim().Equals(""))
            {
                searchBox.Foreground = Brushes.Gray;
                searchBox.Text = "Search";
            }
        }

        private void edition_box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (edition_box.Text.Equals("Edition"))
            {

                edition_box.Foreground = Brushes.Black;
                edition_box.Text = "";
            }
        }

        private void edition_box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (edition_box.Text.Trim().Equals(""))
            {
                edition_box.Foreground = Brushes.Gray;
                edition_box.Text = "Edition";
            }
        }

        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Opacity = 0.2;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Opacity = 0;
        }
    }
}
