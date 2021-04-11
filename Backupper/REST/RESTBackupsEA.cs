using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class RESTBackupsEA : EventArgs
    {
        public string backupsSerialization;

        public RESTBackupsEA(string backupsSerialization)
        {
            this.backupsSerialization = backupsSerialization;
        }
    }
}
