using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Globalization;

using Gecko;
using System.Windows.Forms.Integration;

using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Win32;

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Drawing;
using Gecko.DOM;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Printing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;


namespace LProject
{
    public class ErrorPoint
    {
        public string OrderNumber{get;set;}
        public string Street { get; set; }
        public string City { get; set; }
        public string House { get; set; }
        public string Korp { get; set; }
        public string Interval { get; set; }
        public string PointNumber { get; set; }
        public string PointType { get; set; }
        public string ID_Session { get; set; }
        public string ID_Route { get; set; }
    }

    public class DelPoint
    {
        public string OrderNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string House { get; set; }
        public string Korp { get; set; }
        public string Interval { get; set; }
        public string PointNumber { get; set; }
        public string PointType { get; set; }
        public string ID_Session { get; set; }
        public string ID_Route { get; set; }
    }

    public class ExistPoint
    {
        public string OrderNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string House { get; set; }
        public string Korp { get; set; }
        public string Interval { get; set; }
        public string PointNumber { get; set; }
        public string PointType { get; set; }
        public string ID_Session { get; set; }
        public string ID_Route { get; set; }
    }

    public class newRoute
    {
        public string editRouteImage { get; set; }
        public string hideElemImage { get; set; }
        public string deleteElemImage { get; set; }

        
        public int routeNumber { get; set; }
        public string routeText { get; set; }
        public string colorRoute { get; set; }
        public string colorRouteStroke { get; set; }

        public List<NewPoints> point1 { get; set; }
        public List<NewPoints> point2 { get; set; }
        public List<NewPoints> point3 { get; set; }
        public List<NewPoints> point4 { get; set; }
        public List<NewPoints> point5 { get; set; }

        public List<NewPoints> GetAllPoints()
        {
            var allPoints = new List<NewPoints>(point1.Count +
                                                point2.Count +
                                                point3.Count +
                                                point4.Count +
                                                point5.Count);

            allPoints.AddRange(point1);
            allPoints.AddRange(point2);
            allPoints.AddRange(point3);
            allPoints.AddRange(point4);
            allPoints.AddRange(point5);

            return allPoints;
        }

        public Route GUIroute { get; set; }

        public string RouteColor { get; set; }
    }

    public class NewPoints
    {
        public int pointNumber { get; set; }
        public string pointText { get; set; }
        public string Address { get; set; }
        public string Order { get; set; }
        public int pointId { get; set; }
        public string comments { get; set; }
        public Interval PointInterval { get; set; }
        public int NumInInterval { get; set; }
    }

    public partial class SessionInterface : Window
    {
        DBWork db;
        GeckoWebBrowser browser;
        int sessionId;
        string URL = "file:///" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");




        string address;
        XSSFWorkbook wb;
        XSSFSheet sh;

        ObservableCollection<newRoute> Routes;
        ObservableCollection<comboBox_Print_Route> ComboBoxRoutes_Colection { get; set; }


        string city, street, house, korp, interval, order = "";
        string[] boundsStrArray = new string[4];

        bool firstImport = true;



        public SessionInterface(int ID, bool isArchived = false)
        {
            InitializeComponent();

            SetArchivedGUIState(isArchived);

            sessionId = ID;
            db = new DBWork();

            db.getPointBySession(sessionId);

            InitializeMap();

            //Получение информации о точках из бд, сортировка их в маршруты и вывод в список
            if (!isArchived)
                setRoutes();

        }

        private void SetArchivedGUIState(bool isArchived)
        {
            Print.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Import.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Export.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Add_route.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Add_point.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Archive.Visibility = isArchived ? Visibility.Hidden : Visibility.Visible;
            Unarchive.Visibility = !isArchived ? Visibility.Hidden : Visibility.Visible;
        }

        void InitializeMap()
        {
            try
            {
                browser.Stop();
                browser.Dispose();
                browser = null;
            }
            catch
            { }
            Gecko.Xpcom.Initialize("Firefox");
            WindowsFormsHost host = new WindowsFormsHost();
            browser = new GeckoWebBrowser();
            setRoutes();
            browser.DocumentCompleted += (sender, e) => { DrawRoutesOnMap(); };
            browser.ConsoleMessage += (object sender, ConsoleMessageEventArgs e) => Console.WriteLine(e.Message);

            host.Child = browser;
            GridWeb.Children.Clear();
            GridWeb.Children.Add(host);
            browser.Navigate(URL);

            //System.Windows.Point position = GridWeb.PointToScreen(new System.Windows.Point(0, 0));
            //double screenWidth = GridWeb.ActualWidth;
            //double screenHeight = GridWeb.ActualHeight;

        }

        public static string constractAddress(string street, string house, string korp)
        {
            return String.Format("ул. {0}, {1} {2}", street, house, (korp == "") || (korp == "0") ? " к" + korp : "");
        }

        #region comm
        //public void setRoutes()
        //{
        //    db = new DBWork();
        //    //Коллекция маршруторв
        //    Routes = new ObservableCollection<newRoute> { };
        //    ComboBoxRoutes_Colection = new ObservableCollection<comboBox_Print_Route>() { };


        //    //получение точек из бд
        //    var Data = db.getPointBySession(sessionId);
        //    //получение списка маршрутов из бд
        //    var dataRoutes = db.GetRoutsBySession(sessionId);

        //    var sizeDataRoutes = dataRoutes.Count;
        //    var sizeData = Data.Count;

        //    var dbRoutes = db.GetRoutsBySession(sessionId);

