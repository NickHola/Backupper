using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Main.Logs;
using Main.Thrs;
using Main.DataOre;
using Main.Concurs;
using Main.Salvable;

namespace Main.Www
{
    public class UpLoad : OperazioneBase
    {
        private ConcurrentDictionary<UInt64, UploadItem> queue;
        internal ConfigUpload config;

        internal UpLoad(string nome, object configSource = null) : base(nome)
        {
            queue = new ConcurrentDictionary<UInt64, UploadItem>();
            this.config = new ConfigUpload(this.nome);
            //this.config = (ConfigUpload)Savable.Load(this.config);
            if (configSource != null) this.config = (ConfigUpload)this.config.Load(configSource, out _);

            thrScoda = Thr.AvviaNuovo(() => ThrCicloReadInputBufferAndElaborateMainQueue()); //ATTENZIONE deve stare dopo l'inizializzazione di coda
        }

        public void Add(UploadItem elemUpload)
        {
            if (elemUpload == null)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Ricevuto elemUpload a nothing"));
                return;
            }

            if (elemUpload.TimeoutSec == 0) elemUpload.TimeoutSec = Convert.ToInt16(this.config.DefaultTimeoutSec);

            idItem += 1;
            elemUpload.IdItemInList = idItem;
            Concur.Dictionary_TryAddOrUpdate(queue, new KeyValuePair<UInt64, UploadItem>(idItem, elemUpload), noUpadate: true);
        }

        protected override void ThrCicloReadInputBufferAndElaborateMainQueue()
        {
            Thr.SbloccaThrPadre();

            Thread thrAvvioUL; Int32 numElemInUpl;
            DateTime timeoutStart; //UInt64 indiceProssElem;
            IOrderedEnumerable<KeyValuePair<UInt64, UploadItem>> elementiDaUpl;

            //indiceProssElem = 1;
#if DEBUG == false
            try
            {
#endif
            while (true)
            {
                Thread.Sleep(9);

                ControlloElementiEStatistiche();

                numElemInUpl = (from tmp in this.queue where tmp.Value.UploadState == UplItemState.UplAvviato select tmp).Count();

                if (numElemInUpl >= this.config.MaxParallelStreams) continue;

                if (this.raggiuntoMaxKBSec) continue;

                elementiDaUpl = from tmp in this.queue where tmp.Value.UploadState == UplItemState.Iniziale orderby tmp.Value.Priority ascending, tmp.Key ascending select tmp; //ho messo il >= indiceProssElem poichè se dovesse succedere che il metodo Accoda salta una chiave, 
                                                                                                                                                                               //questo thread non si incanterà, esiste anche il metodo first e firstOrDefault, il primo da eccezione se la query linq non restituisce elementi, nel secondo per il parametro default non si può usare nothing
                if (elementiDaUpl.Count() == 0) continue;

                thrAvvioUL = Thr.AvviaNuovo(() => ThrAvvioUpload(elementiDaUpl.ElementAt(0).Value));
                               
                timeoutStart = DateTime.Now;
                while (true) //Check operation start on item
                {
                    if (elementiDaUpl.ElementAt(0).Value.UploadState != UplItemState.Iniziale) break; //operation started
                    if ((DateTime.Now - timeoutStart).TotalSeconds > timeoutToStartSec)
                    {
                        elementiDaUpl.ElementAt(0).Value.DescErr = "TimeoutToStart di " + timeoutToStartSec + " secondi raggiunto, url:<" + elementiDaUpl.ElementAt(0).Value.url + ">";
                        Log.main.Add(new Mess(elementiDaUpl.ElementAt(0).Value.tipoLogTimeout, "", "per l'url:<" + elementiDaUpl.ElementAt(0).Value.url + ">, " + elementiDaUpl.ElementAt(0).Value.DescErr));
                        elementiDaUpl.ElementAt(0).Value.UploadState = UplItemState.TimeoutToStart;
                        break;
                    }
                    Thread.Sleep(1);
                }

                //indiceProssElem = elementiDaUpl.ElementAt(0).Key + 1;

            }
#if DEBUG == false
            } catch (Exception ex){  
                        Thr.NotificaErrThrCiclo(ex, true);
            }
#endif

        }

        private void ThrAvvioUpload(UploadItem elemUpload)
        {

            Thr.SbloccaThrPadre();

            Thread thrDL;
            thrDL = Thr.AvviaNuovo(() => ThrUpload(ref elemUpload));

            if (Thr.AttesaCompletamento(ref thrDL, (elemUpload.TimeoutSec == -1 ? -1 : elemUpload.TimeoutSec * 1000)) == false)
            { //TimeOut
                elemUpload.DescErr = "Timeout:<" + elemUpload.TimeoutSec + "> secondi, raggiunto";
                elemUpload.UploadState = UplItemState.Timeout;

                Log.main.Add(new Mess(elemUpload.tipoLogTimeout, "", "per l'url:<" + elemUpload.url + ">, " + elemUpload.DescErr));
            }

        }

