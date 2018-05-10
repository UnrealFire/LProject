using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;

namespace LProject
{
    /// <summary>
    /// Interaction logic for ChangeRoute.xaml
    /// </summary>
    public partial class ChangeRoute : Window
    {
        DBWork db;
        int number;
        string hex;
        Route route;

        public ChangeRoute(Route route)
        {
            this.route = route;
            db = new DBWork();
            hex = route.Color;
            InitializeComponent();
            RouteNumber.Value = route.RouteNumber;
        }

        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;
        private void ColorCanvas1_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            var mColor = ColorCanvas1.SelectedColor.Value;
            hex = ColorTranslator.ToHtml(Color.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B));
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            
            number = (int)RouteNumber.Value;
            
            db.ChangeRoute(route.ID_Route, number, hex);
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            this.Close();

        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
