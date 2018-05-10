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
    public partial class MainWindow : Window
    {
        DBWork db;
        private string username = "";
        public MainWindow()
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
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            username = value;
        }

        private void CheckPass_Click(object sender, RoutedEventArgs e)
        {
            string password = Pass.Password;
            Security secur = new Security();
            var isPassOk = secur.tryLogin(username, password);

            if (!isPassOk)
            {
                MessageBox.Show("Неверно введен пароль. Проверьте язык и CAPS LOCK.");
            }
            else
            {
                if (secur.IsUserAdmin(username))
                    new AdminInterface().Show();
                else
                    new SelectSession(username).Show();

                this.Close();
            }

            var sessions = db.getSessionByName(username);
            foreach (var ses in sessions)
            {
                if (ses.ArchTime <= DateTime.Today)
                    db.ArchiveSession(ses.ID_Session, true);
            }
            
        }

        


        
            
    }
}
