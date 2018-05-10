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

using System.Printing;

using Gecko;
using System.Windows.Forms.Integration;

//using System.Web.Script.Services;
using System.Web.Services;

using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Win32;

using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock.Layout;
using Newtonsoft.Json;

namespace LProject
{

    public class newRoute1
    {
        public string editRouteImage { get; set; }
        public string hideElemImage { get; set; }
        public string deleteElemImage { get; set; }

        public string routeText { get; set; }

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

    public class NewPoints1
    {
        public int Id { get; set; }
        public string pointText { get; set; }
        public string Address { get; set; }
        public int PointType { get; set; }
        public int pointId { get; set; }
    }
    
    public partial class PrintRoute : Window
    {
        DBWork db;
        GeckoWebBrowser browser;
        bool IsBrowserLoad = false;
        int sessionId;
        int routeId;
        string URL = "file:///" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GUI", "index.html");


        string address;
        XSSFWorkbook wb;
        XSSFSheet sh;

        ObservableCollection<newRoute> Routes;
        string city, street, house, korp, interval, order = "";

        bool PointCount = false;

        public PrintRoute(int sesID, int rourID)
        {
            
            InitializeComponent();

            sessionId = sesID;
            routeId = rourID;
            db = new DBWork();

            db.getPointBySession(sessionId);
            
            Gecko.Xpcom.Initialize("Firefox");
            WindowsFormsHost host = new WindowsFormsHost();
            browser = new GeckoWebBrowser();
            browser.DocumentCompleted += (sender, e) => { DrawRoutesOnMap(); };

            host.Child = browser;
            GridWeb.Children.Add(host);
            browser.Navigate(URL);

            setRoutes();
            //point Update
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(GridPanel, "GridPrinting");
        }      

        public void setRoutes()
        {
            db = new DBWork();
            Routes = new ObservableCollection<newRoute> { };

            var Data = db.getPointBySession(sessionId);
            var sizeData = Data.Count;
            string dataPoint = "";

            var dbRoutes = db.GetRoutsBySession(sessionId);

            foreach (var item in dbRoutes)
            {
                if (item.RouteNumber == routeId)
                {

                    List<NewPoints> pointSet1 = new List<NewPoints>();
                    List<NewPoints> pointSet2 = new List<NewPoints>();
                    List<NewPoints> pointSet3 = new List<NewPoints>();
                    List<NewPoints> pointSet4 = new List<NewPoints>();
                    List<NewPoints> pointSet5 = new List<NewPoints>();

                    for (var k = 0; k < sizeData; k++)
                    {
                        var elem = Data[k];
                        var city = elem.City;
                        var street = elem.Street;
                        var house = elem.House + "";
                        var korp = elem.Korp + "";
                        var interval = elem.Interval;

                        var route = elem.ID_Route;
                        var pointID = elem.ID_Point;

                        if (route == item.ID_Route)
                        {
                            string streetAndHouse;
                            if ((korp == "") || (korp == "0"))
                            {
                                streetAndHouse = "ул. " + street + ", " + house;
                                dataPoint = "   #1" + ": " + streetAndHouse;
                            }
                            else
                            {
                                streetAndHouse = "ул. " + street + ", " + house + " к" + korp;
                                dataPoint = "   #1" + ": " + streetAndHouse;
                            }

                            var newPoint = new NewPoints
                            {
                                pointId = elem.ID_Point,
                                pointText = dataPoint,
                                Order = elem.OrderNumber,
                                Address = city + ", " + streetAndHouse
                            };
                            switch (interval)
                            {
                                case ("с 9:00 до 12:00"):
                                    pointSet1.Add(newPoint);
                                    break;
                                case ("с 12:00 до 15:00"):
                                    pointSet2.Add(newPoint);
                                    break;
                                case ("с 15:00 до 18:00"):
                                    pointSet3.Add(newPoint);
                                    break;
                                case ("с 18:00 до 21:00"):
                                    pointSet4.Add(newPoint);
                                    break;
                                case ("с 9:00 до 21:00"):
                                    pointSet5.Add(newPoint);
                                    break;
                            }
                        }
                    }


                    if (item.RouteNumber == 0)
                        Routes.Add(new newRoute
                        {
                            routeText = "Без маршрута",
                            point1 = pointSet1,
                            point2 = pointSet2,
                            point3 = pointSet3,
                            point4 = pointSet4,
                            point5 = pointSet5,
                            GUIroute = item,
                            RouteColor = item.Color
                        });
                    else Routes.Add(new newRoute
                    {
                        routeText = "Маршрут " + item.RouteNumber,
                        point1 = pointSet1,
                        point2 = pointSet2,
                        point3 = pointSet3,
                        point4 = pointSet4,
                        point5 = pointSet5,
                        GUIroute = item,
                        RouteColor = item.Color
                    });

                }
            }
            routesList.ItemsSource = Routes;
        }

        public void DrawRoutesOnMap()
        {
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
                            order = p.Order
                        };
                    })
                };
            });



            string result;
            using (AutoJSContext context = new AutoJSContext(browser.Window))
            {
                var json = JsonConvert.SerializeObject(mapRoutes);
                var res = context.EvaluateScript("drawRoutes(" + json + ");", out result);
            }
        } 
        
        public void UpdateAll(object sender, EventArgs e)
        {
            setRoutes();
        }               
       
        public void setPointMap(string point)
        {
            browser.Navigate(point);
        }        

        private void GridPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        [WebMethod]
        public static string ProcessIT(string name, string address)
        {
            string result = "Welcome Mr. " + name + ". Your address is '" + address + "'.";
            return result;
        }
    }
}
