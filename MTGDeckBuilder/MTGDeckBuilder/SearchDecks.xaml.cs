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
        SqlConnection thisConnection;
        int currentPage;
        Label[] titles;
        Label[] creators;
        StackPanel[] colors;
        StackPanel[] rating;
        Image[] stars;
        Image[] trash;
        BitmapImage trashBitMap;
        public SearchDecks()
        {
            InitializeComponent();
            currentQuerry = "SELECT * FROM Deck";
            addButtonViewBox.Visibility = Visibility.Hidden;
            construct(false);
        }

        public SearchDecks(string user) {
            InitializeComponent();
            currentQuerry = "SELECT * FROM Deck where (creator = '" + user + "')";
            construct(true);
        }

        private void construct(bool isMyDecks)
        {
            
            titles = new Label[10];
            creators = new Label[10];
            colors = new StackPanel[10];
            rating = new StackPanel[10];
            trash = new Image[10];
            Viewbox viewBox;
            trashBitMap = new BitmapImage(new Uri("/images/Trash_icon.png", UriKind.Relative));

            for (int i = 0; i < 10; i++)
            {
                Grid trashGrid = new Grid();
                Grid deckGrid = new Grid();
                for (int w = 0; w < 5; w++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width =  w < 4 ? new GridLength(1, GridUnitType.Star) : new GridLength(1, GridUnitType.Auto);
                    deckGrid.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = w < 4 ? new GridLength(1, GridUnitType.Star) : new GridLength(1, GridUnitType.Auto);
                    trashGrid.ColumnDefinitions.Add(col);
                }

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

                trash[i] = new Image();
                trash[i].Name = "Trash" + i;
                trash[i].Margin = new Thickness(50, 25, 0, 25);
                viewBox = new Viewbox();
                viewBox.Margin = new Thickness(50, 25, 0, 25);
                viewBox.Child = trash[i];
                
                trashGrid.Children.Add(viewBox);
                viewBox.Margin = new Thickness(10, 10, 10, 10);
                Grid.SetColumn(viewBox, 4);
                Grid.SetZIndex(viewBox, 200);

                DeckGrid.Children.Add(deckGrid);
                DeckGrid.Children.Add(trashGrid);
                Grid.SetRow(deckGrid, i);
                Grid.SetRow(trashGrid, i);
                Grid.SetZIndex(trashGrid, 200);
                if (!isMyDecks)
                    trashGrid.Visibility = Visibility.Collapsed;
            }
            setDecks(1);
        }
        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (abilities_box.Text.Equals("Abilities/Creatures..."))
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
                abilities_box.Text = "Abilities/Creatures...";
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

        private void setDecks(int i)
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            SqlCommand selectDeck = new SqlCommand(currentQuerry, thisConnection);

            table = new DataTable("decks");
            SqlDataAdapter adapt = new SqlDataAdapter(selectDeck);
            adapt.Fill(table);

            setPage(i);
        }

        private void setPage(int page)
        {
            
            int maxPageInt = table.Rows.Count / 10 + (table.Rows.Count % 10 == 0 ? 0 : 1);
            currentPage = page;
            Console.WriteLine(currentQuerry);

            for (int i = 0; i<10; i++)
            {
                titles[i % 10].Content = null;
                trash[i].Source = null;
                creators[i % 10].Content = null;
                rating[i % 10].Children.Clear();
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
                    string querry = "SELECT color FROM DeckColors WHERE deck = " + table.Rows[i]["id"];
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
                        rating[i%10].Children.Add(img);
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
                        rating[i%10].Children.Add(img);
                    }


                    trash[i % 10].Source = trashBitMap;
                    trash[i % 10].MouseLeftButtonUp += new MouseButtonEventHandler(deleteDeck);
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

        private void deleteDeck(object sender, MouseButtonEventArgs e)
        {
            int deckID = int.Parse(table.Rows[int.Parse(((Image)sender).Name.Substring(5)) + (currentPage - 1) * 10]["id"].ToString());

            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            String getData = "DELETE FROM Deck WHERE id = " + deckID;
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            setDecks(currentPage);
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

        private void Search(object sender, RoutedEventArgs e)
        {
            String type = (typeComboBox.Text.Equals("Any") || typeComboBox.Text.Equals("")) ? "null" : "'" + typeComboBox.Text + "'";
            String b, g, u, w, r;
            b = BCheckBox.IsChecked.Value ? "1" : "null";
            g = GCheckBox.IsChecked.Value ? "1" : "null";
            u = UCheckBox.IsChecked.Value ? "1" : "null";
            w = WCheckBox.IsChecked.Value ? "1" : "null";
            r = RCheckBox.IsChecked.Value ? "1" : "null";

            String name = (searchBox.Text.Equals("Name") || searchBox.Text.Equals("") ? "null" : "'" + searchBox.Text + "'");
            String cards = (cards_box.Text.Equals("Cards") || cards_box.Text.Equals("")) ? "null" : "'" + cards_box.Text + "'";
            String abilities = (abilities_box.Text.Equals("Abilities") || abilities_box.Text.Equals("")) ? "null" : "'" + abilities_box.Text + "'";
            String minLands, maxLands, minCreatures, maxCreatures, minSpells, maxSpells, minArtifacts, maxArtifacts, minEnchantments, maxEnchantments, minInstants, maxInstants;
            
            if (!min_lands_box.Text.Equals(""))
                minLands = (min_lands_box.Text);
            else
                minLands = "null";
            if (!max_lands_box.Text.Equals(""))
                maxLands = (max_lands_box.Text);
            else
                maxLands = "null";
            if (!min_creatures_box.Text.Equals(""))
                minCreatures = (min_creatures_box.Text);
            else
                minCreatures = "null";
            if (!max_creatures_box.Text.Equals(""))
                maxCreatures = (max_creatures_box.Text);
            else
                maxCreatures = "null";
            if (!min_spells_box.Text.Equals(""))
                minSpells = (min_spells_box.Text);
            else
                minSpells = "null";
            if (!max_spells_box.Text.Equals(""))
                maxSpells = (max_spells_box.Text);
            else
                maxSpells = "null";
            if (!min_artifacts_box.Text.Equals(""))
                minArtifacts = (min_artifacts_box.Text);
            else
                minArtifacts = "null";
            if (!max_artifacts_box.Text.Equals(""))
                maxArtifacts = (max_artifacts_box.Text);
            else
                maxArtifacts = "null";
            if (!min_enchantments_box.Text.Equals(""))
                minEnchantments = (min_enchantments_box.Text);
            else
                minEnchantments = "null";
            if (!max_enchantments_box.Text.Equals(""))
                maxEnchantments = (max_enchantments_box.Text);
            else
                maxEnchantments = "null";
            if (!min_instants_box.Text.Equals(""))
                minInstants = (min_instants_box.Text);
            else
                minInstants = "null";
            if (!max_instants_box.Text.Equals(""))
                maxInstants = (max_instants_box.Text);
            else
                maxInstants = "null";

            currentQuerry = "SELECT * from search_decks(" + name + ", " + cards + ',' + abilities + ", " + type + ',' + g + ", " + u + ", " + w + ", " + r + ", " + b + ", " + minLands + ", " + maxLands + ", " + minCreatures + ", " + maxCreatures + ", " + minSpells + ", " + maxSpells + ", " + minArtifacts + ',' + maxArtifacts + ',' + minEnchantments + ',' + maxEnchantments + ',' + minInstants + ',' + maxInstants + ")";

            setDecks(1);
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            typeComboBox.SelectionChanged -= this.Search;
            GCheckBox.Checked -= this.Search;
            UCheckBox.Checked -= this.Search;
            RCheckBox.Checked -= this.Search;
            WCheckBox.Checked -= this.Search;
            BCheckBox.Checked -= this.Search;
            
            searchBox.Text = "Name";
            typeComboBox.SelectedIndex = 0;
            GCheckBox.IsChecked = false;
            UCheckBox.IsChecked = false;
            RCheckBox.IsChecked = false;
            WCheckBox.IsChecked = false;
            BCheckBox.IsChecked = false;
            abilities_box.Text = "Abilities";
            cards_box.Text = "Cards";
            min_lands_box.Text = "";
            max_lands_box.Text = "";
            min_instants_box.Text = "";
            max_instants_box.Text = "";
            min_creatures_box.Text = "";
            max_creatures_box.Text = "";
            min_artifacts_box.Text = "";
            max_artifacts_box.Text = "";
            min_enchantments_box.Text = "";
            max_enchantments_box.Text = "";
            min_spells_box.Text = "";
            max_spells_box.Text = "";

            typeComboBox.SelectionChanged += this.Search;
            GCheckBox.Checked += this.Search;
            UCheckBox.Checked += this.Search;
            RCheckBox.Checked += this.Search;
            WCheckBox.Checked += this.Search;
            BCheckBox.Checked += this.Search;
        }
        
        private void cards_box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (cards_box.Text.Equals("Cards"))
            {

                cards_box.Foreground = Brushes.Black;
                cards_box.Text = "";
            }
        }

        private void cards_box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cards_box.Text.Trim().Equals(""))
            {
                cards_box.Foreground = Brushes.Gray;
                cards_box.Text = "Cards";
            }
        }

        private void abilities_box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (cards_box.Text.Equals("Abilities"))
            {

                cards_box.Foreground = Brushes.Black;
                cards_box.Text = "";
            }
        }

        private void abilities_box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cards_box.Text.Trim().Equals(""))
            {
                cards_box.Foreground = Brushes.Gray;
                cards_box.Text = "Abilities";
            }
        }

        private void AdvancedSearchToggle(object sender, RoutedEventArgs e)
        {
            PointCollection pc = new PointCollection();
            Point p1;
            Point p2;
            Point p3;
            if (more_options_row.Height == new GridLength(0))
            {
                more_options_row.Height = GridLength.Auto;
                p1 = new Point(25, 4.5);
                p2 = new Point(4.5, 45.5);
                p3 = new Point(45.5, 45.5);
            }

            else
            {
                more_options_row.Height = new GridLength(0);
                p1 = new System.Windows.Point(25, 45.5);
                p2 = new System.Windows.Point(4.5, 4.5);
                p3 = new System.Windows.Point(45.5, 4.5);
            }
            pc.Add(p1);
            pc.Add(p2);
            pc.Add(p3);
            triangle.Points = pc;

        }

        private void search_KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Search(sender, e);
        }
    }
}