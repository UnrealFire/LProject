using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LProject.GUI
{
    public class PointRN
    {
        public static int FirstNumber(List<Route> listRoute, List<Point> data, int way)
        {
            //находим маршрут
            var w = 0;
            try
            {
                while (listRoute[w].RouteNumber != way)
                {
                    w++;
                }
            }
            catch (Exception)
            {
                
                return 0;
            }

            var route = listRoute[w];

            //Находим количество точек на этом маршруте
            var count = 0;
            foreach (var point in data)
            {
                if (point.ID_Route == route.ID_Route)
                    count++;
            }

            count++;

            return count;
        }

        public static int NumberInsert(int pointRN_new, List<Route> listRoute, List<Point> data, int way)
        {
            //Вставка новой точки в маршрут. Варианты:
            //Новая точка встает в центр - сдивг точек начиная с номера новой точки
            //Новая точка встает дальше края - вначале для выявления этого получаем последний свободный номер, если он меньше номера новой точки то новой точке присвается последний номер
            //Новая точка встает на место крайней - см первый вариант

            //находим маршрут
            var w = 0;
            try
            {
                while (listRoute[w].RouteNumber != way)
                {
                    w++;
                }
            }
            catch (Exception)
            {

                return 0;
            }

            var route = listRoute[w];

            //если новый номер больше последнего то номер сохраняется как последний +1
            var firstNumber = FirstNumber(listRoute, data, way);
            if (pointRN_new >= firstNumber)
            {
                pointRN_new = firstNumber;
                return pointRN_new;
            }

            //для каждой найденной точки на маршруте, большей, чем новый номер или равной ему, увеличивается номер на 1
            foreach (var point in data)
            {
                if (point.ID_Route == route.ID_Route)
                {
                    if (point.PointType >= pointRN_new)
                    {
                        //прибавляем точке номер и обновляем в бд
                        point.PointType++;
                        DBWork db = new DBWork();
                        db.UpdatePoint(point.ID_Point, point);
                    }
                }
            }

            return pointRN_new;
            
        }

        public static int NumberMove(int pointRN_new, int pointRN_old, List<Route> listRoute, List<Point> data, int way)
        {
            //Номер больше старого, но меньше или равен последнему - все номера начиная с нового сдвигаются на 1 вперед.
            //номер больше старого, и больше последнего - номер приравнивается последнему 
            //Номер меньше старого - все номера больше или равный новому сдвигаются на 1

            //Происходит проверка на пробелы - выстраивается ряд точек на маршруте, если у какой то точки кроме последней нету точки перед ней, то все точки больше последней порядочной сдвигаются назад на 1


            //находим маршрут
            var w = 0;
            try
            {
                while (listRoute[w].RouteNumber != way)
                {
                    w++;
                }
            }
            catch (Exception)
            {

                return 0;
            }

            var route = listRoute[w];
            

            //если новый номер больше последнего то номер сохраняется как последний +1
            var firstNumber = FirstNumber(listRoute, data, way);
            if (pointRN_new > firstNumber)
            {
                pointRN_new = firstNumber;
                return pointRN_new;
            }
            //осуществляем сдвиг, освобождая место под новый номер
            else
            {
                foreach (var point in data)
                {
                    if (point.ID_Route == route.ID_Route)
                    {
                        if (point.PointType >= pointRN_new)
                        {
                            //прибавляем точке номер и обновляем в бд
                            point.PointType++;
                            DBWork db = new DBWork();
                            db.UpdatePoint(point.ID_Point, point);
                        }
                    }
                }
                return pointRN_new;
            }

        }

        public static void SpaceChecking (List<Route> listRoute, List<Point> data, int way)
        {
            //находим маршрут
            var w = 0;
            try
            {
                while (listRoute[w].RouteNumber != way)
                {
                    w++;
                }
            }
            catch (Exception)
            {

                return;
            }

            var route = listRoute[w];

            //Создаем лист точек на маршруте для проверки целостности ряда
            List<Point> wayPoints = new List<Point>() { };

            foreach (var point in data)
            {
                if (point.ID_Route == route.ID_Route)
                {
                    wayPoints.Add(point);
                }
            }

            //проверяем ряд на пробелы, составляем список точек с пробелами после них
            List<bool> nextNums = new List<bool>() { };
            foreach (var point in wayPoints)
            {
                bool nextNum = false;
                foreach (var point1 in wayPoints)
                {
                    if (point.PointType + 1 == point1.PointType)
                    {
                        nextNum = true;
                    }
                }
                nextNums.Add(nextNum);
            }

            //составляем список точек с false
            Dictionary<Point, bool> falseNums = new Dictionary<Point, bool>() { };

            for (int i = 0; i < wayPoints.Count; i++)
            {
                if (!nextNums[i])
                {
                    falseNums.Add(wayPoints[i], false);
                }
            }

            //Находим последнюю точку. Если больше нет точек без последущих, то возвращаем номер

            if (falseNums.Count == 2)
            {
                if (falseNums.ElementAt(0).Key.PointType > falseNums.ElementAt(1).Key.PointType)
                {
                    foreach (var point in wayPoints)
                    {
                        if (point.PointType > falseNums.ElementAt(1).Key.PointType)
                        {
                            //прибавляем точке номер и обновляем в бд
                            point.PointType--;
                            DBWork db = new DBWork();
                            db.UpdatePoint(point.ID_Point, point);
                        }
                    }
                }
                else
                {
                    foreach (var point in wayPoints)
                    {
                        if (point.PointType > falseNums.ElementAt(0).Key.PointType)
                        {
                            //прибавляем точке номер и обновляем в бд
                            point.PointType--;
                            DBWork db = new DBWork();
                            db.UpdatePoint(point.ID_Point, point);
                        }
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }

        public static void DeleteNumber (int pointRN_del, List<Route> listRoute, List<Point> data, int way)
        {
            //При удалении точки делаем сдвиг всех последующих номеров на  -1

            //находим маршрут
            var w = 0;
            try
            {
                while (listRoute[w].RouteNumber != way)
                {
                    w++;
                }
            }
            catch (Exception)
            {

                
            }
            var route = listRoute[w];

            //для каждой найденной точки на маршруте, большей, чем удаляемый номер, уменьшается номер на 1
            foreach (var point in data)
            {
                if (point.ID_Route == route.ID_Route)
                {
                    if (point.PointType > pointRN_del)
                    {
                        //убавляем точке номер и обновляем в бд
                        point.PointType--;
                        DBWork db = new DBWork();
                        db.UpdatePoint(point.ID_Point, point);
                    }
                }
            }
        }
    }
}
