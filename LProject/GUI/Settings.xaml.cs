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
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        DBWork db;
        public Settings()
        {
            db = new DBWork();
            InitializeComponent();
            List<Man> items = new List<Man>();
            var names = db.getNames();
            foreach (var name in names)
                items.Add(new Man() { PersonName = name });
            peopleNames.ItemsSource = items;
        }
        public void ChangeUserProp(object sender, MouseButtonEventArgs e)
        {
            var user = ((ListViewItem)sender).Content;
            string userName = ((Man)user).PersonName;
            ChangeUser change = new ChangeUser(userName);
            change.Show();
            change.DataChanged += user_DataChanged;
        }

        private void CreateNew_click(object sender, RoutedEventArgs e)
        {
            NewUser user = new NewUser();
            user.Show();
            user.DataChanged += user_DataChanged;
        }
        private void user_DataChanged(object sender, EventArgs e)
        {
            List<Man> items = new List<Man>();
            var names = db.getNames();
            foreach (var name in names)
                items.Add(new Man() { PersonName = name });
            peopleNames.ItemsSource = items;
        }
    }
}