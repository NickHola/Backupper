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

        private static Lazy<BackupsConfigFile> instance = new Lazy<BackupsConfigFile>(() => new BackupsConfigFile()); //Thread-safe singleton

        public static BackupsConfigFile Instance
        {
            get
            {
                return instance.Value;
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
            bool inErr = true;
            instance = new Lazy<BackupsConfigFile>(() => (BackupsConfigFile)this.LoadFromFile(out inErr, logMess));
            return inErr;
        }
    }
}
