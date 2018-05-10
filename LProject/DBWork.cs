using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Windows;

namespace LProject
{
 
    class DBWork
    {
        private DataContext db;

        

        public DBWork()
        {


            string serverName = GetName();

            db = new DataContext(serverName);

            db.Log = Console.Out;
        }

        private string GetName()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            string name = "";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    name = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read the file");
            }
            return name;
        }

        public List<string> getNames()
        {
            Table<Man> man = db.GetTable<Man>();


            //Select
            var nameQuery =
                from cust in man
                select cust.PersonName;

            List<string> names = new List<string>();

            foreach (var line in nameQuery)
            {
                names.Add(line);
            }

            return names;
        }

        public void InsertUser(string user, string pass, bool isAdmin = false)
        {
            Table<Man> man = db.GetTable<Man>();

            ////Insert
            Man newMan = new Man();
            newMan.PersonName = user;
            newMan.Pass = pass;
            newMan.IsAdmin = isAdmin;
            man.InsertOnSubmit(newMan);

            db.SubmitChanges();
        }

        public List<Man> GetUserDataByName(string Name)
        {
            Table<Man> man = db.GetTable<Man>();

            var nameQuery =
                from cust in man
                where cust.PersonName == Name
                select cust;

            List<Man> personData = new List<Man>();

            foreach (var line in nameQuery)
            {
                personData.Add(line);
            }
            // Ищем в базе соответствие имя/пароль и возвращем сессию с данными, если они есть, и ничего, если их нет.
            return personData;
        }

        public List<Session> getSession(string name)
        {

            Table<Session> session = db.GetTable<Session>();

            var nameQuery =
                from cust in session
                select cust;

            List<Session> sessionList = new List<Session>();

            foreach (var line in nameQuery)
            {
                sessionList.Add(line);
            }

            return sessionList;
        }

        public void DeleteUser(string user)
        {
            Table<Man> man = db.GetTable<Man>();

            var query = (from cust in man
                         where cust.PersonName == user
                         select cust);

            if (query != null)
            {
                foreach (var line in query)
                    man.DeleteOnSubmit(line);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
        }

        public void saveUserNameChanges(string oldName, string newName)
        {
            Table<Man> man = db.GetTable<Man>();

            var query = (from cust in man
                         where cust.PersonName == oldName
                         select cust).SingleOrDefault();

            query.PersonName = newName;

            db.SubmitChanges();
        }

        public void saveUserPassChanges(string userName, string newPass)
        {
            var salt = userName.Substring(3);
            Security sec = new Security();
            var pass = sec.HashPassword(newPass, salt);
            Table<Man> man = db.GetTable<Man>();

            var query = (from cust in man
                         where cust.PersonName == userName
                         select cust).SingleOrDefault();

            query.Pass = pass;

            db.SubmitChanges();
        }

        public List<Session> getSessionByName(string name)
        {
            Table<Session> session = db.GetTable<Session>();
            Table<Man> man = db.GetTable<Man>();

            var nameQuery =
                from cust in session
                join m in man on cust.ID_People equals m.ID_People
                where m.PersonName == name
                select cust;

            List<Session> sessionList = new List<Session>();

            foreach (var line in nameQuery)
            {
                sessionList.Add(line);
            }

            return sessionList;
        }

        public void InsertSession(string session, string name, int time = 0)
        {
            Table<Session> ses = db.GetTable<Session>();
            Table<Man> man = db.GetTable<Man>();

            var query = (from cust in man
                         where cust.PersonName == name
                         select cust.ID_People).SingleOrDefault();

            Session newSess = new Session();
            newSess.Session_Name = session;
            newSess.ArchTime = DateTime.Today.AddDays(time);
            newSess.CreationDate = DateTime.Today;
            newSess.ID_People = query;

            ses.InsertOnSubmit(newSess);
            db.SubmitChanges();
        }

        public void ArchiveSession(int ID, bool archive /*= true*/)
        {
            Table<Session> sec = db.GetTable<Session>();
            
            var query = (from cust in sec
                         where cust.ID_Session == ID
                         select cust).SingleOrDefault();

            query.IsArchived = archive;
            if (!archive)
                query.ArchTime = new DateTime(2030, 8, 18);
            db.SubmitChanges();
        }

        public void deleteSession(int ID)
        {
            Table<Session> sec = db.GetTable<Session>();

            //deleteRoute(ID);

            var query = (from cust in sec
                         where cust.ID_Session == ID
                         select cust);

            if (query != null)
            {
                foreach (var line in query)
                    sec.DeleteOnSubmit(line);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
        }

        public void InsertRoute(int number, string hex, int session)
        {
            Table<Route> route = db.GetTable<Route>();

            ////Insert
            Route newRoute = new Route();
            newRoute.RouteNumber = number;
            newRoute.Color = hex;
            newRoute.ID_Session = session;
            route.InsertOnSubmit(newRoute);

            db.SubmitChanges();
        }

        public void ChangePoint(string inter, int number, int type, int id)
        {
            Table<Point> point = db.GetTable<Point>();

            ////Insert
            Point newPoint = new Point();
            newPoint.Interval = inter;
            newPoint.PointNumber = number;
            newPoint.PointType = type;

            Table<Route> route = db.GetTable<Route>();

            var query = (from cust in route
                         where cust.RouteNumber == id
                         select cust.ID_Route).SingleOrDefault();

            newPoint.ID_Route = query;



            //point.InsertOnSubmit(newPoint);

            db.SubmitChanges();
        }

        public List<Route> GetRoutsBySession(int SessionId)
        {
            Table<Route> route = db.GetTable<Route>();

            List<Route> routeList = new List<Route>();

            var numberQuery =
                from cust in route
                where cust.ID_Session == SessionId
                orderby cust.RouteNumber ascending
                select cust;

            foreach (var line in numberQuery)
            {
                routeList.Add(line);
            }

            return routeList;
        }


        public void InsertPoint(string order, string city, string street, string house, string korp, string inter, int number, int type, int ses_id, int route)
        {
            Table<Point> point = db.GetTable<Point>();

            ////Insert
            Point newPoint = new Point();
            newPoint.City = city;
            newPoint.Street = street;
            newPoint.House = house;
            newPoint.Korp = korp;
            newPoint.Interval = inter;
            newPoint.PointNumber = number;
            newPoint.PointType = type;
            newPoint.OrderNumber = order;
            newPoint.ID_Session = ses_id;

            Table<Route> routeDB = db.GetTable<Route>();

            var query = (from cust in routeDB
                         where cust.RouteNumber == route &&
                                cust.ID_Session == ses_id
                         select cust.ID_Route).SingleOrDefault();

            newPoint.ID_Route = query;

            point.InsertOnSubmit(newPoint);

            db.SubmitChanges();
        }


        public List<Point> getPointBySession(int sessionID)
        {
            Table<Point> point = db.GetTable<Point>();

            var res =
                from cust in point
                where cust.ID_Session == sessionID
                select cust;
            var result = new List<Point>();

            foreach (var category in res)
            {
                result.Add(category);
            }


            return result;
        }

        public void ChangeRoute(int id, int newNumber, string newColor)
        {
            Table<Route> route = db.GetTable<Route>();
            
            var query = (from cust in route
                         where cust.ID_Route == id
                         select cust).SingleOrDefault();

            query.Color = newColor;
            query.RouteNumber = newNumber;

            db.SubmitChanges();
        }

        public void deleteRoute(int ID)
        {
            Table<Route> route = db.GetTable<Route>();
            Table<Point> point = db.GetTable<Point>();

            var query1 = (from cust in point
                          where cust.ID_Route == ID
                         select cust);

            var query = (from cust in route
                         where cust.ID_Route == ID
                         select cust);

            if (query1 != null)
            {
                foreach (var line in query1)
                    point.DeleteOnSubmit(line);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }

            if (query != null)
            {
                foreach (var line in query)
                    route.DeleteOnSubmit(line);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
        }


        public void deletePoint(int id)
        {
            Table<Point> point = db.GetTable<Point>();

            var query = (from cust in point
                         where cust.ID_Point == id
                         select cust);


            if (query != null)
            {
                foreach (var line in query)
                    point.DeleteOnSubmit(line);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }

        }


        public void ChangePoint(int id, int route, string interval, int number, int type, int sesId, string comment)
        {
            Table<Point> point = db.GetTable<Point>();
            Table<Route> rt = db.GetTable<Route>();

            var query = (from cust in point
                         where cust.ID_Point == id &&
                                cust.ID_Session == sesId
                         select cust).SingleOrDefault();

            var query2 = (from cust in rt
                         where cust.RouteNumber == route &&
                         cust.ID_Session == sesId
                         select cust).SingleOrDefault();


            query.ID_Route = query2.ID_Route;
            query.Interval = interval;
            query.PointNumber = number;
            query.PointType = type;
            query.Comment = comment;

            db.SubmitChanges();
        }

        public Point GetPointById(int Id)
        {
            Table<Point> point = db.GetTable<Point>();

            var nameQuery =
                from cust in point
                where cust.ID_Point == Id
                select cust;
            var res = new List<Point>();

            foreach (var category in nameQuery)
            {
                res.Add(category);
            }
            Point result = res[0];

            return result;
        }

        public Route GetRouteById(int Id)
        {
            Table<Route> route = db.GetTable<Route>();

            var nameQuery =
                from cust in route
                where cust.ID_Route == Id
                select cust;

            Route result = (Route)nameQuery;

            return result;
        }

        public int FindLastPoint(int route, string interval)
        {
            Table<Point> points = db.GetTable<Point>();

            int number = 0;
            var query29 = (from cust in points
               where cust.ID_Route == route &&
               cust.Interval == interval
               select cust);

            foreach (var category in query29)
                if (category.PointNumber > number)
                    number = category.PointNumber;

            return number;
        }

        public void Shift(int route, string interval, int min)
        {
            Table<Point> points = db.GetTable<Point>();
                        
            var query29 = (from cust in points
                           where cust.ID_Route == route &&
                           cust.Interval == interval&&
                           cust.PointNumber >= min
                           select cust);

            foreach (var category in query29)                
                category.PointNumber++;
        }
    }
}
