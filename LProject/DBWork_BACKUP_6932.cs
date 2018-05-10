using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace LProject
{
    public struct PointStruct
    {
        public string city;
        public string street;
        public int house;
        public int korp;
        public string interval;
        public int routeId;
        public int pointType;
        public int pointId;
    }
    class DBWork
    {
        private DataContext db;

        public DBWork()
        {
<<<<<<< HEAD
            db = new DataContext("Data Source=DESKTOP-U1V4NET\\SQLEXPRESS;Initial Catalog=LProject;Integrated Security=True;Pooling=False");
=======
            db = new DataContext("Data Source=DESKTOP-KQI339F\\SQLEXPRESS;Initial Catalog=LProject;Integrated Security=True;Pooling=False");
>>>>>>> ad0e421c67cdcbeabb42370cd7079848b51796f4

            // Attach the log to show generated SQL.
            db.Log = Console.Out;
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
            newPoint.RouteType = type;

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
        public void InsertPoint(string city, string street, int house, int korp, string inter, int number, int type, int ses_id, int route)
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
            newPoint.RouteType = type;
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




        public List<PointStruct> getPointBySession(int sessionID)
        {
            Table<Point> point = db.GetTable<Point>();

            Table<Route> routeDB = db.GetTable<Route>();

            var res = point.Where(f => f.ID_Session == sessionID).Select(data =>
                new PointStruct {
                    city = data.City,
                    street = data.Street,
                    house = data.House,
                    korp = data.Korp,
                    interval = data.Interval,
                    routeId = data.ID_Route,
                    pointType = data.RouteType,
                    pointId = data.ID_Point
                }).Distinct().ToArray();

            var result = new List<PointStruct>();

            foreach (var category in res)
            {
                result.Add((PointStruct)category);
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
        public void ChangePoint(int id, int route, string interval, int number, int type, int sesId)
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
            query.RouteType = type;

            db.SubmitChanges();
        }

    }
}
