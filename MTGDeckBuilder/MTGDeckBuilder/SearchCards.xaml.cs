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
using System.Data;

namespace MTGDeckBuilder
{

    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>


    public partial class Page1 : Page
    {

        
        SqlConnection thisConnection;
        Border[] borders;
        Label[] titles;
        Label[][] contentsOfBorder;
        Image[] images;


        public Page1()
        {
            InitializeComponent();

            borders = new Border[6];
            titles = new Label[6];
            images = new Image[6];
            contentsOfBorder = new Label[6][];

            for (int k = 0; k<6; k++)
            {
                Grid grid = new Grid();

                borders[k] = new Border();
                borders[k].BorderThickness = new Thickness(1.5);

                Viewbox viewBox = new Viewbox();

                images[k] = new Image();
                images[k].Margin = new Thickness(10, 10, 10, 10);

                BitmapImage image = new BitmapImage(new Uri("/magic_the_gathering.png", UriKind.Relative));
                images[k].Source = image;

                viewBox.Child = images[k];

                grid.Children.Add(viewBox);
                Grid.SetRow(viewBox, 0);
                Grid.SetColumn(viewBox, 0);
                Grid.SetRowSpan(viewBox, 7);

                titles[k] = new Label();
                titles[k].Style = Application.Current.Resources["Card Title Style"] as Style;
                titles[k].Content = "Cena";

                viewBox = new Viewbox();
                viewBox.Child = titles[k];
                viewBox.HorizontalAlignment = HorizontalAlignment.Left;


                grid.Children.Add(viewBox);
                Grid.SetRow(viewBox, 0);
                Grid.SetColumn(viewBox, 1);

                Label[] tmp = new Label[6];

                for (int w = 0; w < 2; w++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int w = 0; w < 7; w++)
                    grid.RowDefinitions.Add(new RowDefinition());

                for (int w=0 ; w<6 ; w++)
                {
                    Label tmpp = new Label();
                    tmpp.Style = Application.Current.Resources["Card Property Style"] as Style;
                    tmpp.Content = "Test";
                    tmp[w] = tmpp;

                    viewBox = new Viewbox();
                    viewBox.Child = tmpp;
                    viewBox.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Children.Add(viewBox);
                    Grid.SetRow(viewBox, w + 1);
                    Grid.SetColumn(viewBox, 1);

                }

                borders[k].Child = grid;
                mainGrid.Children.Add(borders[k]);
                Grid.SetRow(borders[k], k / 3);
                Grid.SetColumn(borders[k], k % 3);


                contentsOfBorder[k] = tmp;

            }

            setCards("SELECT * FROM Card ORDER BY ID DESC");
           
        }
        

        private void setCards(String querry)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = querry;

            SqlCommand selectCard = new SqlCommand(getData, thisConnection);

            DataTable table = new DataTable("cards");
            SqlDataAdapter adapt = new SqlDataAdapter(selectCard);
            adapt.Fill(table);

            Console.WriteLine(table.Rows.Count);

            for (int i = 0; i < 6; i++)
            {
                borders[i] = new Border();
                borders[i].BorderThickness = new Thickness(1.5);

                if (table.Rows[i]["multiverseID"] != null)
                {
                    string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + table.Rows[i]["multiverseID"] + @"&type=card";
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    
                    images[i].Source = bi;
                }

                titles[i].Content = table.Rows[i]["name"];

            }
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
