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
    /// Логика взаимодействия для ChangesPg.xaml
    /// </summary>
    public partial class ChangesPg : Window
    {
        private ObservableCollection<Point> errPointsCollection;
        private ObservableCollection<Point> newPointsCollection;
        private ObservableCollection<Point> refPointsCollection;
        private Dictionary<Point, int> refPointDic;
        private ObservableCollection<Point> delPointsCollection;

        public ChangesPg(
            List<Point> errPoints,
            List<Point> newPoints,
            //List<Point> refPoints, 
            Dictionary<Point, int> refDicPoints,
            List<Point> delPoints)
        {
            InitializeComponent();

            errPointsCollection = new ObservableCollection<Point>(errPoints);
            newPointsCollection = new ObservableCollection<Point>(newPoints);
            delPointsCollection = new ObservableCollection<Point>(delPoints);

            refPointDic = refDicPoints;
            List<Point> refList = new List<Point>() { };
            foreach (var point in refDicPoints.Keys)
            {
                refList.Add(point);
            }
            refPointsCollection = new ObservableCollection<Point>(refList);


            LbErrPoints.ItemsSource = errPointsCollection;
            LbNewPoints.ItemsSource = newPointsCollection;
            LbRefPoints.ItemsSource = refPointsCollection;
            LbDelPoints.ItemsSource = delPointsCollection;
        }

        private void ChangesPg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var a = MessageBox.Show("Вы уверены что хотите закрыть окно изменений? Вернуться к нему на данном импорте будет невозможно", "Закрыть", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (a == MessageBoxResult.Yes)

                e.Cancel = false;
            else
                e.Cancel = true;

        }

        private void EbAdd_Click(object sender, RoutedEventArgs e)
        {
            if (EcbOrderNumber.Text == "")
            {
                MessageBox.Show("Неверный номер заказа");
                return;
            }

            if (EcbCity.Text == "")
            {
                MessageBox.Show("Неверный город");
                return;
            }
            if (EcbStreet.Text == "")
            {
                MessageBox.Show("Неверная улица");
                return;
            }
            if (EcbHouse.Text == "")
            {
                MessageBox.Show("Неверный дом");
                return;
            }
            if ((EcbInt.Text == "с 9:00 до 21:00") ||
                (EcbInt.Text == "с 9:00 до 12:00") ||
                (EcbInt.Text == "с 12:00 до 15:00") ||
                (EcbInt.Text == "с 15:00 до 18:00") ||
                (EcbInt.Text == "с 18:00 до 21:00"))
            {
                try
                {
                    var selPoint = LbErrPoints.SelectedItem;
                    SessionInterface._errorPoints.Add((Point)selPoint);
                    errPointsCollection.Remove((Point)selPoint);
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

        private void NbAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selPoint = LbNewPoints.SelectedItem;
                SessionInterface._newPoints.Add((Point)selPoint);
                newPointsCollection.Remove((Point)selPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void RbRef_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selPoint = LbRefPoints.SelectedItem;
                refPointDic.TryGetValue((Point)selPoint, out int id);
                SessionInterface._updateDic.Add((Point)selPoint, id);
                refPointsCollection.Remove((Point)selPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void DbDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selPoint = LbDelPoints.SelectedItem;
                SessionInterface._delPoints.Add((Point)selPoint);
                delPointsCollection.Remove((Point)selPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
