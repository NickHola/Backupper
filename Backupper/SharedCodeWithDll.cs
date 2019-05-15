using System;
using System.Diagnostics;
using System.Reflection;
using Main.Apps;
using Main;

namespace Backupper
{
    public class SharedCodeWithDll : SharedCodeWithApp
    {
        public override string GetAppName()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
        }

        public override string GetAppVersion()
        {
            //Dim assemblyVersion As String = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }

        public override void ExitProcedure(bool saveConfigurations)
        {
            if (saveConfigurations == true)
            {
                SettingsM.Instance.Save(Main.App.Config);
                Main.App.Config.SaveOnFile(false);
            }
            Environment.Exit(0);
        }
    }
}
