using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace LProject
{
    [Table(Name = "Session")]
    class Session
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID_Session;

        [Column]
        public bool IsArchived
        {
            get;
            set;
        }


        [Column]
        public string Session_Name
        {
            get;
            set;
        }

        [Column]
        public int ID_People
        {
            get;
            set;
        }

        [Column]
        public DateTime ArchTime
        {
            get;
            set;
        }

        [Column]
        public DateTime CreationDate
        {
            get;
            set;
        }
    }
}
