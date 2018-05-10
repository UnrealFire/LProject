using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Linq.Mapping;

namespace LProject
{
    [Table(Name = "Point")]
    public class Point
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID_Point;

        [Column]
        public string OrderNumber
        {
            get;
            set;
        }

        [Column]
        public string City
        {
            get;
            set;
        }

        [Column]
        public string Street
        {
            get;
            set;
        }

        [Column]
        public string House
        {
            get;
            set;
        }

        [Column]
        public string Korp
        {
            get;
            set;
        }

        [Column]
        public string Comment
        {
            get;
            set;
        }

        [Column]
        public string Interval
        {
            get;
            set;
        }



        [Column]
        public int PointNumber
        {
            get;
            set;
        }

        [Column]
        public int PointType
        {
            get;
            set;
        }

        [Column]
        public int ID_Route
        {
            get;
            set;
        }

        [Column]
        public int ID_Session
        {
            get;
            set;
        }
        


        public Point()
        {

        }
    }
}