        //    foreach (var item in dbRoutes)
        //    {
        //        //Словарь интервалов, создание
        //        Dictionary<Interval, List<NewPoints>> intervals = new Dictionary<Interval, List<NewPoints>>();
        //        intervals.Add(new Interval(9, 0, 12, 0), new List<NewPoints>());
        //        intervals.Add(new Interval(12, 0, 15, 0), new List<NewPoints>());
        //        intervals.Add(new Interval(15, 0, 18, 0), new List<NewPoints>());
        //        intervals.Add(new Interval(18, 0, 21, 0), new List<NewPoints>());
        //        intervals.Add(new Interval(9, 0, 21, 0), new List<NewPoints>());

        //        //Заполнение словаря, сортировка точек
        //        for (var i = 0; i < sizeData; i++)
        //        {
        //            var elem = Data[i];
        //            var interval = new Interval(elem.Interval);

        //            //Сортировка точек по маршрутам
        //            if (elem.ID_Route == item.ID_Route)
        //            {
        //                //Создание строки адреса
        //                string address = constractAddress(elem.Street, elem.House, elem.Korp);

        //                //Сортировка в словарь интервалов
        //                foreach (var kv in intervals)
        //                {


        //                    if (kv.Key == interval)
        //                    {


        //                        kv.Value.Add(new NewPoints
        //                        {
        //                            //ID точки
        //                            pointId = elem.ID_Point,
        //                            //Текст точки (номер + адрес)
        //                            pointText = ": " + elem.OrderNumber + " " + address,
        //                            //Тип(?) точки
        //                            PointType = elem.PointType,
        //                            //Адрес (город + адерс)
        //                            Address = elem.City + ", " + address,
        //                            //Комментарий
        //                            comments = elem.Comment
        //                        });
        //                    }


        //                }

        //            }
        //        }

        //        foreach (var interval in intervals)
        //        {
        //            var numInInterval = 0;
        //            foreach (var point in interval.Value)
        //            {
        //                numInInterval++;
        //                point.pointText = point.pointText.Insert(0, Convert.ToString(numInInterval));
        //            }
        //        }

        //        Routes.Add(new newRoute
        //        {
        //            routeText = item.RouteNumber == 0 ? "Без маршрута" : "Маршрут " + item.RouteNumber,
        //            routeNumber = item.RouteNumber,
        //            colorRoute = "" + item.Color,
        //            colorRouteStroke = "black",
        //            point1 = intervals.ToList()[0].Value,
        //            point2 = intervals.ToList()[1].Value,
        //            point3 = intervals.ToList()[2].Value,
        //            point4 = intervals.ToList()[3].Value,
        //            point5 = intervals.ToList()[4].Value,
        //            GUIroute = item,
        //            RouteColor = item.Color
        //        });

        //    }
        //    routesList.ItemsSource = Routes;

        //    //Добавляем в ComboBox печати
        //    ComboBoxRoutes_Colection.Add(new comboBox_Print_Route
        //    {
        //        Name = "Все точки",
        //        Id = -1
        //    });
        //    foreach (var route in Routes)
        //    {

        //        ComboBoxRoutes_Colection.Add(new comboBox_Print_Route
        //        {
        //            Name = route.routeText,
        //            Id = route.routeNumber

        //        });
        //    }

        //    comboBox_Print.ItemsSource = ComboBoxRoutes_Colection;

        //}
        #endregion

        public void setRoutes()
        {
            db = new DBWork();
            //Коллекция маршруторв
            Routes = new ObservableCollection<newRoute> { };
            ComboBoxRoutes_Colection = new ObservableCollection<comboBox_Print_Route>() { };


            //получение точек из бд
            var Data = db.getPointBySession(sessionId);
            //получение списка маршрутов из бд
            var dataRoutes = db.GetRoutsBySession(sessionId);

            var sizeDataRoutes = dataRoutes.Count;
            var sizeData = Data.Count;

            var dbRoutes = db.GetRoutsBySession(sessionId);

            foreach (var item in dbRoutes)
            {
                //Словарь интервалов, создание
                Dictionary<Interval, List<NewPoints>> intervals = new Dictionary<Interval, List<NewPoints>>();
                intervals.Add(new Interval(9, 0, 12, 0), new List<NewPoints>());
                intervals.Add(new Interval(12, 0, 15, 0), new List<NewPoints>());
                intervals.Add(new Interval(15, 0, 18, 0), new List<NewPoints>());
                intervals.Add(new Interval(18, 0, 21, 0), new List<NewPoints>());
                intervals.Add(new Interval(9, 0, 21, 0), new List<NewPoints>());

                //Заполнение словаря, сортировка точек
                for (var i = 0; i < sizeData; i++)
                {
                    var elem = Data[i];
                    var interval = new Interval(elem.Interval);

                    //Сортировка точек по маршрутам
                    if (elem.ID_Route == item.ID_Route)
                    {
                        //Создание строки адреса
                        string address = constractAddress(elem.Street, elem.House, elem.Korp);

                        //Сортировка в словарь интервалов
                        foreach (var kv in intervals)
                        {


                            if (kv.Key == interval)
                            {


                                kv.Value.Add(new NewPoints
                                {
                                    //ID точки
                                    pointId = elem.ID_Point,
                                    //Номер точки в маршруте
                                    NumInInterval = elem.PointType,
                                    //Текст точки (номер + адрес)
                                    pointText = elem.PointType + ": " + elem.OrderNumber + " " + address,
                                    //Тип(?) точки
                                    Order = elem.OrderNumber,
                                    //Адрес (город + адерс)
                                    Address = elem.City + ", " + address,
                                    //Комментарий
                                    comments = elem.Comment,
                                    
                                    //Интервал
                                    PointInterval = interval


                                });
                            }


                        }

                    }
                }

                //foreach (var interval in intervals)
                //{
                //    var numInInterval = 0;
                //    foreach (var point in interval.Value)
                //    {
                //        numInInterval++;
                //        point.pointText = point.pointText.Insert(0, Convert.ToString(numInInterval));
                //        point.NumInInterval = numInInterval;
                //    }
                //}

                Routes.Add(new newRoute
                {
                    routeText = item.RouteNumber == 0 ? "Без маршрута" : "Маршрут " + item.RouteNumber,
                    routeNumber = item.RouteNumber,
                    colorRoute = "" + item.Color,
                    colorRouteStroke = "black",
                    point1 = intervals.ToList()[0].Value,
                    point2 = intervals.ToList()[1].Value,
                    point3 = intervals.ToList()[2].Value,
                    point4 = intervals.ToList()[3].Value,
                    point5 = intervals.ToList()[4].Value,
                    GUIroute = item,
                    RouteColor = item.Color
                });

            }
            routesList.ItemsSource = Routes;

            //Добавляем в ComboBox печати
            ComboBoxRoutes_Colection.Add(new comboBox_Print_Route
            {
                Name = "Все точки",
                Id = -1
            });
            foreach (var route in Routes)
            {

                ComboBoxRoutes_Colection.Add(new comboBox_Print_Route
                {
                    Name = route.routeText,
                    Id = route.routeNumber

                });
            }

            comboBox_Print.ItemsSource = ComboBoxRoutes_Colection;

        }



