using System;
using Main.Configs;
using System.Collections.Generic;
using Main.DBs;

namespace Main.Apps
{
    [Serializable] public class AppConfigFile : ConfigFile
    {
       public StrConnection MainDbConnString { get; set; }
       public bool IsEncrypted { get; set; }

        public AppConfigFile() {
            string appFullPath;

            App.GetAppFullPath(out appFullPath, out _, out _, removeExe: true);
            //if (appFullPath.Right(4).ToLower() == ".exe") appFullPath = appFullPath.Left(-4);
            appFullPath += ".cfg";

            this.FullFilePath = appFullPath;


            MainDbConnString = new StrConnection();
            IsEncrypted = false;
            this.configurations = new List<Configs.ConfigurazioneSuFile>(); //Lasciare il value di tipo object altrimenti il serializzatore dice che non riesce ad instanziare Configs poichè MustInherits
        }
    }
}
