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
    /// Interaction logic for SearchDecks.xaml
    /// </summary>
    public partial class SearchDecks : Page
    {

        DataTable table;
        string currentQuerry;
        int currentPage;
        Label[] titles;
        Label[] creators;
        StackPanel[] colors;
        StackPanel[] rating;
        Image[] stars;

        public SearchDecks()
        {
            InitializeComponent();
            currentQuerry = "SELECT * FROM Deck";
            addButtonViewBox.Visibility = Visibility.Hidden;
            construct();
        }

        public SearchDecks(string user) {
            InitializeComponent();
            currentQuerry = "SELECT * FROM Deck where (creator = '" + user + "')";
            construct();
        }

        private void construct()
        {
            
            titles = new Label[10];
            creators = new Label[10];
            colors = new StackPanel[10];
            rating = new StackPanel[10];
            Viewbox viewBox;

            for (int i = 0; i < 10; i++)
            {
                Grid deckGrid = new Grid();
                for (int w = 0; w < 4; w++)
                    deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

                titles[i] = new Label();
                titles[i].Style = Application.Current.Resources["Card Title Style"] as Style;
                titles[i].Content = "Name of Deck #" + i;
                viewBox = new Viewbox();
                viewBox.Child = titles[i];
                viewBox.HorizontalAlignment = HorizontalAlignment.Left;
                deckGrid.Children.Add(viewBox);
                Grid.SetColumn(viewBox, 0);

                creators[i] = new Label();
                creators[i].Style = Application.Current.Resources["Card Title Style"] as Style;
                viewBox = new Viewbox();
                viewBox.Child = creators[i];
                viewBox.HorizontalAlignment = HorizontalAlignment.Left;
                deckGrid.Children.Add(viewBox);
                Grid.SetColumn(viewBox, 1);

                colors[i] = new StackPanel();
                colors[i].Orientation = Orientation.Horizontal;
                viewBox = new Viewbox();
                viewBox.Child = colors[i];
                deckGrid.Children.Add(viewBox);
                Grid.SetColumn(viewBox, 2);

                rating[i] = new StackPanel();
                rating[i].Orientation = Orientation.Horizontal;
                viewBox = new Viewbox();
                viewBox.Child = rating[i];
                deckGrid.Children.Add(viewBox);
                Grid.SetColumn(viewBox, 3);

                DeckGrid.Children.Add(deckGrid);
                Grid.SetRow(deckGrid, i);

            }
            setDecks();
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
            if (contains_box.Text.Equals("Abilities/Creatures..."))
            {

                contains_box.Foreground = Brushes.Black;
                contains_box.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (contains_box.Text.Trim().Equals(""))
            {
                contains_box.Foreground = Brushes.Gray;
                contains_box.Text = "Abilities/Creatures...";
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

        private void nextPageClick(object sender, RoutedEventArgs e)
        {
            setPage(int.Parse(pageTextBox.Text) + 1);
        }

        private void previousPageClick(object sender, RoutedEventArgs e)
        {
            setPage(int.Parse(pageTextBox.Text) - 1);
        }

        private void pageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.Parse(((TextBox)sender).Text) == 0)
                {
                    setPage(1);
                    e.Handled = true;
                    return;
                }

                if (int.Parse(((TextBox)sender).Text) > int.Parse(maxPage.Content.ToString().Substring(1)))
                {
                    MessageBoxResult result = MessageBox.Show("You have selected an invalid page", "Wrong Page", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Handled = true;
                    return;
                }

                setPage(int.Parse(pageTextBox.Text));
                e.Handled = true;
            }
        }

        private void setDecks()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();


            SqlCommand selectCard = new SqlCommand(currentQuerry, thisConnection);

            table = new DataTable("decks");
            SqlDataAdapter adapt = new SqlDataAdapter(selectCard);
            adapt.Fill(table);

            setPage(1);
        }

        private void setPage(int page)
        {
            
            int maxPageInt = table.Rows.Count / 10 + (table.Rows.Count % 10 == 0 ? 0 : 1);
            currentPage = page;
            Console.WriteLine(currentQuerry);

            for (int i = 0; i<10; i++)
            {
                titles[i % 10].Content = null;
                creators[i % 10].Content = null;
                colors[i].Children.Clear();
            }

            if (page == maxPageInt)
            {
                for (int i = 0; i < 10; i++)
                {
                    titles[i].Content = "";
                    creators[i].Content = "";
                }
            }

            if (table.Rows.Count != 0)
            {
                for (int i = page * 10 - 10; i < (page == maxPageInt ? table.Rows.Count : page * 10); i++)
                {
                    titles[i % 10].Content = table.Rows[i]["name"];
                    creators[i % 10].Content = table.Rows[i]["creator"];


                    string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
                    SqlConnection thisConnection = new SqlConnection(@cs);
                    thisConnection.Open();
                    string querry = "SELECT * FROM getColorsFromDeck(" + table.Rows[i]["id"] + ")";
                    SqlCommand powerselect = new SqlCommand(querry, thisConnection);
                    SqlDataReader querryCommandReader = powerselect.ExecuteReader();

                    while (querryCommandReader.Read())
                    {
                        BitmapImage image = new BitmapImage(new Uri("/Mana_"+ querryCommandReader["color"] + ".png", UriKind.Relative));
                        Image img = new Image();
                        img.Source = image;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.Height = 200;
                        img.Width = 200;
                        img.Margin = new Thickness(25, 50, 25, 50);
                        colors[i].Children.Add(img);
                    }

                    int ratingOfDeck = (int)Math.Round(table.Rows[i]["rating"].ToString().Equals("") ? 0 : Double.Parse(table.Rows[i]["rating"].ToString()));
                    for (int k = 0; k < ratingOfDeck; k++)
                    {
                        BitmapImage image = new BitmapImage(new Uri("/images/full_star.png", UriKind.Relative));
                        Image img = new Image();
                        img.Source = image;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.Height = 200;
                        img.Width = 200;
                        img.Margin = new Thickness(25, 50, 25, 50);
                        rating[i].Children.Add(img);
                    }
                    for (int k = 0; k < 5 - ratingOfDeck; k++)
                    {
                        BitmapImage image = new BitmapImage(new Uri("/images/empty_star.png", UriKind.Relative));
                        Image img = new Image();
                        img.Source = image;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.Height = 200;
                        img.Width = 200;
                        img.Margin = new Thickness(25, 50, 25, 50);
                        rating[i].Children.Add(img);
                    }
                }
            }
            maxPage.Content = "/" + maxPageInt;

            pageTextBox.Text = "" + page;

            if (page == maxPageInt)
                nextPage.IsEnabled = false;
            else
                nextPage.IsEnabled = true;

            if (page == 1)
                previousPage.IsEnabled = false;
            else
                previousPage.IsEnabled = true;
            
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if((currentPage - 1) * 10 + int.Parse(((Border)sender).Name.Substring(6)) < table.Rows.Count && Window.GetWindow(this) != null) { 
                Deck d = new Deck(int.Parse(table.Rows[(currentPage-1) * 10 + int.Parse(((Border)sender).Name.Substring(6))]["id"].ToString()));
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(d);
            }
        }

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 0.8;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 1;
        }

        private void addButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((Viewbox)sender).Margin = new Thickness(0, 0, 0, 0);
            newDeckDialog dialog = new newDeckDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == true && Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
            {
                Deck d = new Deck(dialog.deck_id);
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(d);
            }
        }

    }
}