using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.SQLes
{
    internal class ConfigSql
    {
        internal Int32 timeOutQuery, timeOutNonQuery; 

        public ConfigSql() {
            timeOutQuery = -1;
            timeOutNonQuery = -1;
       }
    }
}