        private void ThrUpload(ref UploadItem elemUpload)
        {
            Thr.SbloccaThrPadre();
            WebClient client; string suffFileCorrotto; Uri fullUrlIniziale; DateTime tmpAttesa;
            FtpWebRequest ftpWebRequest; FtpWebResponse ftpResponse; FtpStatusCode ftpStatusCode;
            suffFileCorrotto = "";
            client = new WebClient(); //, flusso As New IO.MemoryStream

            try
            {
                client.UploadDataCompleted += UploadTerminato;
                client.UploadProgressChanged += PercentualeUpload;

                //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)") 'Aggiungo lo user agent nell'header nel caso la richiesta contiene una query.

                client.Credentials = DammiCredenziali(ref elemUpload);

                elemUpload.OperationStartDate = DateTime.Now;
                elemUpload.UploadState = UplItemState.UplAvviato;

                if (elemUpload.UploadType == UploadType.Ftp)
                {
                    if (elemUpload.CorruptFileNameSuffix != "")
                        suffFileCorrotto = elemUpload.CorruptFileNameSuffix;
                    else
                        suffFileCorrotto = config.suffFileCorrotto;
                }

                fullUrlIniziale = new Uri(Path.Combine(elemUpload.url, elemUpload.FileName) + suffFileCorrotto);

                client.UploadDataAsync(fullUrlIniziale, null, elemUpload.Data, elemUpload);
                //client.UploadData(fullUrl, elemUpload.dati)  'Non scatena glie eventi UploadProgressChanged e UploadDataCompleted

                //AttesaBoolean(client.IsBusy, -1) 'Per le proprità(come IsBusy) se cambia di valore sembra che non lo sente quindi non posso utilizzare AttesaBoolean
                while (client.IsBusy)
                {
                    Thread.Sleep(1);
                }

                tmpAttesa = DateTime.Now;

                while (elemUpload.UploadState == UplItemState.UplAvviato)
                {
                    if (DataOra.AttesaTempo(ref tmpAttesa, 1000) == true)
                    {
                        Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Da quando client non è Busy, lo statoUpload non si è aggiornato in tempo utile, fullUrlIniziale:<" + fullUrlIniziale.OriginalString + ">", visualMsgBox: false));
                        elemUpload.UploadState = UplItemState.Eccezione;
                        client.Dispose();
                        break;
                    }
                }

                client.Dispose();

                if (elemUpload.UploadType == UploadType.Ftp)
                {
                    ftpWebRequest = (FtpWebRequest)System.Net.FtpWebRequest.Create(fullUrlIniziale);
                    ftpWebRequest.Credentials = DammiCredenziali(ref elemUpload);
                    ftpWebRequest.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                    //MyFtpWebRequest.RenameTo() = nomeFile

                    ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                    if (elemUpload.Data.Length != ftpResponse.ContentLength)
                    {
                        Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "Rilevate dimensioni differenti tra dati da inviare e dati inviati, elemUpload.dati.Length:<" + elemUpload.Data.Length + ">, ftpResponse.ContentLength:<" + ftpResponse.ContentLength + ">, fullUrlIniziale:<" + fullUrlIniziale.OriginalString + ">"));
                        ftpResponse.Close();
                        return;
                    }

                    ftpResponse.Close();

                    ftpWebRequest = (FtpWebRequest)System.Net.FtpWebRequest.Create(fullUrlIniziale);
                    ftpWebRequest.Credentials = DammiCredenziali(ref elemUpload);
                    ftpWebRequest.Method = System.Net.WebRequestMethods.Ftp.Rename;
                    ftpWebRequest.RenameTo = elemUpload.FileName;
                    ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                    ftpStatusCode = ftpResponse.StatusCode;

                    if (ftpStatusCode != FtpStatusCode.FileActionOK)
                    {
                        Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "Non sono riuscito a rinominare il file, fullUrlIniziale:<" + fullUrlIniziale.OriginalString + "> in elemUpload.nomeFile:<" + elemUpload.FileName + ">"));
                        ftpResponse.Close();
                        return;
                    }
                    ftpResponse.Close();
                }
            }
            catch (ThreadAbortException ex)  //Thread interrupted by abort, do nothing
            { var tmp = ex; }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(elemUpload.tipoLogEccezione, "", "Eccezione per l'url:<" + elemUpload.url + ">, ex.mess:<" + ex.Message + ">"));
                elemUpload.DescErr = ex.Message;
                elemUpload.UploadState = UplItemState.Eccezione;
            }
            finally
            {
                client.CancelAsync();
            }
        }

        private ICredentials DammiCredenziali(ref UploadItem elemUpload)
        {
            if (elemUpload.User != "")
                return new NetworkCredential(elemUpload.User, elemUpload.Password);
            else
                return CredentialCache.DefaultCredentials;
        }

        private void UploadTerminato(Object sender, UploadDataCompletedEventArgs e)
        {
            //Dim client As WebClient = sender
            UploadItem elem = default(UploadItem);
            //If elem.statoDownload = StatiElemDwl.Timeout OrElse elem.statoDownload = StatiElemDwl.Eccezione Then Exit Sub
            try
            {
                elem = (UploadItem)e.UserState;
                if (e.Result == null) return;
                if (elem.UploadState == UplItemState.UplAvviato)
                {
                    elem.UploadState = UplItemState.UplCompletato;
                    elem.progressione.IsComplete = true;
                }
            }
            catch (TargetInvocationException ex)
            {  //Si verifica quando il threadDownload viene stoppat mentre si sta ancora effettuando il download
                if (elem.UploadState == UplItemState.UplAvviato) elem.UploadState = UplItemState.Eccezione;
                Log.main.Add(new Mess(LogType.Warn, "", "TargetInvocationException, ex.mess:<" + ex.Message + ">"));
                return;

            }
            catch (Exception ex)
            {
                if (elem.UploadState == UplItemState.UplAvviato) elem.UploadState = UplItemState.Eccezione;
                Log.main.Add(new Mess(LogType.Warn, "", "ex.mess:<" + ex.Message + ">"));
                return;
            }
        }

        private void PercentualeUpload(object sender, UploadProgressChangedEventArgs e)
        {
            if (e == null) return;
            UploadItem elem = (UploadItem)e.UserState;
            elem.progressione.Percentage = e.ProgressPercentage;
        }

        private void ControlloElementiEStatistiche()
        {
            Int32 elemDaElim; UInt64 totalizzatoreKBSec; bool raggiuntoMaxKBSecTmp, elementoDiInteresse;
            IOrderedEnumerable<KeyValuePair<UInt64, UploadItem>> chiaviValori;

            raggiuntoMaxKBSecTmp = false;
            totalizzatoreKBSec = 0;

            chiaviValori = from tmp in this.queue
                           where tmp.Value.UploadState == UplItemState.UplCompletato ||
                           tmp.Value.UploadState == UplItemState.TimeoutToStart ||
                                      tmp.Value.UploadState == UplItemState.Timeout ||
                                      tmp.Value.UploadState == UplItemState.Eccezione
                           orderby tmp.Key ascending
                           select tmp;

            elemDaElim = chiaviValori.Count() - this.config.MaxItemInStatisticsQueue;

            if (elemDaElim < 0) elemDaElim = 0;

            //For Each chiaveValore In From c In Me.codaStatistiche  'Può dare errori se gli si modifica il numero degli elementi del dictionary

            foreach (KeyValuePair<UInt64, UploadItem> chiaveValore in chiaviValori)
            {

                elementoDiInteresse = false;

                CheckUploadSubsetEnded(chiaveValore, ref elementoDiInteresse);

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

        private void CheckUploadSubsetEnded(KeyValuePair<UInt64, UploadItem> chiaveValore, ref bool elementoDiInteresse) //Per verificare se un dato sottoinsieme ha finito il download e alzare un evento
        {
            if (chiaveValore.Value.idSubsetInList == "" || chiaveValore.Value.IsOperationListEnded == true) return;
            elementoDiInteresse = true;
            var chiaviValoriConIdLista = from tmp in this.queue where tmp.Value.idSubsetInList == chiaveValore.Value.idSubsetInList select tmp;

            var uploadInCorso = from tmp in chiaviValoriConIdLista
                                where tmp.Value.UploadState == UplItemState.Iniziale || tmp.Value.UploadState == UplItemState.UplAvviato
                                select tmp;
            if ((uploadInCorso).Count() == 0)
            { // 'Significa che tutti i upload con quel idListaUpl sono terminati
                foreach (KeyValuePair<UInt64, UploadItem> tmpChiaveValore in chiaviValoriConIdLista)
                {
                    tmpChiaveValore.Value.RaiseEvent_SubsetOperationEnded();
                }
                elementoDiInteresse = false; //Lo rimetto false dato che ormai ho lanciato gli eventi
            }
        }

        private void ControlloKBSec(KeyValuePair<UInt64, UploadItem> chiaveValore, ref UInt64 totalizzatoreKBSec, ref bool raggiuntoMaxKBSecTmp, ref bool elementoDiInteresse)
        {

            DateTime ora = DateTime.Now;
            if ((ora - chiaveValore.Value.OperationFinishDate).TotalSeconds > this.config.CheckKBSecInterval) return;

            elementoDiInteresse = true;

            if (chiaveValore.Value.UploadState != UplItemState.UplCompletato) return;

            totalizzatoreKBSec += Convert.ToUInt64(chiaveValore.Value.Data.Length / 1024);

            if ((totalizzatoreKBSec / this.config.CheckKBSecInterval) > this.config.MaxKBSec) raggiuntoMaxKBSecTmp = true;
        }

    }
}
