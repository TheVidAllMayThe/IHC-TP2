using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Data;

namespace MTGDeckBuilder
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    
    public partial class Card : Page
    {
        private int deck_id;
        private int card_id;
        private String editionKey;
        private SqlConnection thisConnection;

        public Card(int card_id, int deck_id = -1)
        {
            this.deck_id = deck_id;
            this.card_id = card_id;
            InitializeComponent();
            show_card();
        }

        public void add_buttonClick(object sender, RoutedEventArgs e)
        {
            addCardDialog add = new addCardDialog(((BitmapImage)image.Source), rarity.Content.ToString() == "Basic Land");
            add.ShowDialog();
            if (add.DialogResult == true)
                App.Add_Card(card_id, add.DeckName, add.DeckID, add.Amount, add.SideBoard, name.Content.ToString());
        }

        private void addButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 0.8;
        }

        private void addButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Opacity = 1;
        }


        public void show_card()
        {
            string cs = ConfigurationManager.ConnectionStrings["magicConnect"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(@cs))
            {
                using (SqlCommand cmd = new SqlCommand("usp_CardSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@id", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@id"].Value = this.card_id;
                    // open connection and execute stored procedure
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    name.Content = dr["name"] == null ? "---" : dr["name"].ToString();
                    text.Content = dr["text"] == null ? "---" : dr["text"].ToString();
                    rarity.Content = dr["rarity"] == null ? "---" : dr["rarity"].ToString();
                    artist.Content = dr["artist"] == null ? "---" : dr["artist"].ToString();
                    this.editionKey = dr["edition"] == null ? "---" : dr["edition"].ToString();
                    int multiverseID = (dr["multiverseID"] == null || dr["multiverseID"].ToString() == "") ? 0 : int.Parse(dr["multiverseID"].ToString());
                    dr.Close();

                    if (multiverseID != null)
                    {
                        string fullFilePath = @"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + multiverseID + @"&type=card";
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(fullFilePath, UriKind.RelativeOrAbsolute);
                        bi.EndInit();
                        image.Source = bi;
                    }
                }
                using (SqlCommand cmd = new SqlCommand("usp_FlavorSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@card", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@card"].Value = this.card_id;
                    // open connection and execute stored procedure

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) flavor.Content = dr["flavor"];
                    else flavor.Content = "---";
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("usp_TypeOfCardSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@card", SqlDbType.Int);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@card"].Value = this.card_id;
                    cmd.Parameters["@type"].Value = DBNull.Value;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) type.Content = dr["type"];
                    else type.Content = "---";
                    while (dr.Read()) type.Content += ", " + dr["type"];
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("usp_SubtypeOfCardSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@card", SqlDbType.Int);
                    cmd.Parameters.Add("@subtype", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@card"].Value = this.card_id;
                    cmd.Parameters["@subtype"].Value = DBNull.Value;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) subtype.Content = dr["subtype"];
                    while (dr.Read()) subtype.Content += ", " + dr["subtype"];
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("usp_CreatureSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@card", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@card"].Value = this.card_id;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        power.Content = dr["power"];
                        toughness.Content = dr["toughness"];
                    }
                    else
                    {
                        power.Content = "---";
                        toughness.Content = "---";
                    }
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("usp_EditionSelect", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // set up the parameters
                    cmd.Parameters.Add("@code", SqlDbType.VarChar, 255);

                    // set parameter values
                    cmd.Parameters["@code"].Value = this.editionKey;

                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    legality.Content = (dr["legality"] == null ? "---" : dr["legality"]);
                    dr.Close();
                }
                using (SqlCommand cmd = new SqlCommand("SELECT Magic.udf_avgCardPrice(@card)", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // set up the parameters
                    cmd.Parameters.Add("@card", SqlDbType.Int);

                    // set parameter values
                    cmd.Parameters["@card"].Value = card_id;

                    avg_price.Content = cmd.ExecuteScalar().ToString();
                }
            }
        }
    }
}
