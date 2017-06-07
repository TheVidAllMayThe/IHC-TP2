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

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Image1.Opacity = 0.8;
            label.Foreground = Brushes.Black;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Image1.Opacity = 1;
            label.Foreground = Brushes.White;
        }

        private void border2_MouseEnter(object sender, MouseEventArgs e)
        {
            Image2.Opacity = 0.8;
            label_Copy.Foreground = Brushes.Black;
        }

        private void border2_MouseLeave(object sender, MouseEventArgs e)
        {
            Image2.Opacity = 1;
            label_Copy.Foreground = Brushes.White;

        }

        private void border3_MouseEnter(object sender, MouseEventArgs e)
        {
            Image3.Opacity = 0.8;
            label_Copy1.Foreground = Brushes.Black;
        }

        private void border3_MouseLeave(object sender, MouseEventArgs e)
        {
            Image3.Opacity = 1;
            label_Copy1.Foreground = Brushes.White;
        }

        private void border4_MouseEnter(object sender, MouseEventArgs e)
        {
            Image4.Opacity = 0.8;
            label_Copy2.Foreground = Brushes.Black;
        }

        private void border4_MouseLeave(object sender, MouseEventArgs e)
        {
            Image4.Opacity = 1;
            label_Copy2.Foreground = Brushes.White;
        }

        private void search_cards_border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Margin = new Thickness(0, 0, 0, 0);
            if (Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
            {
                SearchCards sc = new SearchCards();
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(sc);
            }
        }

        private void search_cards_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Margin = new Thickness(3, 3, 3, 3);
        }

        private void search_decks_border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
            {
                SearchDecks sd = new SearchDecks();
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(sd);
            }
        }

        private void search_decks_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Margin = new Thickness(3, 3, 3, 3);
        }

        private void build_deck_border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Margin = new Thickness(0, 0, 0, 0);
            newDeckDialog dialog = new newDeckDialog();
            dialog.ShowDialog();
            if(dialog.DialogResult == true && Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
                {
                    Deck d = new Deck(dialog.deck_id);
                    ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(d);
                }
        }

        private void build_deck_border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Margin = new Thickness(3, 3, 3, 3);
        }

        private void mydecksBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) != null) //Avoid double click null pointer exceptions
            {
                SearchDecks sd = new SearchDecks(App.User);
                ((MainWindow)Window.GetWindow(this)).MainFrame.Navigate(sd);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 gameWindow = new Window1();
            gameWindow.Show();
            MainWindow main = Application.Current.MainWindow as MainWindow;
            main.Close();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window2 gameWindow = new Window2();
            gameWindow.Show();

        }
    }
}
