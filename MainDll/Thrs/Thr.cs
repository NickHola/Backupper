using System;
using System.Data;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Main.DataOre;
using Main.SQLes;
using Main.Concurs;
using Main.Logs;
using System.Reflection;

//ATTENZIONE: se si usa un metodo anonimo(lambda) e si passano dei parametri al metodo, tali parametri non subito passati e quindi se successivamente allo start del thr si modificano, il thr li riceve modificati
//Dai test fatti risulta: La quantità di parametri sembra che non inficia, con l'atom lo sleep(2) sembra che funzioni sempre con un 13 parametri di cui 1 semplice come sringa, IsBackground e SetApartmentState sembra che non inficiano il risultato
//Quindi invece che mettere una sleep (dai test sembra che 30ms erano sufficienti) si userà l'AutoResetEvent che blocca il thr padre finchè il thr figlio non invia un messaggio di sblocco

namespace Main.Thrs
{
    public static class Thr
    {
        private static DataTable errThrCiclo = new DataTable();
        public static ConcurrentDictionary<Int32, AutoResetEvent> listaAttesaThr = new ConcurrentDictionary<Int32, AutoResetEvent>(); //AutoResetEvent leggi sopra

        public static Thread UIThread
        {
            get { return System.Windows.Application.Current.Dispatcher.Thread; }
        }

        public static System.Windows.Threading.Dispatcher UIDispatcher
        {
            get { return System.Windows.Application.Current.Dispatcher; }
        }

        public static Thread AvviaNuovo(ThreadStart start, ApartmentState apartment = ApartmentState.STA)
        {
            AutoResetEvent segnaleAttesa = new AutoResetEvent(false);
            Thread thr = new Thread(start);
            thr.SetApartmentState(apartment);
            thr.IsBackground = true;

            Concur.Dictionary_TryAddOrUpdate(listaAttesaThr, new KeyValuePair<Int32, AutoResetEvent>(thr.ManagedThreadId, segnaleAttesa), noUpadate: true);
            thr.Start();

            if (segnaleAttesa.WaitOne(1000) == false)
            { //Significa che è andato in timeout e quindi il thread chiamato non ha eseguito la set sull'AutoResetEvent contenuto nella listaAttesaThr
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "Il thr figlio non ha eseguito la .set() sull'AutoResetEvent contenuto nella listaAttesaThr"));
            }
            return thr;
        }

        public static void SbloccaThrPadre()
        {
            var idThrFiglio = Thread.CurrentThread.ManagedThreadId;
            if (listaAttesaThr.ContainsKey(idThrFiglio) == false)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "Nella listaAttesaThr non c'è il seguente thrId:<" + idThrFiglio + ">"));
                return;
            }

            listaAttesaThr[idThrFiglio].Set(); //Sblocca il ThrPadre
            Concur.Dictionary_TryRemove(listaAttesaThr, idThrFiglio);
        }

     
        ///<summary>La si usa al posto di thread.Join poichè quest'ultima blocca l'interfaccia grafica, mentre qua si usa il DoEvents</summary>
        ///<param name="timeOutMs">Se -1 attesa infinita</param>  
        public static bool AttesaCompletamento(ref Thread thread, int timeOutMs)
        {
            DateTime oraInizio = DateTime.MinValue; bool esito;

            if (Thread.CurrentThread.ManagedThreadId == 1)
            { //Se il thread è quello dell'ui allora devo per forza fare il DoEvents per non bloccare l'interfaccia ma bloccare l'esecuzione del codice.
                esito = true;
                if (timeOutMs > -1) { oraInizio = DateTime.Now; }

                while (thread.IsAlive)
                {
                    Util.DoEvents();
                    if (timeOutMs != -1 && DataOra.AttesaTempo(ref oraInizio, (UInt64)timeOutMs) == true)
                    { esito = false; }
                }
            }
            else
            { esito = thread.Join(timeOutMs); } //thread.Join ritorna true se il thr completa l'esecuzione entro il tempo previsto, se timeout = -1 attesa infinita

            if (thread.IsAlive) thread.Abort();

            return esito;
        }

        public static bool FiltraEccezioniThreadNonCiclanti(Exception ex)
        {
            string mess, tipo;
            mess = ex.Message;
            tipo = ex.GetType().ToString();

            if (mess.Contains("Thread interrotto") && tipo.Contains("ThreadAbortException")) return true; //mess:<Thread interrotto.>, tipo:<System.Threading.ThreadAbortException>

            return false;
        }

        public static void NotificaErrThrCiclo(Exception ex, bool visualLog)
        { //Tale metodo mi permette di bloccare la segnalazione di continue notifiche create da thread con ciclo infinito che vanno continuamente in eccezione

            if (errThrCiclo.Columns.Count == 0)
            {
                errThrCiclo.Columns.Add("nomeThr", typeof(String));
                errThrCiclo.Columns.Add("data", typeof(DateTime));
            }

            string nomeThrCiclo, testeUteLog; DataRow[] rigaDate;
            testeUteLog = Log.main.warnUserText;
            //Non posso ricercare quelli che iniziano per 'thrciclo' poichè l'offuscatore gli cambia nome quindi presumo che l'eccezione arrivi dal thrCiclo, contrariamente se l'eccezione arriva da un metodo richiamato dal... 
            nomeThrCiclo = Util.GetCallStack(dammiSoloSubLiv: 2); //...thrCiclo, inevitabilmente prendere il nome sbagliato 

            rigaDate = errThrCiclo.Select("nomeThr='" + nomeThrCiclo + "'");

            if (rigaDate.Length > 0)
            {
                if ((DateTime.Now - ((DateTime)rigaDate[0]["data"])).TotalSeconds < 20)
                {
                    return; //Se già ho visualizzato una eccezione per questo ciclo meno di 20 sec. fa non rivisualizzo
                }
                else
                {
                    rigaDate[0]["data"] = DateTime.Now;
                }
            }
            else
            {
                errThrCiclo.Rows.Add(nomeThrCiclo, DateTime.Now);
            }

            if (SqlObj.FiltraEccezioniQuery(ex) == true) visualLog = false;

            if (visualLog == false) testeUteLog = "";

            Log.main.Add(new Mess(Tipi.Warn, testeUteLog, "Eccezione in thrCiclo:<" + nomeThrCiclo + ">, ex.mess:<" + ex.Message + ">, ex.type:<" + ex.GetType().ToString() + ">"));

        }
    }
}
