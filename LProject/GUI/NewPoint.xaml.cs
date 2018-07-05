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
        int pointRN; //номер в маршруте
        int wayVal; //маршрут
        string interVal; //интервал
        int sessionID; //сессия


        private SessionInterface sesinter;

        public NewPoint(SessionInterface sesinter, int sessionId) : this()
        {
            sessionID = sessionId;
            this.sesinter = sesinter;
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
                //pointRN = (int)wayNumber.Value; // получаем введенное значение
                //if (pointNumber < db.FindLastPoint(wayVal + 1, interVal) + 1)
                //    db.Shift(wayVal + 1, interVal, pointNumber);
                if (Corpes.Text != "")
                    korp = Corpes.Text;
                var orderNumber = Order.Text;
                var house = House.Text;

                var city = City.Text;
                var street = Street.Text;
                var interval = interVal;
                var number = 0; //заглушка?
                //Если номер больше 0, то вызываем метод сдвига
                if (wayNumber.Value > 0)
                {
                    //Метод возвращает (утверждает) номер точки, осуществляя по необходимости сдвиг
                    pointRN = GUI.PointRN.NumberInsert((int)wayNumber.Value, db.GetRoutsBySession(sessionID), db.getPointBySession(sessionID), wayVal);
                }
                //если равен или меньше то ставим в конец
                else
                {
                    DBWork db = new DBWork();
                    pointRN = GUI.PointRN.FirstNumber(db.GetRoutsBySession(sessionID), db.getPointBySession(sessionID), wayVal);
                }
                

                db.InsertPoint(orderNumber, city, street, house, korp, interval, number, pointRN, sessionID, wayVal);

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
            wayNumber.Value = 0;
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
        }


        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}