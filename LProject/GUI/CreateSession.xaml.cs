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


    public partial class CreateSession : Window
    {
        DBWork db;
        String manager;
        String session;
        int dayNumber;
        bool doArchive;

        public CreateSession()
        {
            db = new DBWork();
            InitializeComponent();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;

            var names = db.getNames();
            comboBox.ItemsSource = names;
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            string value = comboBox.SelectedItem as string;
            manager = value;
        }
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            this.Close();         
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            doArchive = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            doArchive = false;
        }

        private void Create_click(object sender, RoutedEventArgs e)
        {
            dayNumber = (int)DayNumber.Value;
            if (Name_text.Text != null)
            {
                session = Name_text.Text;
                if (manager != null)
                    if (doArchive)
                        db.InsertSession(session, manager, dayNumber);
                    else
                        db.InsertSession(session, manager);
            }
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                 handler(this, new EventArgs());
            }
            this.Close();
        }
        
    }

   
}
