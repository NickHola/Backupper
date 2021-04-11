using System;
using Main.Logs;
using Main.Validations;

namespace Main.Www
{
    ///<summary> StatoDownload definisce se il download è terminato (terminato=val diverso da Iniziale e DwlAvviato) e se andato in errore oppure no </summary>
    public class DownloadItem : ItemBase
    {
        DwlItemState downloadState; //, dimensioneDatiKb_ As UInt64
        string textDownloaded;
        public readonly bool translateDataInText;
        public event EventHandler DownloadEnded;

        public string TextDownloaded
        {
            get { return textDownloaded; }
            internal set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                textDownloaded = value;
            }
        }

        public DwlItemState DownloadState
        {
            get { return downloadState; }
            internal set
            { //Settabile solo all'interno della dll
                bool scatenaEvento = false;

                Validation.CtrlValue(value);

                //Serve per non far scatenare l'evento più volte nel caso in sui si setta statoDownload ad un valore diverso da iniziale, più volte
                if ((downloadState != DwlItemState.DwlCompletato && downloadState != DwlItemState.TimeoutToStart && downloadState != DwlItemState.Timeout && downloadState != DwlItemState.Eccezione) &&
                       (value == DwlItemState.DwlCompletato || value == DwlItemState.TimeoutToStart || value == DwlItemState.Timeout || value == DwlItemState.Eccezione)) scatenaEvento = true;

                downloadState = value;

                if (scatenaEvento == true)
                {
                    OperationFinishDate = DateTime.Now; //Va prima del richiamo dell'evento
                    DownloadEnded?.Invoke(this, new GenericEventArgs()); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
                }
            }
        }

        /// <param name="idSubsetOperaz">Serve per far scatenare l'evento downloadListaTerminato, quando tutti gli elementi di download con un certo id sono terminati</param>  
        /// <param name="timeoutSec">Se omesso o 0 si prende il valore defaultTimeoutSec dell'oggetto di tipo ConfigDownload, non può essere infinito</param>
        public DownloadItem(string url, byte priority = 128, string idSubsetOperaz = "", int timeoutSec = 0, bool convertiInTesto = true, LogType tipoLogTimeout = LogType.Warn,
                   LogType tipoLogEccezione = LogType.ERR, Progressione progressione = null) : base(url, priority, idSubsetOperaz, timeoutSec, progressione, tipoLogTimeout, tipoLogEccezione)
        { this.translateDataInText = convertiInTesto; }

    }
}