        public void DrawRoutesOnMap()
        {
            //Xpcom.GetService<nsIMemory>("@mozilla.org/xpcom/memory-service;1").HeapMinimize(true);

            var mapRoutes = Routes.Select(r =>
            {
                return new
                {
                    routeColor = r.RouteColor,
                    points = r.GetAllPoints().Select(p =>
                    {
                        return new
                        {
                            id = p.pointId,
                            address = p.Address,
                            interval = p.PointInterval.ToShortString(),
                            numInt = p.NumInInterval,
                            order = p.Order
                        };
                    })
                };
            });



            browser.AddMessageEventListener("centerChange", (string p) =>
            {
                boundsStrArray = p.Split(';');
                //for (int i = 0; i < 4; i++)
                //{
                //    bounds[i] = Convert.ToDouble(boundsStrArray[i]);
                //}                
            });

            //string result;
            browser.AddMessageEventListener("clickPointEvent", (string p) =>
            {
                int ID = Convert.ToInt32(p);
                Point point = db.GetPointById(ID);
                EditPoint edit = new EditPoint(point, sessionId);
                edit.Show();
                edit.DataChanged += UpdateAll;
            });

            var json = JsonConvert.SerializeObject(mapRoutes);
            var routes = browser.Document.GetElementById("routes");
            routes.SetAttribute("data-routes", json);

            var bounds_div = browser.Document.GetElementById("bounds_div");
            bounds_div.SetAttribute("left-x", boundsStrArray[0]);
            bounds_div.SetAttribute("left-y", boundsStrArray[1]);
            bounds_div.SetAttribute("right-x", boundsStrArray[2]);
            bounds_div.SetAttribute("right-y", boundsStrArray[3]);

            var draw = browser.Document.GetElementById("draw_routes").DomObject;
            var drawButton = new GeckoInputElement(draw);
            drawButton.Click();

            //using (AutoJSContext context = new AutoJSContext(browser.Window))
            //{
            //    var json = JsonConvert.SerializeObject(mapRoutes);
            //    var res = context.EvaluateScript("drawRoutes(" + json + ");", out result);
            //}

        }


        public delegate void DataChangedEventHandler(object sender, EventArgs e);

        public event DataChangedEventHandler DataChanged;

        private void Archive_click(object sender, RoutedEventArgs e)
        {
            db.ArchiveSession(sessionId, true);
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            Print.Visibility = Visibility.Hidden;
            Import.Visibility = Visibility.Hidden;
            Export.Visibility = Visibility.Hidden;
            Add_route.Visibility = Visibility.Hidden;
            Add_point.Visibility = Visibility.Hidden;
            Archive.Visibility = Visibility.Hidden;
            Unarchive.Visibility = Visibility.Visible;
        }

        private void Unarchive_click(object sender, RoutedEventArgs e)
        {
            db.ArchiveSession(sessionId, false);
            DataChangedEventHandler handler = DataChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }


            Print.Visibility = Visibility.Visible;
            Import.Visibility = Visibility.Visible;
            Export.Visibility = Visibility.Visible;
            Add_route.Visibility = Visibility.Visible;
            Add_point.Visibility = Visibility.Visible;
            Archive.Visibility = Visibility.Visible;
            Unarchive.Visibility = Visibility.Hidden;
        }

        private void Export_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save File";

