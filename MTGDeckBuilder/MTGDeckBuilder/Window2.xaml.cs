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
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>

    public class Listing
    {
        private int _id;
        private string _user;
        private string _startDate;
        private String _endDate;
        private int _sell;
        private double _totalPrice;

        public string User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
            }
        }
        public string StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
            }
        }
        public string EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
            }
        }
        public int Sell
        {
            get
            {
                return _sell;
            }

            set
            {
                _sell = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public double TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                _totalPrice = value;
            }
        }
    }
    public class CardInListing
    {
        private int _id;
        private int _listingid;
        private string _user;
        private int _card;
        private string _cardname;
        private double _priceperunit;
        private string _condition;
        private int _units;
        private string _startdate;
        private int _amountToBuy;


        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int Listingid
        {
            get
            {
                return _listingid;
            }

            set
            {
                _listingid = value;
            }
        }

        public int Card
        {
            get
            {
                return _card;
            }

            set
            {
                _card = value;
            }
        }

        public string Cardname
        {
            get
            {
                return _cardname;
            }

            set
            {
                _cardname = value;
            }
        }

        public double Priceperunit
        {
            get
            {
                return _priceperunit;
            }

            set
            {
                _priceperunit = value;
            }
        }

        public string Condition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }
        public int Units
        {
            get
            {
                return _units;
            }

            set
            {
                _units = value;
            }
        }   
        public string User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
            }
        }
        
        public string Startdate
        {
            get
            {
                return _startdate;
            }

            set
            {
                _startdate = value;
            }
        }

        public int AmountToBuy
        {
            get
            {
                return _amountToBuy;
            }

            set
            {
                _amountToBuy = value;
            }
        }
    }
        public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            updateVisual();
        }

        private void buy(object sender, RoutedEventArgs e)
        {
            CardInListing l = sender as CardInListing;
            if(l.AmountToBuy > l.Units || l.AmountToBuy<0)
            {
                MessageBox.Show("Can't buy that much");
                return;
            }
        }

        private void updateVisual()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;

            SqlConnection thisConnection = new SqlConnection(@cs);
            thisConnection.Open();

            String getData = "SELECT * FROM listings(1, 0)";
            SqlDataReader dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            ObservableCollection<CardInListing> temp = new ObservableCollection<CardInListing>();
            while (dr.Read())
            {
                temp.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["listingid"].ToString()), User = dr["User"].ToString(), Startdate = dr["StartDate"].ToString(), Card = int.Parse(dr["card"].ToString()), Cardname = dr["cardname"].ToString(), Priceperunit = double.Parse(dr["priceperunit"].ToString()), Condition = dr["condition"].ToString(), Units = int.Parse(dr["units"].ToString()) });
            }
            //selling.ItemsSource = temp;
            listBox_s.ItemsSource = temp;

            getData = "SELECT * FROM listings(0, 0)";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();

            temp = new ObservableCollection<CardInListing>();

            while (dr.Read())
            {
                temp.Add(new CardInListing { Id = int.Parse(dr["ID"].ToString()), Listingid = int.Parse(dr["listingid"].ToString()), User = dr["User"].ToString(), Startdate = dr["StartDate"].ToString(), Card = int.Parse(dr["card"].ToString()), Cardname = dr["cardname"].ToString(), Priceperunit = double.Parse(dr["priceperunit"].ToString()), Condition = dr["condition"].ToString(), Units = int.Parse(dr["units"].ToString()) });
            }

            listBox_b.ItemsSource = temp;
            ;
            dr.Close();

            getData = "EXEC usp_sellingListingsSelect";
            dr = new SqlCommand(getData, thisConnection).ExecuteReader();
            ObservableCollection<Listing> listings = new ObservableCollection<Listing>(); 
            while (dr.Read()) {

                SqlDataReader dr2 = new SqlCommand("SELECT dbo.totalListingPrice(" + dr["ID"] + ")", thisConnection).ExecuteReader();
                dr2.Read();
                listings.Add(new Listing { Id = int.Parse(dr["ID"].ToString()), EndDate = dr["EndDate"].ToString(), StartDate = (dr["StartDate"] == null ? "null" : dr["StartDate"].ToString()), TotalPrice = (dr2.GetValue(0) == null ? 0.0 : dr2.GetDouble(0)) });   

            }

            listBox_ls.ItemsSource = listings;
        }
    }
}
