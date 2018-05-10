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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class SelectSession : Window
    {

        DBWork db;
        string userName;
        public SelectSession(string user)
        {
            userName = user;
            InitializeComponent();
            LoadSessionsList();
        }

        private void LoadSessionsList()
        {
            db = new DBWork();
            List<Session> panel = new List<Session>();

            foreach (var ses in db.getSessionByName(userName))
            {
                if (!ses.IsArchived)
                    panel.Add(new Session() {
                        Session_Name = ses.Session_Name,
                        ID_Session = ses.ID_Session,
                        IsArchived = ses.IsArchived
                    });
            }

            Session.ItemsSource = panel;
        }

        public void OpenSession(object sender, MouseButtonEventArgs e)
        {
            var session = (Session)((ListViewItem)sender).Content;
            new SessionInterface(session.ID_Session, session.IsArchived).Show();
        }

        private void DeleteSession_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            db = new DBWork();
            db.deleteSession(((Session)btn.DataContext).ID_Session);

            List<Session> panel = new List<Session>();

            foreach (var ses in db.getSessionByName(userName))
            {
                if (!ses.IsArchived)
                    panel.Add(new Session() { Session_Name = ses.Session_Name, ID_Session = ses.ID_Session, IsArchived = ses.IsArchived });
            }

            Session.ItemsSource = panel;
        }
        private void CreateNew_click(object sender, RoutedEventArgs e)
        {
            CreateSession newSession = new CreateSession();
            newSession.Show();
            newSession.DataChanged += CreateSession_DataChanged;
        }
        private void CreateSession_DataChanged(object sender, EventArgs e)
        {
            List<Session> panel = new List<Session>();

            var names = db.getNames();
            
            foreach (var ses in db.getSessionByName(userName))
            {
                if (!ses.IsArchived)
                    panel.Add(new Session() { Session_Name = ses.Session_Name, ID_Session = ses.ID_Session, IsArchived = ses.IsArchived });
            }

            Session.ItemsSource = panel;
        }

        private void Wnd_Activated(object sender, EventArgs e)
        {
            LoadSessionsList();
        }
    }
}
