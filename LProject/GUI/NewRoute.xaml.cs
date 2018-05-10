using System;
using System.Windows;
using System.Drawing;
using System.Collections.Generic;

namespace LProject
{
    public partial class NewRoute : Window
    {
        DBWork db;
        int number;
        string hex;
        List<int> RouteNumbers;

        int sessionId;
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;
        public NewRoute(int sId, List<int> routeNumbers)
        {
            sessionId = sId;
            RouteNumbers = routeNumbers;
            db = new DBWork();
            InitializeComponent();
        }
        private void ColorCanvas1_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            var mColor = ColorCanvas1.SelectedColor.Value;
            hex = ColorTranslator.ToHtml(Color.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B));
        }
        private void AddPointClick(object sender, RoutedEventArgs e)
        {
            string message;
            string caption;

            number = (int)RouteNumber.Value;
            if (String.IsNullOrEmpty(hex))
            {
                message = "Выберите цвет!";
                caption = "Ошибка при заполнении";

                MessageBox.Show(message, caption, MessageBoxButton.OK);
            }

            else
            {
                if (RouteNumbers.Contains(number))
                    MessageBox.Show("Маршрут с таким номером уже существует");
                else
                {
                    db.InsertRoute(number, hex, sessionId);

                    DataChangedEventHandler handler = DataChanged;

                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }

                    this.Close();
                }
                
            }

            
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

}
