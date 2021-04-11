using System;
using System.Threading.Tasks;
using System.Windows;
using MainDll = Main;
using Main.Logs;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SharedCodeWithDll sharedCode = new SharedCodeWithDll();

            MainDll.App.Initialize(sharedCode);

            SettingsM.Instance.Load(MainDll.App.Config);

            switch (SettingsM.Instance.SaveBackupIn)
            {
                case BackupsSaveLocation.File:
                    BackupsConfigFile.Instance.Load();
                    BackupsM.Instance.Load(BackupsConfigFile.Instance);
                    break;

                case BackupsSaveLocation.RESTService:
                    RESTBackups restBackups = new RESTBackups();
                    Task<string> task = restBackups.GetBackupsAsync();
                    try
                    {
                        task.Wait();
                        BackupsM.Instance.Load(task.Result);
                    }
                    catch (Exception ex)
                    {
                        string innerEx = ex.InnerException != null ? " inner ex:<" + ex.InnerException.Message + ">" : "" ;
                        Log.main.Add(new Mess(LogType.ERR, "", "GetBackupsAsync return exception:<" + ex.Message + ">" + innerEx));
                    }
                    break;

                case BackupsSaveLocation.Db:
                    break;
                default:
                    Log.main.Add(new Mess(LogType.ERR, "", "Received unexpected value for SettingsM.Instance.SaveBackupIn:<" + SettingsM.Instance.SaveBackupIn.ToString() + ">"));
                    break;
            }
            var wndMain = new WndMainV();
            wndMain.Show();
        }
    }
}
