using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Linq.Mapping;

namespace LProject
{
    [Table(Name = "Route")]
    public class Route
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID_Route;

        [Column]
        public int RouteNumber
        {
            get;
            set;
        }

        [Column]
        public string Color
        {
            get;
            set;
        }

        public Route()
        {

        }

        [Column]
        public int ID_Session
        {
            get;
            set;
        }
    }
}
