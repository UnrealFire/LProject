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
    /// Interaction logic for AdminInterface.xaml
    /// </summary>
    public partial class AdminInterface : Window
    {

        DBWork db;
        public AdminInterface()
       { 
            db = new DBWork();
            InitializeComponent();
            List<SessionPanel> panel = new List<SessionPanel>();
            List<SessionPanel> panelacrh = new List<SessionPanel>();
            var names = db.getNames();
       

            foreach (var name in names)
            {
                foreach (var ses in db.getSessionByName(name))
                {
                    if (ses.IsArchived)
                        panelacrh.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session, Archieved =ses.IsArchived });
                    else
                        panel.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session, Archieved = ses.IsArchived });
                }
            }

            active.ItemsSource = panel;
            archived.ItemsSource = panelacrh;
        }

        public void OpenSession(object sender, MouseButtonEventArgs e)
        {
            var user = ((ListViewItem)sender).Content;
            int ID = ((SessionPanel)user).Session_ID;
            bool Arch = ((SessionPanel)user).Archieved;
            
            
            if (Arch)
            {
                SessionInterface archSession = new SessionInterface(ID, true);
                archSession.Show();
                archSession.DataChanged += CreateSession_DataChanged;
            }
            else
            {
                SessionInterface session = new SessionInterface(ID);
                session.Show();
                session.DataChanged += CreateSession_DataChanged;
            }
        }

        private void CreateNew_click(object sender, RoutedEventArgs e)
        {
            CreateSession newSession = new CreateSession();
            newSession.Show();
            newSession.DataChanged += CreateSession_DataChanged;  
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            db.deleteSession(((SessionPanel)btn.DataContext).Session_ID);

            List<SessionPanel> panel = new List<SessionPanel>();
            List<SessionPanel> panelacrh = new List<SessionPanel>();

            var names = db.getNames();

            foreach (var name in names)
            {
                foreach (var ses in db.getSessionByName(name))
                {
                    if (ses.IsArchived)
                        panelacrh.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session });
                    else
                        panel.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session });
                }
            }

            active.ItemsSource = panel;
            archived.ItemsSource = panelacrh;
        }

        private void CreateSession_DataChanged(object sender, EventArgs e)
        {
            List<SessionPanel> panel = new List<SessionPanel>();
            List<SessionPanel> panelacrh = new List<SessionPanel>();
            db = new DBWork();

            active.ItemsSource = null;
            archived.ItemsSource = null;

            var names = db.getNames();


            foreach (var name in names)
            {
                var sessions = db.getSessionByName(name);
                foreach (var ses in sessions)
                {
                    if (ses.IsArchived)
                        panelacrh.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session });
                    else
                        panel.Add(new SessionPanel() { Manager = name, Sessions = ses.Session_Name, Session_ID = ses.ID_Session });
                }
            }

            active.ItemsSource = panel;
            archived.ItemsSource = panelacrh;
        }


        private void Settings_click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
    }

    public class SessionPanel
    {
        public string Manager
        {
            get;
            set;
        }
        public string Sessions { get; set; }
        public int Session_ID { get; set; }
        public bool Archieved { get; set; }
        public bool ObjectTrackingEnabled { get; set; }

    }

}
