using System;
using System.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Main.Logs2;
using System.IO;
using System.Threading;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using Main.MsgBxes;
using Main.Salvable;

namespace Main.Logs
{
    public class Log
    {
        #region Membri statici
        public static Log main;
        public const string testoUteAvvGenerico = "È stata generata una avvertenza";
        public const string testoUteErrGenerico = "È stato generato un errore";
        #endregion

        private readonly string name;
        private WndMessaggi wndMess, wndMessSoloLog;
        internal UscMessaggi uscMess, uscMessSoloLog;
        internal ConfigLog config;
        //private  DataTable dtOraErrThrCiclo;
        //private DateTime oraUltNotif;
        private ConcurrentQueue<Mess> logQueue = new ConcurrentQueue<Mess>();
        //Private ultMessPerThread As New Concurrent.ConcurrentDictionary(Of Int32, Mess)
        public readonly string warnUserText, errUserText;
        public readonly Int32 tStimatoPerLoggareMs;
        //log

        private Log(string name = "", string warnUserText = "", string errUserText = "") //Usare la sub inizializza per instanziare un nuovo log
        {
            this.name = (name == null || name == "") ? "main" : name;
            this.warnUserText = (warnUserText == null || warnUserText == "") ? testoUteAvvGenerico : warnUserText;
            this.errUserText = (errUserText == null || errUserText == "") ? testoUteErrGenerico : errUserText;
            this.tStimatoPerLoggareMs = 300;
        }

        public static void Initializes(ref Log oggLog, string name = "", string warnUserText = "", string errUserText = "", bool visualMess = true,
                                        bool visualMessSoloLog = true, Configs.SaveLocation saveLocation = null)
        { //ATTENZIONE non posso fare tutto dentro la new poichè ConfigLog richiama ValidazioneNs.ControlloValore che a sua volta gli serve il log inizializzato

            oggLog = new Log(name, warnUserText, errUserText);
            oggLog.config = new ConfigLog(oggLog.name);
            if (visualMess == true) oggLog.uscMess = new UscMessaggi(oggLog.name + "Mess", TipiUscMessaggi.Mess, saveLocation: saveLocation);
            if (visualMessSoloLog == true) oggLog.uscMessSoloLog = new UscMessaggi(oggLog.name + "MessSoloLog", TipiUscMessaggi.MessSoloLog, saveLocation: saveLocation);
        }

        internal bool LoadConfigAndStartThread(object SaveLocation)
        {
            Mess logMess = new Mess(Tipi.ERR, this.errUserText);

            this.config = (ConfigLog)this.config.Load(App.Config, out bool inErr, logMess: logMess);
            //Return False  //La prima volta che faccio il DB o il fle config(a seconda di dove salvo le impostazioni) ritornerà false quindi lo segnalo come errore ma non ritorno false

            if (this.uscMess != null)
                this.uscMess.config = (ConfigMess)this.uscMess.config.Load(App.Config, out inErr, logMess: logMess);

            if (this.uscMessSoloLog != null)
                this.uscMessSoloLog.config = (ConfigMess)this.uscMessSoloLog.config.Load(App.Config, out inErr, logMess: logMess);

            Thread thrGestoreNotifiche = new Thread(thrCicloMessageManager);
            thrGestoreNotifiche.IsBackground = true;
            thrGestoreNotifiche.Start();

            return true;
        }

        internal void ShowMessageWindow()
        {

            if (this.uscMess != null)
            {
                wndMess = new WndMessaggi(this.uscMess);
                //wndMess.Show();
            }

            if (this.uscMessSoloLog != null)
            {
                wndMessSoloLog = new WndMessaggi(this.uscMessSoloLog);
                //wndMessSoloLog.Show();
            }

        }

        public void Add(Mess mess)
        { //Accoda: si aggiungono messaggi tramite metodo così l'oggetto coda è isolato dall'esterno e non c'è bisogno di appendere nel testo di tracciamaneto la sub SubName
            if (mess == null) mess = new Mess(Tipi.ERR, "", "ricevuto mess a nothing");
            if (mess.oraCreazione == DateTime.MinValue) mess.oraCreazione = DateTime.Now;
            logQueue.Enqueue(mess);
        }

        public void Add(Tipi tipoLog, string userMessage, List<string> logsList)
        {
            if (logsList == null) logsList = new List<string> { "ricevuto logsList a nothing" }; //ATTENZIONE Deve essere il primo controllo

            if (userMessage == null)
            {
                userMessage = "";
                logsList = new List<string> { "ricevuto userMessage a nothing, la logsList sarà comunque tracciata" };
            }

            foreach (string log in logsList)
                Add(new Mess(tipoLog, userMessage, log));
        }

