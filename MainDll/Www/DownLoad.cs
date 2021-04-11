using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using Main.DataOre;
using Main.Concurs;
using Main.Logs;
using Main.Thrs;
using Main.Salvable;


namespace Main.Www
{
    public class DownLoad : OperazioneBase
    {
        ConcurrentDictionary<UInt64, DownloadItem> queue;
        ConcurrentQueue<DownloadItem> inputBuffer; //Serve poichè se più thread chiamano contemporaneamente il metodo Add succede che si inserisce nella queue 2 o più DownloadItem con la stessa Key poichè chiamata in contemporanea
        internal ConfigDownload config;
      

        internal DownLoad(string nome, object configSource = null) : base(nome)
        {
            queue = new ConcurrentDictionary<UInt64, DownloadItem>();
            inputBuffer = new ConcurrentQueue<DownloadItem>();
            this.config = new ConfigDownload(this.nome);
            //this.config = (ConfigDownload)Savable.Load(this.config);
            if (configSource != null) this.config = (ConfigDownload)this.config.Load(configSource, out _);

            thrScoda = Thr.AvviaNuovo(() => ThrCicloReadInputBufferAndElaborateMainQueue()); //ATTENZIONE deve stare dopo l'inizializzazione di queue
        }

        public void Add(DownloadItem elemDownload)
        {
            if (elemDownload == null)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto elemDownload a nothing"));
                return;
            }

            if (elemDownload.TimeoutSec == 0) elemDownload.TimeoutSec = Convert.ToInt16(this.config.DefaultTimeoutSec);

            inputBuffer.Enqueue(elemDownload);
        }

        protected override void ThrCicloReadInputBufferAndElaborateMainQueue()
        {
            Thr.SbloccaThrPadre();
            DateTime timeoutStart;
            //UInt64 indiceProssElem;
            IOrderedEnumerable<KeyValuePair<UInt64, DownloadItem>> elementiDaDwl;
            DownloadItem elemDownload = null;

            //indiceProssElem = 1;

#if DEBUG == false 
            try {
#endif
            while (true)
            {
                Thread.Sleep(9);

                while (inputBuffer.Count > 0)
                {
                    if (Concur.Queue_TryDequeue(inputBuffer, ref elemDownload) == false)
                    {
                        break;
                    }
                    idItem += 1;
                    elemDownload.IdItemInList = idItem;
                    Concur.Dictionary_TryAddOrUpdate(queue, new KeyValuePair<UInt64, DownloadItem>(idItem, elemDownload), noUpadate: true);
                }

                ControlloElementiEStatistiche();

                ItemInProcessing = (from tmp in this.queue where tmp.Value.DownloadState == DwlItemState.DwlAvviato select tmp).Count();

                if (ItemInProcessing >= this.config.MaxParallelStreams) continue;

                if (this.raggiuntoMaxKBSec) continue;

                elementiDaDwl = from tmp in this.queue where tmp.Value.DownloadState == DwlItemState.Iniziale orderby tmp.Value.Priority ascending, tmp.Key ascending select tmp; //A parità di priority prendo il più vecchio in coda

                ItemToProcess = elementiDaDwl.Count();

                if (ItemToProcess == 0) continue;

                elemDownload = elementiDaDwl.ElementAt(0).Value; //Serve poichè gli elementi della LinQ si aggiornano all'istante quando un parametro della where o della orderby cambiano

                Thread thrDL = Thr.AvviaNuovo(() => ThrAvvioDownLoad(elementiDaDwl.ElementAt(0).Key));

                timeoutStart = DateTime.Now;

                while (true) //Check operation start on item
                {
                    if (elemDownload.DownloadState != DwlItemState.Iniziale) break; //operation started 
                    if ((DateTime.Now - timeoutStart).TotalSeconds > timeoutToStartSec)
                    {
                        elemDownload.DescErr = "TimeoutToStart di " + timeoutToStartSec + " secondi raggiunto, url:<" + elemDownload.url + ">";
                        Log.main.Add(new Mess(elemDownload.tipoLogTimeout, "", "per l'url:<" + elemDownload.url + ">, " + elemDownload.DescErr));
                        elemDownload.DownloadState = DwlItemState.TimeoutToStart;
                        break;
                    }
                    Thread.Sleep(1);
                }

                Thread.Sleep(config.SleepAfterOneOperationMs);

                //while (elemDownload.DownloadState == DwlItemState.DwlAvviato)
                //{
                //    Thread.Sleep(1);
                //}
                //indiceProssElem = elementiDaDwl.ElementAt(0).Key + 1;

            }
#if DEBUG == false
            } catch (Exception ex) { 
                Thr.NotificaErrThrCiclo(ex, true);
            }
#endif
        }

