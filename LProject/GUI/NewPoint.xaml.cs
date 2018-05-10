using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace LProject
{

    public partial class NewPoint : Window
    {
        DBWork db;
        int pointNumber;
        int wayVal;
        string interVal;
        int sessionID;
        int pointType;


        private SessionInterface sesinter;

        public NewPoint(SessionInterface sesinter, int sessionId) : this()
        {
            sessionID = sessionId;
            this.sesinter = sesinter;
            pointType = 1;
        }

        public NewPoint()
        {
            db = new DBWork();
            InitializeComponent();
        }
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void AddPointClick(object sender, RoutedEventArgs e)
        {
            string korp = "0";
            if (City.Text == null || Street.Text == "" || House.Text == "")
                System.Windows.MessageBox.Show("Не все обязательные значения введены");
            else
            {
                pointNumber = (int)wayNumber.Value;
                if (pointNumber < db.FindLastPoint(wayVal + 1, interVal) + 1)
                    db.Shift(wayVal + 1, interVal, pointNumber);
                if (Corpes.Text != "")
                    korp = Corpes.Text;
                var orderNumber = Order.Text;
                var house = House.Text;

                var city = City.Text;
                var street = Street.Text;
                var interval = interVal;
                var number = pointNumber;

                db.InsertPoint(orderNumber, city, street, house, korp, interval, number, choosePoint(), sessionID, wayVal);

                DataChangedEventHandler handler = DataChanged;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }

                this.Close();
            }
        }

        private void wayComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;
            var routes = db.GetRoutsBySession(sessionID);
            List<int> routeValues = new List<int>();
            foreach (var route in routes)                
               routeValues.Add(route.RouteNumber);
            comboBox.ItemsSource = routeValues;
            comboBox.SelectedIndex = 0;
        }
        
        private void wayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //db = new DBWork();
            var comboBox = sender as ComboBox;
           
            wayVal = (int)comboBox.SelectedItem;
            wayNumber.Value = db.FindLastPoint(wayVal+1, interVal) + 1;
        }

        private void intervalComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;

            List<string> intervalValues = new List<string>()
                { "с 9:00 до 12:00", "с 12:00 до 15:00", "с 15:00 до 18:00", "с 18:00 до 21:00", "с 9:00 до 21:00" };
            comboBox.ItemsSource = intervalValues;
            comboBox.SelectedIndex = 0;
        }
        private void intervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            interVal = comboBox.SelectedItem as string;
            wayNumber.Value = db.FindLastPoint(wayVal + 1, interVal) + 1;
        }


        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public int choosePoint()
        {
            if (rb1.IsChecked == true)
            {
                pointType = 1;
            }
            else if (rb2.IsChecked == true)
            {
                pointType = 2;
            }
            else if (rb3.IsChecked == true)
            {
                pointType = 3;
            }
            else if (rb4.IsChecked == true)
            {
                pointType = 4;
            }
            else if (rb5.IsChecked == true)
            {
                pointType = 5;
            }
            else if (rb6.IsChecked == true)
            {
                pointType = 6;
            }
            else if (rb7.IsChecked == true)
            {
                pointType = 7;
            }
            else if (rb8.IsChecked == true)
            {
                pointType = 8;
            }
            else if (rb9.IsChecked == true)
            {
                pointType = 9;
            }
            else if (rb10.IsChecked == true)
            {
                pointType = 10;
            }

            return pointType;

        }

        

    }
}