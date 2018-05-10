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
    /// Interaction logic for GetDBName.xaml
    /// </summary>
    public partial class GetDBName : Window
    {
        public string Name;

        public GetDBName()
        {
            Name = "";
            InitializeComponent();
        }
        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;
        private void OkClick(object sender, RoutedEventArgs e)
        {
            Name = DBName.Text;
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
            this.Close();
        }
    }
}
