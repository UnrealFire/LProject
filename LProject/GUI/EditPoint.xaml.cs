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
using System.Text.RegularExpressions;

namespace LProject
{
    /// <summary>
    /// Interaction logic for NewPoint.xaml
    /// </summary>
    public partial class EditPoint : Window
    {
        DBWork db;

        int wayVal;
        string interVal;
        string addr_street;
        string addr_house;
        string addr_korp;
        string order;
        int pointRN;

        Point point;
        int sessionID;


        public EditPoint(Point point, int sessionId)
        {
            this.point = point;
            sessionID = sessionId;
            db = new DBWork();

            InitializeComponent();

            addr_street = point.Street;
            addr_house = point.House;
            addr_korp = point.Korp;
            order = point.OrderNumber;
            pointRN = point.PointType;


            Comment.Text = point.Comment; 
            wayNumber.Value = point.PointType;
            tB_Addr_street.Text = addr_street;
            tB_Addr_house.Text = addr_house;
            tB_Addr_korp.Text = addr_korp;
            tB_order.Text = order;
        }
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            var route = wayVal;
            var interval = interVal;
            var number = 0; //заглушка?
            string comment = Comment.Text;

            addr_street = tB_Addr_street.Text;
            addr_house = tB_Addr_house.Text;
            addr_korp = tB_Addr_korp.Text;
            order = tB_order.Text;

            if ((addr_street == "") || (addr_house == "") || (order == ""))
                MessageBox.Show("Введите улицу/дом/номер заказа");

            //Если номер точки изменился используем метод перестроения ряда
            if (wayNumber.Value != pointRN & wayNumber.Value > 0)
            {
                try
                {
                    //Вызов метода сдвига точек при изменении номера
                    pointRN = GUI.PointRN.NumberMove((int)wayNumber.Value, pointRN, db.GetRoutsBySession(sessionID), db.getPointBySession(sessionID), wayVal);
                }
                catch (Exception)
                {
                    MessageBox.Show("Неверный ввод номера точки в маршруте");
                    return;
                }
            }


            db.ChangePoint(point.ID_Point, route, interval, number, pointRN, sessionID, comment, addr_street, addr_house,addr_korp, order);
            GUI.PointRN.SpaceChecking(db.GetRoutsBySession(sessionID), db.getPointBySession(sessionID), wayVal);
            var Data = db.getPointBySession(sessionID);
            db.ChangePoint(point.ID_Point, route, interval, number, pointRN, sessionID, comment, addr_street, addr_house, addr_korp, order);

            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            this.Close();
        }

        private void wayComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;
            var routes = db.GetRoutsBySession(sessionID);
            int selectedItem = 000;
            List<int> routeValues = new List<int>();
            foreach (var route in routes)
            {
                if (route.ID_Route == point.ID_Route)
                    selectedItem = route.RouteNumber;
                routeValues.Add(route.RouteNumber);
            }
            comboBox.ItemsSource = routeValues;
            comboBox.SelectedItem = selectedItem;
        }

        private void wayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            //int value = (int)comboBox.SelectedItem;
            wayVal = (int)comboBox.SelectedItem;
            //wayNumber.Value = db.FindLastPoint(wayVal + 1, interVal) + 1;
        }

        private void intervalComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;
            int selectedItem = 000;
            
            List<string> intervalValues = new List<string>()
                { "с 9:00 до 12:00", "с 12:00 до 15:00", "с 15:00 до 18:00", "с 18:00 до 21:00", "с 9:00 до 21:00" };

            comboBox.ItemsSource = intervalValues;

            Regex r = new Regex(@"\s{2,}");
            point.Interval = r.Replace(point.Interval, " ");

            switch (point.Interval)
            {
                case "с 9:00 до 12:00":
                    selectedItem = 0;
                    break;
                case "с 12:00 до 15:00":
                    selectedItem = 1;
                    break;
                case "с 15:00 до 18:00":
                    selectedItem = 2;
                    break;
                case "с 18:00 до 21:00":
                    selectedItem = 3;
                    break;
                case "с 9:00 до 21:00":
                    selectedItem = 4;
                    break;
            }
            comboBox.SelectedIndex = selectedItem;
        }

        private void intervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            interVal = comboBox.SelectedItem as string;
            //wayNumber.Value = db.FindLastPoint(wayVal + 1, interVal) + 1;
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            GUI.PointRN.DeleteNumber(point.PointType, db.GetRoutsBySession(sessionID), db.getPointBySession(sessionID), wayVal);
            db.deletePoint(point.ID_Point);

            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            this.Close();
        }

        

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch
            { }
        }

        

    }
       
}

