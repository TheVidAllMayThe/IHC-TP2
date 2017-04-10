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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Image1.Opacity = 0.5;
            label.Foreground = Brushes.Black;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Image1.Opacity = 1;
            label.Foreground = Brushes.White;
        }

        private void border2_MouseEnter(object sender, MouseEventArgs e)
        {
            Image2.Opacity = 0.5;
            label_Copy.Foreground = Brushes.Black;
        }

        private void border2_MouseLeave(object sender, MouseEventArgs e)
        {
            Image2.Opacity = 1;
            label_Copy.Foreground = Brushes.White;

        }

        private void border3_MouseEnter(object sender, MouseEventArgs e)
        {
            Image3.Opacity = 0.5;
            label_Copy1.Foreground = Brushes.Black;
        }

        private void border3_MouseLeave(object sender, MouseEventArgs e)
        {
            Image3.Opacity = 1;
            label_Copy1.Foreground = Brushes.White;
        }

        private void border4_MouseEnter(object sender, MouseEventArgs e)
        {
            Image4.Opacity = 0.5;
            label_Copy2.Foreground = Brushes.Black;
        }

        private void border4_MouseLeave(object sender, MouseEventArgs e)
        {
            Image4.Opacity = 1;
            label_Copy2.Foreground = Brushes.White;
        }
    }
}
