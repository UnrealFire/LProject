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

namespace LProject
{
    public partial class ChangePoint : Window
    {
        DBWork db;
        int num;
        string wayVal;
        string interVal;

        public ChangePoint()
        {
            db = new DBWork();
            InitializeComponent();
        }

        private void AddPointClick(object sender, RoutedEventArgs e)
        {
            string message;
            string caption;

            var interval = interVal;
            var number = num;
            var type = 0;
            var id = 0;
            if(String.IsNullOrEmpty(wayVal)!=false)
                id = Convert.ToInt32(wayVal);
            
                db.ChangePoint(interval, number, type, id);
                this.Close();

        }
        private void wayComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;

           List<string> journeyValues = new List<string>(){"First", "Second", "Third"} ;
            comboBox.ItemsSource = journeyValues;
            comboBox.SelectedIndex = 0;
        }

        private void wayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            string value = comboBox.SelectedItem as string;
            wayVal =  value;
        }

        private void intervalComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;

            List<string> intervalValues = new List<string>() { "10-12", "12-14", "14-16" };
            comboBox.ItemsSource = intervalValues;
            comboBox.SelectedIndex = 0;
        }
        private void intervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            string value = comboBox.SelectedItem as string;
            interVal = value;
        }


        private void ChooseNumber(object sender, RoutedEventArgs e)
        {
            num = (int)wayNumber.Value;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
