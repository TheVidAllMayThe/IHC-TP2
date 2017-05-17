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
        String currentQuerry;
        DataTable table;
        BitmapImage[] bis;
        string abilitiesStartingText;
        private static int currentPage;

        public Page1()
        {
            InitializeComponent();
            abilitiesStartingText = abilities_box.Text;
            borders = new Border[6];
            titles = new Label[6];
            images = new Image[6];
            contentsOfBorder = new Label[6][];
           

            for (int k = 0; k<6; k++)
            {
                Grid grid = new Grid();

                Grid buttongrid = new Grid();

                for (int w = 0; w < 2; w++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int w = 0; w < 7; w++)
                    grid.RowDefinitions.Add(new RowDefinition());

                ColumnDefinition col = new ColumnDefinition();

                col.Width = new GridLength(1, GridUnitType.Auto);

                grid.ColumnDefinitions.Add(col);

                for (int w = 0; w < 2; w++)
                    buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int w = 0; w < 7; w++)
                    buttongrid.RowDefinitions.Add(new RowDefinition());

                col = new ColumnDefinition();

                col.Width = new GridLength(1, GridUnitType.Auto);
                grid.ColumnDefinitions.Add(col);


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
                Grid.SetColumnSpan(viewBox, 2);

                Label[] tmp = new Label[6];

                

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
                    Grid.SetColumnSpan(viewBox, 2);

                }

                Button button = newCircularButton();
                button.Background = Brushes.Green;
                button.Foreground = Brushes.White;
                button.Width = 50;
                button.Height = 50;
                Label plus = new Label();
                plus.Content = "+";
                plus.Foreground = Brushes.White;
                plus.FontSize = 25;
                plus.FontWeight = FontWeights.Bold;
                button.Content = plus;
                button.Click += new RoutedEventHandler(this.addButton_Click);
                button.Name = "AddButton" + k;
                viewBox = new Viewbox();
                viewBox.Child = button;
                viewBox.Margin = new Thickness(0, 0, 15, 15);
                viewBox.HorizontalAlignment = HorizontalAlignment.Right;

                buttongrid.Children.Add(viewBox);
                Grid.SetRow(viewBox, 6);
                Grid.SetColumn(viewBox, 2);
                Grid.SetZIndex(viewBox, 3);

                borders[k].Child = grid;
                mainGrid.Children.Add(borders[k]);
                Grid.SetRow(borders[k], k / 3);
                Grid.SetColumn(borders[k], k % 3);

                mainGrid.Children.Add(buttongrid);
                Grid.SetRow(buttongrid, k / 3);
                Grid.SetColumn(buttongrid, k % 3);

                Grid.SetZIndex(buttongrid, 5);


                contentsOfBorder[k] = tmp;

            }

            currentQuerry = "SELECT id, multiverseID, Card.name as cardName, Edition.name as editionName, rarity, cmc FROM Card join Edition on edition = code ORDER BY ID DESC";
            setCards();
           
        }
        

        private void setCards()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            
            SqlCommand selectCard = new SqlCommand(currentQuerry, thisConnection);

            table = new DataTable("cards");
            SqlDataAdapter adapt = new SqlDataAdapter(selectCard);
            adapt.Fill(table);

            
            bis = new BitmapImage[table.Rows.Count];

            setPage(1);
        }

        private void setPage(int page)
        {
            int maxPageInt = table.Rows.Count / 6 + (table.Rows.Count % 6 == 0 ? 0 : 1);
            currentPage = page;
            Console.WriteLine(currentQuerry);

            if(page == maxPageInt)
            {
                BitmapImage image = new BitmapImage(new Uri("/magic_the_gathering.png", UriKind.Relative));
                for (int i = 0; i < 6; i++)
                {
                    images[i].Source = image;
                    titles[i].Content = "";
                    for (int k = 0; k < 6; k++)
                    {
                        contentsOfBorder[i][k].Content = "";
                    }
                }
            }

            if (table.Rows.Count != 0)
            {
                for (int i = page * 6 - 6; i < (page == maxPageInt ? table.Rows.Count : page * 6); i++)
                {
                    borders[i % 6] = new Border();
                    borders[i % 6].BorderThickness = new Thickness(1.5);

                    if (table.Rows[i]["multiverseID"] != null)
                    {
                        if (bis[i] == null)
                        {
                            string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + table.Rows[i]["multiverseID"] + @"&type=card";
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                            bi.EndInit();
                            bis[i] = bi;
                            images[i % 6].Source = bis[i];
                        }

                        else
                            images[i % 6].Source = bis[i];

                    }

                    titles[i % 6].Content = table.Rows[i]["cardName"];

                    contentsOfBorder[i % 6][0].Content = table.Rows[i]["editionName"];

                    contentsOfBorder[i % 6][1].Content = table.Rows[i]["rarity"];

                    var querry = "SELECT * FROM TypeOfCard where card = " + table.Rows[i]["id"];
                    SqlCommand selectTypes = new SqlCommand(querry, thisConnection);
                    DataTable types = new DataTable("types");
                    SqlDataAdapter adapt = new SqlDataAdapter(selectTypes);

                    adapt.Fill(types);

                    if (types.Rows.Count == 0)
                        contentsOfBorder[i % 6][2].Content = "---";
                    else
                    {
                        contentsOfBorder[i % 6][2].Content = "";
                        foreach (DataRow row in types.Rows)
                        {
                            contentsOfBorder[i % 6][2].Content = contentsOfBorder[i % 6][2].Content.ToString() + row["type"] + ", ";
                        }
                        contentsOfBorder[i % 6][2].Content = contentsOfBorder[i % 6][2].Content.ToString().Substring(0, contentsOfBorder[i % 6][2].Content.ToString().Length - 2);
                    }

                    querry = "SELECT * FROM SubTypeOfCard where card = " + table.Rows[i]["id"];
                    selectTypes = new SqlCommand(querry, thisConnection);
                    DataTable subtypes = new DataTable("types");
                    adapt = new SqlDataAdapter(selectTypes);
                    adapt.Fill(subtypes);

                    if (types.Rows.Count == 0)
                        contentsOfBorder[i % 6][3].Content = "---";
                    else
                    {
                        contentsOfBorder[i % 6][3].Content = "";
                        foreach (DataRow row in subtypes.Rows)
                        {
                            contentsOfBorder[i % 6][3].Content = contentsOfBorder[i % 6][3].Content.ToString() + row["subtype"] + ", ";
                        }

                        Console.WriteLine(contentsOfBorder[i % 6][3].Content);
                        if (contentsOfBorder[i % 6][3].Content.ToString().Trim().Equals(""))
                            contentsOfBorder[i % 6][3].Content = "---";
                        else
                            contentsOfBorder[i % 6][3].Content = contentsOfBorder[i % 6][3].Content.ToString().Substring(0, contentsOfBorder[i % 6][3].Content.ToString().Length - 2);
                    }

                    if (contentsOfBorder[i % 6][2].ToString().Contains("Creature"))
                    {
                        querry = "SELECT * FROM Creature where card = " + table.Rows[i]["id"];
                        SqlCommand powerselect = new SqlCommand(querry, thisConnection);
                        SqlDataReader querryCommandReader = powerselect.ExecuteReader();
                        querryCommandReader.Read();
                        contentsOfBorder[i % 6][4].Content = "Power: " + querryCommandReader["power"];
                        contentsOfBorder[i % 6][5].Content = "Toughness: " + querryCommandReader["toughness"];
                    }
                    else
                    {
                        contentsOfBorder[i % 6][4].Content = "";
                        contentsOfBorder[i % 6][5].Content = "";
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
            
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
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
            setPage(int.Parse(pageTextBox.Text)+1);
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            String type = typeComboBox.Text.Equals("Type") || typeComboBox.Text.Equals("None") ? "null": "'"+typeComboBox.Text+ "'";
            String b, g, u, w, r;
            b = BCheckBox.IsChecked.Value ? "1":"null";
            g = GCheckBox.IsChecked.Value ? "1":"null";
            u = UCheckBox.IsChecked.Value ? "1":"null";
            w = WCheckBox.IsChecked.Value ? "1":"null";
            r = RCheckBox.IsChecked.Value ? "1":"null";
            String edition = edition_box.Text.Equals("Edition") ? "null": "'" + edition_box.Text + "'";
            String minPower, maxPower, minTough, maxTough, minCMC, maxCMC;

            if (!min_power_box.Text.Equals(""))
                minPower = (min_power_box.Text);
            else
                minPower = "null";
            if (!max_power_box.Text.Equals(""))
                maxPower = (max_power_box.Text);
            else
                maxPower = "null";
            if (!min_toughness_box.Text.Equals(""))
                minTough = (min_toughness_box.Text);
            else
                minTough = "null";
            if (!max_toughness_box.Text.Equals(""))
                maxTough = (max_toughness_box.Text);
            else
                maxTough = "null";
            if (!min_cmc_box.Text.Equals(""))
                minCMC = (min_cmc_box.Text);
            else
                minCMC = "null";
            if (!max_cmc_box.Text.Equals(""))
                maxCMC = (max_cmc_box.Text);
            else
                maxCMC = "null";

            string abilities = abilities_box.Text.Equals(abilitiesStartingText) ? "null" : "'" + abilities_box.Text + "'";

            string rarity = rarity_combo_box.Text.Equals("Rarity") ? "null" : "'" + rarity_combo_box.Text + "'";

            currentQuerry = "SELECT * from search_cards(" + (searchBox.Text.Equals("Search")? "null" : "'" + searchBox.Text + "'") + ", " + type + ", " + g + ", " + u + ", " + w + ", " + r + ", " + b + ", " + abilities + ", " + edition + ", " + minPower + ", " + maxPower + ", " + minTough + ", " + maxTough + ", " + minCMC + ", " + maxCMC + ", " + rarity + ")";

            BitmapImage image = new BitmapImage(new Uri("/magic_the_gathering.png", UriKind.Relative));
            for (int i = 0; i < 6; i++)
            {
                images[i].Source = image;
                titles[i].Content = "";
                for (int k = 0; k < 6; k++)
                {
                    contentsOfBorder[i][k].Content = "";
                }
            }

            setCards();

        }

        private void border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int borderPressed = int.Parse(((Border)sender).Name.Substring(6));

            Card.Card_id = (int)table.Rows[(currentPage - 1) * 6 + borderPressed]["id"];

            
            NavigationService.Navigate(new Uri("Card.xaml", UriKind.Relative));
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            int buttonPressed = int.Parse(((Button)sender).Name.Substring(9));
            int rowNumber = (currentPage - 1) * 6 + buttonPressed;
            Card.Card_id = (int)table.Rows[rowNumber]["id"];
            addCardDialog add = (table.Rows[rowNumber]["rarity"].ToString().Equals("Basic Land") ? new addCardDialog((BitmapImage)images[rowNumber].Source, true): new addCardDialog((BitmapImage)images[rowNumber].Source, false));
            add.ShowDialog();
            if(add.DialogResult == true)
                Add_Card((int)table.Rows[rowNumber]["id"], add.DeckID, add.Amount, add.SideBoard);
        }

        public Button newCircularButton()
        {
            Button circleButton = new Button();
            
            ControlTemplate circleButtonTemplate = new ControlTemplate(typeof(Button));

            // Create the circle
            FrameworkElementFactory circle = new FrameworkElementFactory(typeof(Ellipse));
            circle.SetValue(Ellipse.FillProperty, Brushes.Black);
            circle.SetValue(Ellipse.StrokeProperty, Brushes.Green);
            circle.SetValue(Ellipse.StrokeThicknessProperty, 1.0);

            // Create the ContentPresenter to show the Button.Content
            FrameworkElementFactory presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            presenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(Button.ContentProperty));
            presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            
            // Create the Grid to hold both of the elements
            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            grid.AppendChild(circle);
            grid.AppendChild(presenter);

            // Set the Grid as the ControlTemplate.VisualTree
            circleButtonTemplate.VisualTree = grid;

            // Set the ControlTemplate as the Button.Template
            circleButton.Template = circleButtonTemplate;

            return circleButton;
        }

        private void Add_Card(int cardID, int deckID, int amount, bool sideboard)
        {
            
            SqlConnection thisConnection;
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            string getData = "EXEC addCardToDeck " + cardID + ", " + deckID + ", " + amount + ", " + (sideboard?1:0);
            try
            {
                new SqlCommand(getData, thisConnection).ExecuteNonQuery();
            }
            catch (SqlException e) {
                MessageBox.Show(e.Message.Split('.')[2], "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                thisConnection.Close();
            MessageBox.Show("Successfully added card");
        }
    }
}
