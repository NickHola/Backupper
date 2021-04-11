using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Main.Logs;
using Main.DBs;
using Main.Roots;
using Main.Zips;
using Main.Apps;
using Main.Serializes;
using Newtonsoft.Json;
using Main.Configs;
using Main.Salvable;
using System.Text.RegularExpressions;

namespace Main
{
    public static class App
    {
        private static WndAppConfigFile wndAppConfig;
        private static Thread thrLockedQryKiller, thrAccessoRoot;
        private static bool mainInizializzato, proceduraChiusuraGiàEseguita;
        internal static AppConfigFile config;
        private static ResourceDictionary uiResource;

        public static SharedCodeWithApp SharedCodeApp { get; set; }
        public static UInt64 CurrentUserId { get; set; } = 1;
        //public static SaveLocation DefaultSaveLocation { get; set; }
        public static ResourceDictionary UIResource {
            get {
                if (uiResource == null)
                {
                    uiResource = new ResourceDictionary();
                    uiResource.Source = new Uri(@"/MainDll;component/UIRes.xaml", UriKind.Relative);
                }
                return uiResource;
            }
        } // { get; set; }
        public static Www.DownLoad Download { get; set; }
        public static Www.UpLoad Upload { get; set; }
        public static bool DebugMode { get; set; }

        public static AppConfigFile Config
        {
            get { return config; }
            set { config = value; }
        }  //Config si instanzia in Main.Initialize 
                       
        internal static DBBase Db { get; set; }
        internal static bool WndConfigAppAperto { get; set; }
        
        public static bool Initialize(SharedCodeWithApp codiceApp, string logTestoUteAvv = "", string logTestoUteErr = "")
        {
            DBBase db = null;
            Type classeDb = null;
            return Initialize(codiceApp, db, classeDb, logTestoUteAvv: logTestoUteAvv, logTestoUteErr: logTestoUteErr);
        }

        ///<param name="db">L'applicazione che userà main.dll avrà la sua variabile db e non userà DBNs.db, poichè DBNs.db è di tipo base e avrei difficoltà con l'intellisense</param>  
        public static bool Initialize(SharedCodeWithApp codiceApp, DBBase db, Type classeDb, string logTestoUteAvv = "", string logTestoUteErr = "")
        {
            //***********************************************************************************************************************************************************************************************************************
            //****************ATTENZIONE: Inserire il codice: <runtime><legacyUnhandledExceptionPolicy enabled = "1" /></runtime>  nell'App.config, vedere Eccezioni.vb per i dettagli**********************************************
            //***********************************************************************************************************************************************************************************************************************
            App.Config = new Apps.AppConfigFile(); 

            mainInizializzato = false;
            proceduraChiusuraGiàEseguita = false;

            Thread thrCheckTimeInizial = new Thread(thrCheckInitializeTime); //Non ho usato avvio download per non aspettare
            thrCheckTimeInizial.IsBackground = true;
            thrCheckTimeInizial.Start();

            if (Log.main == null) Log.Initializes(ref Log.main, warnUserText: logTestoUteAvv, errUserText: logTestoUteErr);

            AppDomain.CurrentDomain.UnhandledException += Excep.GestoreExNonGestiteDeiThreadSecondari; //Intercetta tutte le eccezioni non gestite dei thread secondari
            Application.Current.DispatcherUnhandledException += Excep.GestoreExNonGestiteDelMainThread; //Intercetta tutte le eccezioni non gestite del main thread (UI thread)

            if (codiceApp == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "ricevuto codiceApp a null"));
                return false;
            }
            App.SharedCodeApp = codiceApp;

            //if (defaultSaveLoc == null)
            //{
            //    Log.main.Add(new Mess(Tipi.ERR, "", "ricevuto defaultSaveLoc a null"));
            //    return false;
            //}