            saveFileDialog1.DefaultExt = "xls";
            saveFileDialog1.Filter = "Microsoft Excel (*.xls)|*.xls";
            saveFileDialog1.FileName = DateTime.Now.ToString(new CultureInfo("ru-RU")).Replace('/', '_').Replace(' ', '_').Replace(':', '_').Replace('.', '_');
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == "")
            {
                return;
            }
            else
            {
                address = saveFileDialog1.FileName;
            }

            string[] top = { "Интернет-заказ", "Город", "Наименование улицы", "Дом", "Корпус", "Интервал доставки" };

            wb = new XSSFWorkbook();
            sh = (XSSFSheet)wb.CreateSheet("Лист 1");

            int countRow = 1;
            int countColumn = 6;

            //заголовок
            for (int i = 0; i < countRow; i++)
            {
                var currentRow = sh.CreateRow(i);

                for (int j = 0; j < countColumn; j++)
                {
                    var currentCell = currentRow.CreateCell(j);
                    currentCell.SetCellValue(top[j]);
                    sh.AutoSizeColumn(j);
                }

            }

            //... data
            var Data = db.getPointBySession(sessionId);
            var sizeData = Data.Count;

            for (var k = 0; k < sizeData; k++)
            {
                var elem = Data[k];
                var order = elem.OrderNumber;
                var city = elem.City;
                var street = elem.Street;
                var house = elem.House;
                var korp = elem.Korp;
                var interval = elem.Interval;
                string[] data = { order, city, street, house, korp, interval };

                var currentRow = sh.CreateRow(k + 1);
                for (int j = 0; j < countColumn; j++)
                {
                    var currentCell = currentRow.CreateCell(j);
                    currentCell.SetCellValue(data[j]);
                    sh.AutoSizeColumn(j);
                }
            }


            if (!File.Exists(address))
            {
                File.Delete(address);
            }

            using (var fs = new FileStream(address, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }

            MessageBox.Show("Экспорт завершен");

            //this.Close();
        }

        List<Point> errorList = new List<Point>() { };
        //List<Point> updateList = new List<Point>() { };
        Dictionary<Point, int> updateDic = new Dictionary<Point, int>() { };
        List<Point> newList = new List<Point>() { };
        List<Point> deleteList = new List<Point>() { };
        List<Point> verifdData = new List<Point>() { };

        public static List<Point> _errorPoints { get; set; } = new List<Point>() { };
        //public static List<Point> _updatePoints { get; set; } = new List<Point>() { };
        public static Dictionary<Point, int> _updateDic { get; set; } = new Dictionary<Point, int>() { };
        public static List<Point> _newPoints { get; set; } = new List<Point>() { };
        public static List<Point> _delPoints { get; set; } = new List<Point>() { };

        private void Refresh_click(object sender, RoutedEventArgs e)
        {
            

            #region base_import
            bool hasRoutes = db.GetRoutsBySession(sessionId).Count != 0;
            if (!hasRoutes)
            {
                db.InsertRoute(0, "#ff888888", sessionId);
            }

            //Проверка на наличие в базе точек
            db = new DBWork();
            //получение точек из бд
            var Data = db.getPointBySession(sessionId);
            if (Data.Count > 0)
                firstImport = false;
            else
                firstImport = true;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Open File";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "xls";
            openFileDialog1.Filter = "Microsoft Excel (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                return;
            }
            else
            {
                address = openFileDialog1.FileName;
            }

            try
            {
                using (FileStream file = new FileStream(address, FileMode.Open, FileAccess.Read))
                {
                    wb = new XSSFWorkbook(file);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Файл используется другим процессом или недоступен");
                return;
            }


            ISheet sheet = wb.GetSheetAt(0); //доделать проверку количества страниц 


            //проверка заголовков
            for (int row = 0; row < 1; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow != null)
                {
                    for (int column = 0; column < 6; column++)
                    {
                        var stringCellValue = currentRow.GetCell(column).StringCellValue;

                        if ((column == 0) && (stringCellValue != "Интернет-заказ"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Интернет-заказ' ", "Ошибка");
                            return;
                        }

                        if ((column == 1) && (stringCellValue != "Город"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Город' ", "Ошибка");
                            return;
                        }

                        if ((column == 2) && (stringCellValue != "Наименование улицы"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Наименование улицы' ", "Ошибка");
                            return;
                        }

                        if ((column == 3) && (stringCellValue != "Дом"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Дом' ", "Ошибка");
                            return;
                        }

                        if ((column == 4) && (stringCellValue != "Корпус"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Корпус' ", "Ошибка");
                            return;
                        }

                        if ((column == 5) && (stringCellValue != "Интервал доставки"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Интервал доставки' ", "Ошибка");
                            return;
                        }

                    }
                }
            }

            //проверка типов элементов таблицы
            for (int row = 1; row < sheet.LastRowNum; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow != null)
                {
                    for (int column = 0; column < 6; column++)
                    {
                        var Value = currentRow.GetCell(column);

                    }
                }
            }
            #endregion

            //Проверяем данные
            SheetVerif(sheet);

            var vData = verifdData;
            

            
            for (var q = 0; q < vData.Count; q++)
            {
                var currPoint = vData[q];
                var newP = true;
                //сравниваем по заказу
                foreach (var point in Data)
                {
                    //ищем идентичные строки
                    if (point.OrderNumber == currPoint.OrderNumber)
                    {
                        newP = false;
                        //если точка не идентична, то добавляется в список на обновление
                        if (!(point.City == currPoint.City &
                            point.Street == currPoint.Street &
                            point.House == currPoint.House &
                            point.Korp == currPoint.Korp &
                            point.Interval == currPoint.Interval))
                        {
                            currPoint.PointType = point.PointType;
                            currPoint.Comment = point.Comment;
                            currPoint.ID_Route = point.ID_Route;
                            currPoint.PointNumber = point.PointNumber;

                            updateDic.Add(currPoint, point.ID_Point);
                            //updateList.Add(currPoint);
                        }
                    }
                }
                //ищем новые точки
                if (newP)
                    newList.Add(currPoint);
            }

            
            //ищем точки на удаление
            foreach (var point in Data)
            {
                var delP = true;
                foreach (var sPoint in vData)
                {
                    if (sPoint.OrderNumber == point.OrderNumber)
                    {
                        delP = false;
                    }
                }
                if (delP)
                    deleteList.Add(point);
            }

            //Открываем окно изменений
            GUI.ChangesPg changesPage = new GUI.ChangesPg(errorList, newList, updateDic, deleteList);
            changesPage.ShowDialog();

            //Добавляем исправленые точки
            foreach (var point in _errorPoints)
            {
                try
                {
                    db.InsertPoint(
                        point.OrderNumber,
                        point.City,
                        point.Street,
                        point.House,
                        point.Korp,
                        point.Interval,
                        point.PointNumber,
                        point.PointType,
                        point.ID_Session,
                        point.ID_Route
                        );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                
            }

            //Добавляем новые точки
            foreach (var point in _newPoints)
            {
                try
                {
                    db.InsertPoint(
                        point.OrderNumber,
                        point.City,
                        point.Street,
                        point.House,
                        point.Korp,
                        point.Interval,
                        point.PointNumber,
                        point.PointType,
                        point.ID_Session,
                        point.ID_Route
                        );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            //Обновляем точки
            foreach (var pointKVP in _updateDic)
            {
                if (pointKVP.Key.Comment == null)
                    pointKVP.Key.Comment = "";
                var route = db.GetRouteById(pointKVP.Key.ID_Route);
                try
                {
                    db.ChangePoint(pointKVP.Value,
                        route.RouteNumber,
                        pointKVP.Key.Interval,
                        pointKVP.Key.PointNumber,
                        pointKVP.Key.PointType,
                        pointKVP.Key.ID_Session,
                        pointKVP.Key.Comment,
                        pointKVP.Key.Street,
                        pointKVP.Key.House,
                        pointKVP.Key.Korp,
                        pointKVP.Key.OrderNumber);
                    //db.UpdatePoint(pointKVP.Value, pointKVP.Key);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            //Удаляем точки
            foreach (var point in _delPoints)
            {
                try
                {
                    db.deletePoint(point.ID_Point);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            setRoutes();
            InitializeMap();

            updateDic.Clear();
            errorList.Clear();
            newList.Clear();
            deleteList.Clear();
            verifdData.Clear();
            _errorPoints.Clear();
            _updateDic.Clear();
            _newPoints.Clear();
            _delPoints.Clear();

        }

        private void SheetVerif (ISheet sheet)
        {
            for (var q = 1; q <= sheet.LastRowNum; q++)
            {
                var currentRow = sheet.GetRow(q);
                if (currentRow != null)
                {
                    var rowStatus = true;
                    for (int column = 0; column < 6; column++)
                    {
                        var Value = Convert.ToString(currentRow.GetCell(column));
                        if (Value == "" && column != 4)
                        {
                            rowStatus = false;
                        }



                        if (column == 5)
                        {
                            Regex r = new Regex(@"\s{2,}");
                            Value = r.Replace(Value, " ");
                            if ((Value == "с 9:00 до 21:00") ||
                            (Value == "с 9:00 до 12:00") ||
                            (Value == "с 12:00 до 15:00") ||
                            (Value == "с 15:00 до 18:00") ||
                            (Value == "с 18:00 до 21:00"))
                            {
                                switch (column)
                                {
                                    case (0):
                                        order = Value;
                                        break;
                                    case (1):
                                        city = Value;
                                        break;
                                    case (2):
                                        street = Value;
                                        break;
                                    case (3):
                                        house = Value;
                                        break;
                                    case (4):
                                        korp = Value;
                                        break;
                                    case (5):
                                        interval = Value;
                                        break;
                                }
                            }
                            else
                            {
                                rowStatus = false;
                                interval = Value;
                            }
                        }
                        else
                        {
                            switch (column)
                            {
                                case (0):
                                    order = Value;
                                    break;
                                case (1):
                                    city = Value;
                                    break;
                                case (2):
                                    street = Value;
                                    break;
                                case (3):
                                    house = Value;
                                    break;
                                case (4):
                                    korp = Value;
                                    break;
                                case (5):
                                    interval = Value;
                                    break;
                            }
                        }


                    }

                    var currPoint = new Point
                    {
                        OrderNumber = order,
                        City = city,
                        Street = street,
                        House = house,
                        Korp = korp,
                        Interval = interval,
                        PointNumber = 0,
                        PointType = 1,
                        ID_Session = sessionId,
                        ID_Route = 0
                    };

                    try
                    {
                        if (rowStatus)
                        {
                            // q+1 ,где 1 - заговок таблицы , чтобы точки совпадали с номером строчки
                            //db.InsertPoint(order, city, street, house, korp, interval, q + 1, 1, sessionId, 0);
                            verifdData.Add(currPoint);
                        }

                    }
                    catch (Exception)
                    {
                        rowStatus = false;

                    }

                    if (!rowStatus)
                    {
                        errorList.Add(currPoint);
                    }

                }
            }
        }



        private void Import_click(object sender, RoutedEventArgs e)
        {
            #region base_import
            bool hasRoutes = db.GetRoutsBySession(sessionId).Count != 0;
            if (!hasRoutes)
            {
                db.InsertRoute(0, "#ff888888", sessionId);
            }

            //Проверка на наличие в базе точек
            db = new DBWork();
                //получение точек из бд
                var Data = db.getPointBySession(sessionId);
            if (Data.Count > 0)
                firstImport = false;
            else
                firstImport = true;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Open File";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "xls";
            openFileDialog1.Filter = "Microsoft Excel (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                return;
            }
            else
            {
                address = openFileDialog1.FileName;
            }

            try
            {
                using (FileStream file = new FileStream(address, FileMode.Open, FileAccess.Read))
                {
                    wb = new XSSFWorkbook(file);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Файл используется другим процессом или недоступен");
                return;
            }


            ISheet sheet = wb.GetSheetAt(0); //доделать проверку количества страниц 


            //проверка заголовков
            for (int row = 0; row < 1; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow != null)
                {
                    for (int column = 0; column < 6; column++)
                    {
                        var stringCellValue = currentRow.GetCell(column).StringCellValue;

                        if ((column == 0) && (stringCellValue != "Интернет-заказ"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Интернет-заказ' ", "Ошибка");
                            return;
                        }

                        if ((column == 1) && (stringCellValue != "Город"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Город' ", "Ошибка");
                            return;
                        }

                        if ((column == 2) && (stringCellValue != "Наименование улицы"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Наименование улицы' ", "Ошибка");
                            return;
                        }

                        if ((column == 3) && (stringCellValue != "Дом"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Дом' ", "Ошибка");
                            return;
                        }

                        if ((column == 4) && (stringCellValue != "Корпус"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Корпус' ", "Ошибка");
                            return;
                        }

                        if ((column == 5) && (stringCellValue != "Интервал доставки"))
                        {
                            MessageBox.Show("Некорректное заголовок 'Интервал доставки' ", "Ошибка");
                            return;
                        }

                    }
                }
            }

            //проверка типов элементов таблицы
            for (int row = 1; row < sheet.LastRowNum; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow != null)
                {
                    for (int column = 0; column < 6; column++)
                    {
                        var Value = currentRow.GetCell(column);

                    }
                }
            }
            #endregion

            
            //проверка данных
            SheetVerif(sheet);

            int q = 0;
            foreach (var row in verifdData)
            {
                
                try
                {
                    db.InsertPoint(row.OrderNumber,
                        row.City,
                        row.Street,
                        row.House,
                        row.Korp,
                        row.Interval,
                        0,
                        1, 
                        sessionId, 0);
                    q++;
                }
                catch (Exception)
                {
                }
            }

            
            //MessageBox.Show("Импорт завершен");



            if (errorList.Count > 0)
            {
                //Открываем окно изменений
                GUI.ChangesPg changesPage = new GUI.ChangesPg(errorList, newList, updateDic, deleteList);
                changesPage.ShowDialog();

                foreach (var point in _errorPoints)
                {
                    db.InsertPoint(
                        point.OrderNumber,
                        point.City,
                        point.Street,
                        point.House,
                        point.Korp,
                        point.Interval,
                        point.PointNumber,
                        point.PointType,
                        point.ID_Session,
                        point.ID_Route
                        );
                }
            }

            setRoutes();
            InitializeMap();
            _errorPoints.Clear();
            errorList.Clear();
            verifdData.Clear();
            firstImport = false;
        }

        

        private int FindOrder(string order, List<Point> Data)
        {
            var i = 0;
            var orderFound = false;
            for (i = 0; i < Data.Count; i++)
            {
                if (Data[i].OrderNumber == order)
                {
                    orderFound = true;
                    break;
                }
                          
            }
            if (orderFound)
                return i;
            else
                return -1;
            
        }

        private void AddPoint_click(object sender, RoutedEventArgs e)
        {
            bool hasRoutes = db.GetRoutsBySession(sessionId).Count != 0;
            if (!hasRoutes)
            {
                db.InsertRoute(0, "#ff888888", sessionId);
            }
            NewPoint point = new NewPoint(this, sessionId);
            point.Show();
            point.DataChanged += UpdateAll;

        }

        public void UpdateAll(object sender, EventArgs e)
        {
            setRoutes();
            InitializeMap();
        }

        private void AddRoute_click(object sender, RoutedEventArgs e)
        {
            List<int> routeNumbers = new List<int>();
            foreach (var r in Routes)
                routeNumbers.Add(r.routeNumber);
            NewRoute route = new NewRoute(sessionId, routeNumbers);
            route.Show();
            route.DataChanged += UpdateAll;
        }

        private void CreatePDFPrint_Unknown_route(string filename)
        {
            //Создаем новый маршрут со всеми точками
            newRoute routeOut = new newRoute();
            routeOut.point1 = new List<NewPoints>();
            routeOut.point2 = new List<NewPoints>();
            routeOut.point3 = new List<NewPoints>();
            routeOut.point4 = new List<NewPoints>();
            routeOut.point5 = new List<NewPoints>();

            foreach (var route in Routes)
            {

                routeOut.point1.AddRange(route.point1);
                routeOut.point2.AddRange(route.point2);
                routeOut.point3.AddRange(route.point3);
                routeOut.point4.AddRange(route.point4);
                routeOut.point5.AddRange(route.point5);
            }

            //перезаполняем нумерацию
            int numInInterval = 0;
            foreach (var point in routeOut.point1)
            {
                numInInterval++;
                var ch = point.pointText.IndexOf(':');
                point.pointText = (point.pointText.Remove(0, ch)).Insert(0, Convert.ToString(numInInterval));
            }
            numInInterval = 0;
            foreach (var point in routeOut.point2)
            {
                numInInterval++;
                var ch = point.pointText.IndexOf(':');
                point.pointText = (point.pointText.Remove(0, ch)).Insert(0, Convert.ToString(numInInterval));
            }
            numInInterval = 0;
            foreach (var point in routeOut.point3)
            {
                numInInterval++;
                var ch = point.pointText.IndexOf(':');
                point.pointText = (point.pointText.Remove(0, ch)).Insert(0, Convert.ToString(numInInterval));
            }
            numInInterval = 0;
            foreach (var point in routeOut.point4)
            {
                numInInterval++;
                var ch = point.pointText.IndexOf(':');
                point.pointText = (point.pointText.Remove(0, ch)).Insert(0, Convert.ToString(numInInterval));
            }
            numInInterval = 0;
            foreach (var point in routeOut.point5)
            {
                numInInterval++;
                var ch = point.pointText.IndexOf(':');
                point.pointText = (point.pointText.Remove(0, ch)).Insert(0, Convert.ToString(numInInterval));
            }

            //Наполняем PDF
            string path = System.IO.Path.Combine(Path.GetTempPath(), "print.png");
            string fg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Fradm.TTF");
            BaseFont fgBaseFont = BaseFont.CreateFont(fg, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fgFont = new iTextSharp.text.Font(fgBaseFont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Document doc = new Document(iTextSharp.text.PageSize.A4, 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));//textBox1(in the form) assign here the title of the document
            doc.Open(); //open the document in order to write inside.

            Phrase ph = new Phrase("Все точки", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "9-12", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point1.Count > 0)
            {
                foreach (var point in routeOut.point1)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments,  fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "12-15", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point2.Count > 0)
            {
                foreach (var point in routeOut.point2)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));

                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "15-18", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point3.Count > 0)
            {
                foreach (var point in routeOut.point3)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "18-21", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point4.Count > 0)
            {
                foreach (var point in routeOut.point4)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "9-21", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point5.Count > 0)
            {
                foreach (var point in routeOut.point5)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            doc.Close();


            return;
        }

        private void CreatePDFPrint_Known_route(newRoute routeOut, string filename)
        {
            //Наполняем PDF
            string path = System.IO.Path.Combine(Path.GetTempPath(), "print.png");
            string fg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Fradm.TTF");
            BaseFont fgBaseFont = BaseFont.CreateFont(fg, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fgFont = new iTextSharp.text.Font(fgBaseFont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));//textBox1(in the form) assign here the title of the document
            doc.Open(); //open the document in order to write inside.

            //Добавляем картинку на фон
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(path);
            image.ScaleToFit(doc.PageSize);
            image.SetAbsolutePosition(0, 0);
            doc.Add(image);


            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            doc.NewPage();


            Phrase ph = new Phrase(routeOut.routeText, fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "9-12", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point1.Count > 0)
            {
                foreach (var point in routeOut.point1)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "12-15", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point2.Count > 0)
            {
                foreach (var point in routeOut.point2)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));

                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "15-18", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point3.Count > 0)
            {
                foreach (var point in routeOut.point3)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "18-21", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point4.Count > 0)
            {
                foreach (var point in routeOut.point4)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            ph = new Phrase(" " + "9-21", fgFont);
            doc.Add(ph);
            doc.Add(new Paragraph(" "));
            if (routeOut.point5.Count > 0)
            {
                foreach (var point in routeOut.point5)
                {
                    ph = new Phrase(" - " + point.pointText + "; Комм: " + point.comments, fgFont);
                    doc.Add(ph);
                    doc.Add(new Paragraph(" "));
                }
            }
            else
            {
                ph = new Phrase(" - " + "нет точек в интервале", fgFont);
                doc.Add(ph);
            }
            doc.Add(new Paragraph(" "));

            doc.Close();
        }



        private void Print_click(object sender, RoutedEventArgs e)
        {
            //Получаем маршрут
            var selRoute = (comboBox_Print_Route)comboBox_Print.SelectedItem;
            newRoute routeOut = Routes[0];
            if (selRoute != null)
            {
                var numberRoute = selRoute.Id;

                if (selRoute.Id > -1)
                {
                    foreach (var route in Routes)
                    {
                        if (route.routeNumber == numberRoute)
                            routeOut = route;
                    }
                }


                string path = System.IO.Path.Combine(Path.GetTempPath(), "print.png");

                //скриншот области браузера
                var bmp = Gecko.Utils.WindowsImageCreator.GetBitmap(
                    browser,
                    Convert.ToUInt32(browser.Width),
                    Convert.ToUInt32(browser.Height));

                ////Т.к. скриншот снимается в 4хкратном размере (причина неизвестна) то обрезаем лишнее пространство полотна
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle();
                try
                {
                    switch (Properties.Settings.Default.printRes)
                    {
                        case "1x":
                            rectangle = new System.Drawing.Rectangle(0, 0, bmp.Width / 1, bmp.Height / 1);
                            break;
                        case "2x":
                            rectangle = new System.Drawing.Rectangle(0, 0, bmp.Width / 2, bmp.Height / 2);
                            break;
                    }
                }
                catch
                {
                    rectangle = new System.Drawing.Rectangle(0, 0, bmp.Width / 2, bmp.Height / 2);
                }

                var bmp2 = bmp.Clone(rectangle, bmp.PixelFormat);

                bmp2.Save(path);
                //bmp.Save(path);


                DateTime fileCreationDatetime = DateTime.Now;

                //Получение пути
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = string.Format("{0}", fileCreationDatetime.ToString(@"yyyyMMdd") + "_" + fileCreationDatetime.ToString(@"HHmmss")); // Default file name
                dlg.DefaultExt = ".pdf"; // Default file extension
                dlg.Filter = "PDF Docs (.pdf)|*.pdf"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    string filename = dlg.FileName;

                    //Создание PDF документа
                    string fileName = string.Empty;




                    if (selRoute.Id == -1)
                        CreatePDFPrint_Unknown_route(filename);
                    else
                        CreatePDFPrint_Known_route(routeOut, filename);

                    //var adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("App Paths").OpenSubKey("AcroRd32.exe");
                    //var pathToAdobe = adobe.GetValue("");

                    //var process = new Process();
                    //process.StartInfo = new ProcessStartInfo((string)pathToAdobe+" /p "+filename);
                    //process.StartInfo.UseShellExecute = false;
                    //process.Start();

                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Выберите маршрут", "Не выбран маршрут", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void setPointMap(string point)
        {
            browser.Navigate(point);
        }

        private void EditRoute_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var routes = ((newRoute)(button.DataContext)).GUIroute;
            if (routes.RouteNumber != 0)
            {
                ChangeRoute change = new ChangeRoute(routes);
                change.Show();
                setRoutes();
                change.DataChanged += UpdateAll;

            }

        }

        public class comboBox_Print_Route
        {
            //public Bitmap ColorBmp { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void comboBox_Print_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selRoute = (comboBox_Print_Route)comboBox_Print.SelectedItem;
            if (selRoute != null)
            {
                var numberRoute = selRoute.Id;
                newRoute routeOut = Routes[0];
                foreach (var route in Routes)
                {
                    if (route.routeNumber == numberRoute)
                        routeOut = route;
                }
                routeClick_route(routeOut);
            }

        }

        private void HideElem_Click(object sender, RoutedEventArgs e)
        {
            //Страшно. Очень страшно.
            var button = (Button)sender;
            var par = ((StackPanel)(button.Parent)).Parent;
            Expander pare = (Expander)par;

            if (pare.IsExpanded == false)
                pare.IsExpanded = true;
            else
                pare.IsExpanded = false;


        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            UpdateAll(sender, e);
        }


        private void DeleteElem_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var routes = ((newRoute)(button.DataContext)).GUIroute;
            if (routes.RouteNumber != 0)
            {
                db.deleteRoute(routes.ID_Route);
                setRoutes();

                InitializeMap();
            }
        }

        private void Интерфейс_сессии_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            browser.Stop();
            browser.Dispose();
            browser = null;
        }

        private void GridPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void routeClick_route(newRoute route)
        {

            var item = route;
            var currRoute = (newRoute)item;
            var currPoint1 = currRoute.point1;
            var currPoint2 = currRoute.point2;
            var currPoint3 = currRoute.point3;
            var currPoint4 = currRoute.point4;
            var currPoint5 = currRoute.point5;

            string[] data = new string[currPoint1.Count() + currPoint2.Count() + currPoint3.Count() + currPoint4.Count() + currPoint5.Count()];
            int count = 0;

            for (int i = 0; i < currPoint1.Count(); i++)
            {
                try
                {
                    data[count] = currPoint1[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint2.Count(); i++)
            {
                try
                {
                    data[count] = currPoint2[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint3.Count(); i++)
            {
                try
                {
                    data[count] = currPoint3[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }


            for (int i = 0; i < currPoint4.Count(); i++)
            {
                try
                {
                    data[count] = currPoint4[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint5.Count(); i++)
            {
                try
                {
                    data[count] = currPoint5[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            using (AutoJSContext context = new AutoJSContext(browser.Window))
            {
                var addressPoint = JsonConvert.SerializeObject(data);
                var routePoints = browser.Document.GetElementById("route_points");
                routePoints.SetAttribute("data-address-route", addressPoint);
                var zoom = browser.Document.GetElementById("zoom_route").DomObject;
                var zoomButton = new GeckoInputElement(zoom);
                zoomButton.Click();
                //try
                //{
                //    string result = String.Empty;
                //    browser.Focus();
                //    var res = context.EvaluateScript("zoomRoute();", out result);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Error: " + ex);
                //}

            }
        }


        private void routeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            var item = ((ListViewItem)sender).Content;
            var currRoute = (newRoute)item;
            var currPoint1 = currRoute.point1;
            var currPoint2 = currRoute.point2;
            var currPoint3 = currRoute.point3;
            var currPoint4 = currRoute.point4;
            var currPoint5 = currRoute.point5;

            string[] data = new string[currPoint1.Count() + currPoint2.Count() + currPoint3.Count() + currPoint4.Count() + currPoint5.Count()];
            int count = 0;

            for (int i = 0; i < currPoint1.Count(); i++)
            {
                try
                {
                    data[count] = currPoint1[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint2.Count(); i++)
            {
                try
                {
                    data[count] = currPoint2[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint3.Count(); i++)
            {
                try
                {
                    data[count] = currPoint3[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }


            for (int i = 0; i < currPoint4.Count(); i++)
            {
                try
                {
                    data[count] = currPoint4[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            for (int i = 0; i < currPoint5.Count(); i++)
            {
                try
                {
                    data[count] = currPoint5[i].Address;
                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            using (AutoJSContext context = new AutoJSContext(browser.Window))
            {
                var addressPoint = JsonConvert.SerializeObject(data);
                var routePoints = browser.Document.GetElementById("route_points");
                routePoints.SetAttribute("data-address-route", addressPoint);
                var zoom = browser.Document.GetElementById("zoom_route").DomObject;
                var zoomButton = new GeckoInputElement(zoom);
                zoomButton.Click();
                //try
                //{
                //    string result = String.Empty;
                //    browser.Focus();
                //    var res = context.EvaluateScript("zoomRoute();", out result);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Error: " + ex);
                //}

            }
        }

        private void pointClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //string result;
            var item = ((ListViewItem)sender).Content;
            var currPoint = (NewPoints)item;

            var addressPoint = JsonConvert.SerializeObject(currPoint.Address);
            var routePoints = browser.Document.GetElementById("address_point");
            routePoints.SetAttribute("data-address-point", addressPoint);
            var zoom = browser.Document.GetElementById("zoom_point").DomObject;
            var zoomButton = new GeckoInputElement(zoom);
            zoomButton.Click();

            //using (AutoJSContext context = new AutoJSContext(browser.Window))
            //{
            //    var addressPoint = JsonConvert.SerializeObject(currPoint.Address);
            //    var res = context.EvaluateScript("zoomPoint(" + addressPoint + ");", out result);
            //}

        }


        private void ErrorList_Click(object sender, RoutedEventArgs e)
        {


        }
    }

    
}
