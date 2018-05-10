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
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        DBWork db;
        public NewUser()
        {
            db = new DBWork();
            InitializeComponent();
        }

        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void Create_click(object sender, RoutedEventArgs e)
        {
            //Строка не меньше 3 знаков
            var pass = Pass.Password;
            var userName = Name_text.Text;
            if (userName.Length < 5)
                MessageBox.Show("Имя пользователя должно быть не меньше 5 знаков");
            else
            {
                var salt = userName.Substring(3);
                Security secur = new Security();
                string hash = secur.HashPassword(pass, salt);
                System.Console.Write(hash);
                db.InsertUser(userName, hash);
                //БД вызов на добавление
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

                this.Close();
            }
        }
    }
}
