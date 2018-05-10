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
using System.Collections.ObjectModel;

namespace LProject.GUI
{
    /// <summary>
    /// Логика взаимодействия для ErrorList.xaml
    /// </summary>
    public partial class ErrorList : Window
    {
        private ObservableCollection<ErrorPoint> errPointsCollection;

        public ErrorList(List<ErrorPoint> errorPoints)
        {
            InitializeComponent();

            errPointsCollection = new ObservableCollection<ErrorPoint>(errorPoints);

            LbErrorPoints.ItemsSource = errPointsCollection;

            
        }

        private void LbErrorPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var IS = LbErrorPoints.ItemsSource;
            
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbOrderNumber.Text == "")
            {
                MessageBox.Show("Неверный номер заказа");
                return;
            }
            
            if (cbCity.Text == "")
            {
                MessageBox.Show("Неверный город");
                return;
            }
            if (cbStreet.Text == "")
            {
                MessageBox.Show("Неверная улица");
                return;
            }
            if (cbHouse.Text == "")
            {
                MessageBox.Show("Неверный дом");
                return;
            }
            if ((cbInt.Text == "с 9:00 до 21:00") ||
                (cbInt.Text == "с 9:00 до 12:00") ||
                (cbInt.Text == "с 12:00 до 15:00") ||
                (cbInt.Text == "с 15:00 до 18:00") ||
                (cbInt.Text == "с 18:00 до 21:00"))
            {
                try
                {
                    var selPoint = LbErrorPoints.SelectedItem;
                    SessionInterface._errorPoints.Add((ErrorPoint)selPoint);
                    errPointsCollection.Remove((ErrorPoint)selPoint);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Ошибка");
                }
                
            }
            else
            {
                MessageBox.Show("Неверный интервал. Примеры: с  9:00 до 21:00;с 9:00 до 12:00;с 12:00 до 15:00;с 15:00 до 18:00;с 18:00 до 21:00");
                return;
            }
        }

        private void ErrorListForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var a = MessageBox.Show("Вы уверены что хотите закрыть окно ошибочных точек? Вернуться к нему на данном импорте будет невозможно", "Закрыть", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if ( a == MessageBoxResult.Yes)
                
                e.Cancel = false;
            else
                e.Cancel = true;

        }
    }
}
