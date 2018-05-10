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
    /// Interaction logic for ChangeUser.xaml
    /// </summary>
    
    public partial class ChangeUser : Window
    {
        DBWork db;
        string user;
        public ChangeUser(string user)
        {
            db = new DBWork();
            this.user = user;

            
            InitializeComponent();
            Name_text.Text = user;
        }
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Delete_click(object sender, RoutedEventArgs e)
        {
            System.Console.Write(user);
            db.DeleteUser(user);

            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }


            this.Close();
        }
        private void Save_click(object sender, RoutedEventArgs e)
        {
            if (user != Name_text.Text)
                db.saveUserNameChanges(user, Name_text.Text);
            if (Pass.Password != String.Empty && Name_text.Text.Length >= 5)
                db.saveUserPassChanges(user, Pass.Password);
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            this.Close();

        }

    }
}
