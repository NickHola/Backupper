using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Configs;
using Main.Apps;
using Main;
using Main.Logs;

namespace Backupper
{
    class BackupsConfigFile : ConfigFile
    {
        public bool IsEncrypted { get; set; }

        static BackupsConfigFile instance;

        public static BackupsConfigFile Instance
        {
            get
            {
                if (instance == null)
                    instance = new BackupsConfigFile();
                return instance;
            }
        }

        private BackupsConfigFile()
        {
            string appPath;

            Main.App.GetAppFullPath(out _, out appPath, out _, removeExe: true);
            appPath += "Backups.cfg";

            this.FullFilePath = appPath;

            IsEncrypted = false;
            this.configurations = new List<ConfigurazioneSuFile>();
        }

        public bool Load(Mess logMess = null)
        {
            instance = (BackupsConfigFile)this.LoadFromFile(out bool inErr, logMess);
            return inErr;
        }
    }
}
