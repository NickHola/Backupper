using System;
using System.Windows.Threading;
using Main.Serializes;
using Main.Logs;
//ATTENZIONE: AppDomain.CurrentDomain.UnhandledException serve per catturare tutte le eccezioni non gestite (di tutti i thread), di default una volta catturata l'eccezione purtroppo l'applicazione si chiude, se nel 
//file app.config si inserisce <runtime><legacyUnhandledExceptionPolicy enabled = "1"/></runtime> allora (solamente se lanciato fuori da visual studio) l'applicazione non si chiude; ma se l'eccezione si verifica nel 
//main thread il meccanismo non funziona, quindi è stato necessario utilizzare anche Application.Current.DispatcherUnhandledException così le eccezioni del main thread vengono intercettate solo da quest'ultimo 


namespace Main
{
    public static class Excep
    {
        private const string exVolontaria = "AppCode:ThrowNewEx"; //Eccezione di tipo volontaria: generata a codice tramite Throw New Exception()

        public static string ScriviLogInEx(Mess logMess)
        {
            string testoSerial = "";
            if (Serialize.SerializeInText(logMess, ref testoSerial) == false) return logMess.testoDaLoggare; //Se fallisce nel messaggio metto il testo da loggare

            return exVolontaria + testoSerial;
        }

        public static void LeggiLogInEx(Exception ex, out Mess logMess, out bool logFound)
        {
            LeggiLogInEx(ex.Message, out logMess, out logFound, ex.GetType().ToString(), ex.StackTrace);
           
        }

        public static void LeggiLogInEx(string exMessage, out Mess logMess, out bool logFound, string exGetType = "", string exStackTrace = "")
        {
            string testoDeserial;

            logFound = false;
            logMess = null;

            if (exMessage.Left(exVolontaria.Length) == exVolontaria)
            { //Se è una eccezione di tipo volontaria allora deserializzo 
                logMess = new Mess(Tipi.Warn, "");  //Lo devo inizializzare così è noto il tipo per il deserializzatore
                testoDeserial = exMessage.Right(exVolontaria.Length * -1);
                if (Serialize.DeserializeFromText(testoDeserial, ref logMess) == true) logFound = true;
            }

            if (logFound == false) logMess = new Mess(Tipi.Warn, Log.main.warnUserText, "Ecc. non gestita, Messaggio: " + exMessage + Util.crLf + ", Tipo ecc: " + exGetType + Util.crLf + "Sub: " + exStackTrace);
        }


        public static void GestoreExNonGestiteDelMainThread(System.Object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            //System.Windows.MessageBox.Show("GestoreExNonGestiteDelMainThread"); 
            args.Handled = true;                 //Serve per non far terminare l'applicazione
            GestoreExNonGestite(args.Exception);
        }

        public static void GestoreExNonGestiteDeiThreadSecondari(System.Object sender, UnhandledExceptionEventArgs args)
        {
            //System.Windows.MessageBox.Show("GestoreExNonGestiteDeiThreadSecondari");
            GestoreExNonGestite((Exception)(args.ExceptionObject));
        }

        private static void GestoreExNonGestite(Exception ex)
        {
            try
            {
                Mess logMess;
                LeggiLogInEx(ex, out logMess, out _);
                Log.main.Add(logMess);
            }
            catch (Exception ex2)
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "Errore in sezione Catch, ex2.mess:<" + ex2.Message + ">"));
            }
        }
    }
}