        private void ThrAvvioDownLoad(UInt64 itemKey)
        {
            Thr.SbloccaThrPadre();

            Thread thrDL;
            thrDL = Thr.AvviaNuovo(() => ThrDownload(itemKey));

            DownloadItem elemDownload = null;
            if (Concur.Dictionary_TryGet(this.queue, itemKey, ref elemDownload, out _) == false)
                return;

            if (Thr.AttesaCompletamento(ref thrDL, elemDownload.TimeoutSec == -1 ? -1 : elemDownload.TimeoutSec * 1000) == false)
            { //TimeOut
                elemDownload.DescErr = "Timeout:<" + elemDownload.TimeoutSec + "> secondi, raggiunto";
                elemDownload.DownloadState = DwlItemState.Timeout;

                Log.main.Add(new Mess(elemDownload.tipoLogTimeout, "", "per l'url:<" + elemDownload.url + ">, " + elemDownload.DescErr));
            }
        }

        private void ThrDownload(UInt64 itemKey)
        {
            Thr.SbloccaThrPadre();
             DateTime tmpAttesa;
            WebClient client = new WebClient();
 
            //client = new WebClient(); //TODO decommentare //, flusso As New IO.MemoryStream
            DownloadItem elemDownload = null;

            try
            {
                if (Concur.Dictionary_TryGet(this.queue, itemKey, ref elemDownload, out _) == false)
                    return;

                client.DownloadDataCompleted += DownloadTerminato;
                client.DownloadProgressChanged += PercentualeDownload;

                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)"); //Aggiungo lo user agent nell'header nel caso la richiesta contiene una query.
                elemDownload.OperationStartDate = DateTime.Now;
                elemDownload.DownloadState = DwlItemState.DwlAvviato;

                client.DownloadDataAsync(new Uri(elemDownload.url), elemDownload);
                //client.DownloadData() //Non scatena gli eventi DownloadProgressChanged e DownloadDataCompleted

                //AttesaBoolean(client.IsBusy, -1) //Per le proprità(come IsBusy) se cambia di valore sembra che non lo sente quindi non posso utilizzare AttesaBoolean
                while (client.IsBusy)
                    Thread.Sleep(1);

                tmpAttesa = DateTime.Now;

                while (elemDownload.DownloadState == DwlItemState.DwlAvviato)
                {
                    if (DataOra.AttesaTempo(ref tmpAttesa, 1000) == true)
                    {
                        Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Da quando client non è Busy, lo statoDownload non si è aggiornato in tempo utile, elemDownload.url:<" + elemDownload.url + ">", visualMsgBox: false));
                        elemDownload.DownloadState = DwlItemState.Eccezione;
                        break;
                    }
                }
  
                //client.Dispose();

            }
            catch (ThreadAbortException ex)  //Thread interrupted by abort, do nothing
            { var tmp = ex; }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(elemDownload.tipoLogEccezione, "", "Eccezione per l'url:<" + elemDownload.url + ">, ex.mess:<" + ex.Message + ">"));
                elemDownload.DescErr = ex.Message;
                elemDownload.DownloadState = DwlItemState.Eccezione;
            }
            finally
            {
                //client.CancelAsync(); //TODO verificare se è la causa dell'errore  sul sito investing 

                client.CancelAsync();
                client.Dispose();
            }
        }

        private void DownloadTerminato(Object sender, DownloadDataCompletedEventArgs e)
        {
            //Dim client As WebClient = sender
            string messToLog = "";
            DownloadItem elem = null;
            //ATTENZIONE: messo statoDownload = Timeout e = Eccezione poichè l'evento si scatena anche in questi case ed e.Result è in uno stato di errore ma non Nothing quindi darebbe eccezione 
            //If elem.statoDownload = StatiElemDwl.Timeout OrElse elem.statoDownload = StatiElemDwl.Eccezione Then Exit Sub
            try
            {
                elem = (DownloadItem)e.UserState;
                if (e.Result == null) return;

                if (elem.DownloadState == DwlItemState.DwlAvviato)
                {
                    elem.Data = e.Result;
                    if (elem.translateDataInText == true) elem.TextDownloaded = Encoding.ASCII.GetString(elem.Data);
                        elem.DownloadState = DwlItemState.DwlCompletato;
                        elem.progressione.IsComplete = true;
                }
            }
            catch (TargetInvocationException ex)
            { //Si verifica quando il threadDownload viene stoppato mentre si sta ancora effettuando il download
                if (elem.DownloadState == DwlItemState.DwlAvviato) elem.DownloadState = DwlItemState.Eccezione;
                messToLog = "TargetInvocationException, ex.mess:<" + ex.Message + ">";
                if (ex.InnerException != null) messToLog += ", inner ex.mess:<" + ex.InnerException.Message + ">";
                Log.main.Add(new Mess(LogType.Warn, "", messToLog));
                return;

            }
            catch (Exception ex)
            {
                if (elem.DownloadState == DwlItemState.DwlAvviato) elem.DownloadState = DwlItemState.Eccezione;
                messToLog = "Eccezione ex.mess:<" + ex.Message + ">";
                if (ex.InnerException != null) messToLog += ", inner ex.mess:<" + ex.InnerException.Message + ">";
                Log.main.Add(new Mess(LogType.Warn, "", messToLog));
                return;
            }
        }

        private void PercentualeDownload(Object sender, DownloadProgressChangedEventArgs e)
        {
            if (e == null) return;

            DownloadItem elem = (DownloadItem)e.UserState;

            elem.progressione.Percentage = e.ProgressPercentage;
        }

        private void ControlloElementiEStatistiche()
        {
            Int16 elemDaElim; UInt64 totalizzatoreKBSec; bool raggiuntoMaxKBSecTmp, elementoDiInteresse;
            IOrderedEnumerable<KeyValuePair<UInt64, DownloadItem>> chiaviValori;

            raggiuntoMaxKBSecTmp = false;
            totalizzatoreKBSec = 0;

            chiaviValori = from c in this.queue
                           where c.Value.DownloadState == DwlItemState.DwlCompletato ||
                                        c.Value.DownloadState == DwlItemState.TimeoutToStart ||
                                        c.Value.DownloadState == DwlItemState.Timeout ||
                                        c.Value.DownloadState == DwlItemState.Eccezione
                           orderby c.Key ascending
                           select c;

            elemDaElim = Convert.ToInt16(chiaviValori.Count() - this.config.MaxItemInStatisticsQueue);

            if (elemDaElim < 0) elemDaElim = 0;

            //For Each chiaveValore In From c In Me.codaStatistiche  'Può dare errori se gli si modifica il numero degli elementi del dictionary

            foreach (KeyValuePair<UInt64, DownloadItem> chiaveValore in chiaviValori)
            {

                elementoDiInteresse = false;

                CheckDownloadSubsetEnded(chiaveValore, ref elementoDiInteresse);

                if (raggiuntoMaxKBSecTmp == false && this.config.MaxKBSec > 0) ControlloKBSec(chiaveValore, ref totalizzatoreKBSec, ref raggiuntoMaxKBSecTmp, ref elementoDiInteresse);

                if (elemDaElim > 0 && elementoDiInteresse == false)
                {
                    Concur.Dictionary_TryRemove(this.queue, chiaveValore.Key);
                    elemDaElim -= 1;
                    continue;
                }
            }
            this.raggiuntoMaxKBSec = raggiuntoMaxKBSecTmp;
        }

        private void CheckDownloadSubsetEnded(KeyValuePair<UInt64, DownloadItem> chiaveValore, ref bool elementoDiInteresse)
        {
            if (chiaveValore.Value.idSubsetInList == "" || chiaveValore.Value.IsOperationListEnded == true) return;

            elementoDiInteresse = true;

            var chiaviValoriConIdLista = from c in this.queue where c.Value.idSubsetInList == chiaveValore.Value.idSubsetInList select c;

            if ((from c in chiaviValoriConIdLista
                 where c.Value.DownloadState == DwlItemState.Iniziale || //Significa che tutti i download con quel idListaDwl sono terminati
                      c.Value.DownloadState == DwlItemState.DwlAvviato
                 select c).Count() == 0)
            {
                foreach (var tmpChiaveValore in chiaviValoriConIdLista)
                {
                    tmpChiaveValore.Value.RaiseEvent_SubsetOperationEnded();
                }
                elementoDiInteresse = false; //Lo rimetto false dato che ormai ho lanciato gli eventi
            }


        }

        private void ControlloKBSec(KeyValuePair<UInt64, DownloadItem> chiaveValore, ref UInt64 totalizzatoreKBSec, ref bool raggiuntoMaxKBSecTmp, ref bool elementoDiInteresse)
        {
            DateTime ora = DateTime.Now;
            if ((ora - chiaveValore.Value.OperationFinishDate).TotalSeconds > this.config.CheckKBSecInterval) return;

            elementoDiInteresse = true;

            if (chiaveValore.Value.DownloadState != DwlItemState.DwlCompletato) return;

            totalizzatoreKBSec += Convert.ToUInt64((chiaveValore.Value.Data.Length / 1024));

            if ((totalizzatoreKBSec / this.config.CheckKBSecInterval) > this.config.MaxKBSec) raggiuntoMaxKBSecTmp = true;
        }

    }
}