        private void thrCicloMessageManager()
        {
            Mess mess; string tmpStr = "";

            //Threading.Thread.Sleep(150) 'Non potendo mettere la creazione di questo thread dentro Inizializza(poichè shared), attendo un po' di tempo 

            while (true)
            {
                Thread.Sleep(1);

#if DEBUG == false
                    try {
#endif

                while (logQueue.Count > 0)
                {
                    if (logQueue.TryDequeue(out mess) == false) break;
                    do
                    {
                        Logger(mess);  //Prima loggo poichè se dopo il log c'è la procedura di chiusura non ho tanto tempo per loggare
                        Viewer(ref mess);

                        mess.nCicloInGestore += 1;
                        if (mess.nCicloInGestore > 200)
                        {
                            if (mess.visualiz == false) tmpStr = "visualizzare";
                            if (mess.loggato == false) tmpStr = "loggare";
                            MsgBx.Show("", "Non sono riuscito a " + tmpStr + " il mess: " + mess.testoDaVisual + " " + mess.testoDaLoggare, MsgBxPicture.Alert);

                            goto ExitWhile;
                        }
                        else if (mess.nCicloInGestore > 1)
                        {
                            Thread.Sleep(1);
                        }

                    } while (mess.loggato == false || mess.visualiz == false);
                }
            ExitWhile:;
#if DEBUG == false
                } catch (Exception ex) { 
                    //NotificaErrThrCiclo(ex, True) qui non posso poichè richiamerebbe se stesso
                }
#endif
            }
        }

        private void Logger(Mess mess)
        {
            if (mess.tipo == Tipi._Nothing) mess.loggato = true; //Si verifica nel caso in cui un metodo ne richiama un altro passondogli logMess con tipo log a nothing poichè non vuole che un eventuale errore venga loggato
            if (mess.loggato == true) return;
            StreamWriter logFile; string strInizioLog = "";

            if (File.Exists(config.DammiPercNomeFile()) == false)
            {
                if (App.SharedCodeApp != null)
                {
                    strInizioLog += App.SharedCodeApp.GetAppName();
                    strInizioLog += " Ver. " + App.SharedCodeApp.GetAppVersion();
                }
                logFile = new StreamWriter(config.DammiPercNomeFile(), true);
                logFile.WriteLine("Log starts - " + strInizioLog);
                logFile.WriteLine("yy/mm/dd hh:mm:ss.ms  LogType  UserMessage and LogMessage");
                logFile.WriteLine("");
            }
            else
            {
                logFile = new StreamWriter(this.config.DammiPercNomeFile(), true); //ATTENZIONE: non lo posso mettere prima del controllo xkè dopo sarebbe sempre (exist = true)
            }

            logFile.WriteLine(mess.oraCreazione.ToString("yy/MM/dd HH:mm:ss.fff") + "  " + mess.tipo.ToString() + "  " + mess.testoDaVisual + " " + mess.testoDaLoggare);
            logFile.Close();

            mess.loggato = true;

            if (new FileInfo(config.DammiPercNomeFile()).Length > config.DimFile)
            {

                string logDaCanc, logDaRinom, suffissoDest;

                for (int i = (config.NumFile - 1); i <= 1; i -= 1)
                {

                    if (i == 1)
                        logDaRinom = config.DammiPercNomeFile();
                    else
                        logDaRinom = config.DammiPercNomeFile(conEsten: false) + "_" + i + config.EstensioneFile;

                    suffissoDest = "_" + (i + 1);

                    logDaCanc = config.DammiPercNomeFile(conEsten: false) + suffissoDest + config.EstensioneFile;

                    if (File.Exists(logDaCanc) == true) FileSystem.DeleteFile(logDaCanc, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);

                    if (File.Exists(logDaRinom) == true) FileSystem.RenameFile(logDaRinom, config.DammiPercNomeFile(conPerc: false, conEsten: false) + suffissoDest + config.EstensioneFile);
                }
            }
        }

        private void Viewer(ref Mess mess)
        {
            if (mess.tipo == Tipi._Nothing) mess.visualiz = true; //Si verifica nel caso in cui un metodo ne richiama un altro passondogli logMess con tipo log a nothing poichè non vuole che un eventuale errore venga loggato
            if (App.DebugMode == true) mess.visualiz = true;
            if (mess.visualiz == true) return;

            bool esito = true;

            if (uscMess != null && uscMess.accoda(mess) == false) esito = false;
            if (uscMessSoloLog != null && uscMessSoloLog.accoda(mess) == false) esito = false;

            mess.visualiz = esito;
        }
    }
}
