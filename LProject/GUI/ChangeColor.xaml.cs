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
    public partial class ChangeColor : Window
    {
        DBWork db;
        int number;
        string hex;

        public ChangeColor()
        {
            db = new DBWork();
            InitializeComponent();
        }
        private void GetNumber(object sender, RoutedEventArgs e)
        {
            number = (int)Number.Value;
        }
        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            hex = new ColorConverter().ConvertToString(ChooseColor.SelectedColor);
        }
        private void AddPointClick(object sender, RoutedEventArgs e)
        {
            db.InsertColor(number, hex);
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

}
