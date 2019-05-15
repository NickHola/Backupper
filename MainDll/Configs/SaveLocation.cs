using Main.DBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Logs;

namespace Main.Configs
{
    public class SaveLocation
    {
        public Type DestinationType { get; private set; }

        public ConfigFile ConfigFile { get; set; }
        public DBBase Db { get; set; }

         public SaveLocation(ConfigFile destination)
        {
            this.ConfigFile = destination; //even if null
            DestinationType = typeof(ConfigFile);
        }

        public SaveLocation(DBBase destination)
        {
            this.Db = destination; //even if null
            DestinationType = typeof(DBBase);
        }
    }
}