            if (App.CheckAdminPermission() == false) //Placed after App.CodiceAppCond because use it.
            {
                MessageBox.Show("The application requires administrator privileges");
                App.SharedCodeApp.ExitProcedure(saveConfigurations: false);
            }

            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "l'id del thread non è 1 richiamare Inizializza con il thread dell'UI"));
                return false;
            }

            if (dll.inizializza() == false) return false;

            //App.config.dbPrincipale.completa = "Provider=SQL Server Native Client 10.0;Server=Desktop\SQL2008R2Exp;Database=TestMain;Trusted_Connection=Yes;" 'oppure ";User Id=myUsername;Password = myPassword;"
            //App.config.SalvaSuFile()

            //App.MostraWndConfigApp()

            //Serial.Inizializza() 'Non so per quale stranissimo motivo, se lo inizializzo all'interno di "Serial" restituisce l'errore "System.IO.FileNotFoundException" dicendo che non...
            Serialize.jsonSett = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }; //...trova il file "Newtonsoft.Json.dll", 

            //while (AppConfigFile.LoadFromFile(ref App.config, out _) == false)
            App.config = (AppConfigFile)App.config.LoadFromFile(out bool inErr);
            //while (inErr == true)
            //{
            //    App.ShowWndConfigApp();
            //    App.config = (AppConfigFile)App.config.LoadFromFile(out inErr);
            //}

            if (classeDb != null)
            {
                bool esito; List<string> listaErr;

                db = (DBBase)Activator.CreateInstance(classeDb); //La New del db dell'applicativo viene fatta qui perchè devo avere la variabile LogNs.log inizializzata
                App.Db = db;
                App.Db.ConnString = App.Config.MainDbConnString;

                if (db.ControlloStrutturaDB(out esito, out listaErr) == false)
                {
                    //Log.main.Add(Tipi.ERR, Log.main.errUserText, listaErr);
                    MessageBox.Show("Controllo strutta DB fallito");
                    return false;  //qui ci va il return false perchè significa che non sono riusciuto neanche a farla la comparazione
                }

                if (esito == false)
                {
                    Log.main.Add(LogType.ERR, "", listaErr);
                    //TODO -10 open errors list window with options: try recovery or close.
                }

                thrLockedQryKiller = new Thread(DBUtil.thrCycle_LockedQryKiller); //Non ho usato avvio download per non aspettare
                thrLockedQryKiller.IsBackground = true;
                thrLockedQryKiller.Start();
            }

            App.AcquireStartupParameter(); //Deve stare prima di log.CaricaConfigAvviaThr perchè CaricaConfigAvviaThr in debugModeOn cambia comportamento


            if (Log.main.LoadConfigAndStartThread(App.Config) == false) return false; //deve stare dopo l'inizializzazione del DB e del App.config poichè normalmente caricherà le impostazioni da li
            Log.main.ShowMessageWindow();

            if (Zip.inizializza() == false) return false;

            App.Download = new Www.DownLoad("main");
            App.Upload = new Www.UpLoad("main");

            thrAccessoRoot = new Thread(Root.ThrMostraNotificaAccessoRoot); //Non ho usato avvio download per non aspettare
            thrAccessoRoot.IsBackground = true;
            thrAccessoRoot.Start();

            Application.Current.Exit += ApplicationExit;

            mainInizializzato = true;
            //TestUpload()
            return true;
        }

        private static void thrCheckInitializeTime()
        {
            while (true)
            {
                Thread.Sleep(11000);
                if (mainInizializzato == true) return;

                if (App.WndConfigAppAperto == false)
                {
                    switch (MessageBox.Show("Vuoi aprire la finestra del file di configurazione dell'app? (premere cancel per non chiedere più)", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
                    {
                        case MessageBoxResult.Yes:
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                wndAppConfig = new WndAppConfigFile();
                                wndAppConfig.ShowDialog();
                                return;
                            }));
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                }
            }
        }

        private static void ApplicationExit(object sender, ExitEventArgs e)
        {
            ClosingProcedure();
        }

        public static bool CheckAdminPermission()
        {
            string percEPrefisso = @"C:\Windows\DaCancellare_";
            string percNomefile = percEPrefisso + Str.GeneraRandom(5, Str.TipiRandom.soloLettereNumeri) + ".txt";

            try
            {
                while (File.Exists(percNomefile))
                {
                    percNomefile = percEPrefisso + Str.GeneraRandom(5, Str.TipiRandom.soloLettereNumeri) + ".txt";
                }
            }
            catch
            { return false; }

            StreamWriter file;

            try
            {
                file = new StreamWriter(percNomefile, true);
                file.Close();
            }
            catch
            { return false; }

            if (File.Exists(percNomefile))
                File.Delete(percNomefile);
            else
                return false;

            return true;
        }

        public static bool GetAppFullPath(out string fullPath, out string path, out string name, bool removeExe = false, bool removeSvhost = true)
        {
            fullPath = path = name = "";

            try
            {
                string percorsoENomeFileApp = Process.GetCurrentProcess().MainModule.FileName;
                path = Path.GetDirectoryName(percorsoENomeFileApp) + @"\";
                name = Path.GetFileName(percorsoENomeFileApp);
                if (removeSvhost == true) Regex.Replace(name, ".vshost", "", RegexOptions.IgnoreCase);
                if (removeExe == true && name.Right(4).ToLower() == ".exe") name = name.Left(-4);

                fullPath = path + name;
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Eccezione ex.mess:<" + ex.Message + ">"));
                return false;
            }
            return true;
        }

        public static void AcquireStartupParameter()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.ToLower() == "debugmodeon") DebugMode = true;
            }
        }

        public static bool ShowWndConfigApp()
        {
            bool esito;
            WndAppConfigFile wndAppConfig;

            try
            {
                WndConfigAppAperto = true;
                wndAppConfig = new WndAppConfigFile();
                wndAppConfig.ShowDialog();
                esito = true;
            }
            catch (InvalidOperationException ex)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Eccezione ex.mess:<" + ex.Message + ">"));
                esito = false;
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Eccezione ex.mess:<" + ex.Message + ">"));
                esito = false;
            }
            finally
            {
                WndConfigAppAperto = false;
            }
            return esito;
        }

        public static bool ClosingProcedure(bool salvaConfigApp = true, Int32 tSleepMs = 0)
        {

            if (proceduraChiusuraGiàEseguita == true) return true;
            proceduraChiusuraGiàEseguita = true;

            if (salvaConfigApp == true)
            {
                Log.main.config.Save(App.Config);
                Log.main.uscMess.config.Save(App.Config);
                Log.main.uscMessSoloLog.config.Save(App.Config);
                App.Download.config.Save(App.Config);
                App.Upload.config.Save(App.Config);

                App.Config.SaveOnFile(App.Config.IsEncrypted); //Non mi interessa il successo del metodo dato che se ritorna false già traccia tutto lui
            }

            if (Root.ntiRoot != null) Root.ntiRoot.Visible = false; //Rimarrebbe nell'icon tray se non la levassi io(Bug di windows)

            if (tSleepMs > 0) Thread.Sleep(tSleepMs); //Solitamente serve per dare il tempo al thread log di tracciare su file

            SharedCodeApp.ExitProcedure(saveConfigurations: salvaConfigApp);
            return true;
        }
    }
}
