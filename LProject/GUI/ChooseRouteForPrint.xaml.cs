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


namespace LProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChooseRouteForPrint : Window
    {
        DBWork db;

        int sesID;
        int RouteValue;

        public ChooseRouteForPrint(int sessionId)
        {
            sesID = sessionId;
            db = new DBWork();
            InitializeComponent();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            var routes = db.GetRoutsBySession(sesID);
            List<int> routeValues = new List<int>();
            foreach (var route in routes)
                routeValues.Add(route.RouteNumber);
            comboBox.ItemsSource = routeValues;
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            RouteValue = (int)comboBox.SelectedItem;
        }        

        private void further_Click(object sender, RoutedEventArgs e)
        {
            PrintRoute print = new PrintRoute(sesID, RouteValue);
            //для проверки
            print.Show();

            this.Close();            
        }       
            
    }
}
