using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace LProject
{
    [Table(Name = "dbo.People")]
    class Man
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID_People;

        [Column]
        public string PersonName
        {
            get;
            set;
        }

        [Column]
        public string Pass
        {
            get;
            set;
        }

        [Column]
        public bool IsAdmin
        {
            get;
            set;
        }

        public Man()
        {

        }
    }
}
