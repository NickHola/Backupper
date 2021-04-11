using System;
using System.IO;
using Main.Logs;
using Main.Validations;

namespace Main.Www
{
    ///<summary> StatoUpload definisce se l'upload è terminato(terminato= val diverso da Iniziale e UplAvviato) e se andato in errore oppure no</summary>
    public class UploadItem : ItemBase
    {
        UplItemState uploadState;

        public readonly string User, Password, FileName, CorruptFileNameSuffix;
        public UploadType UploadType;
        
        public event EventHandler UploadEnded; 


           public UplItemState UploadState {  
               get { return uploadState; }

               internal set { //Settabile solo all'interno della dll
                bool scatenaEvento = false;
                Validation.CtrlValue(value);

                //Serve per non far scatenare l'evento più volte nel caso in sui si setta statoDownload ad un valore diverso da iniziale, più volte
                if ((uploadState != UplItemState.UplCompletato && uploadState != UplItemState.TimeoutToStart && uploadState != UplItemState.Timeout && uploadState != UplItemState.Eccezione) &&
                    (value == UplItemState.UplCompletato || value == UplItemState.TimeoutToStart || value == UplItemState.Timeout || value == UplItemState.Eccezione)) scatenaEvento = true;

                uploadState = value;

                if (scatenaEvento == true) {
                    OperationFinishDate = DateTime.Now; //Va prima del richiamo dell'evento
                    UploadEnded?.Invoke(this, new GenericEventArgs()); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
                }
                }
            }

        /// <param name="timeoutSec">Se omesso o 0 si prende il valore defaultTimeoutSec dell'oggetto di tipo ConfigDownload, non può essere infinito</param>
        public UploadItem(string urlFolder, object oggettoUpload, byte priority = 128, string nomeFile = "", string suffFileCorrotto = "", string idSubsetOperaz = "", UploadType tipoUpload = UploadType.Ftp, string utente = "", string password = "",
        int timeoutSec = 0, LogType tipoLogTimeout = LogType.Warn, LogType tipoLogEccezione = LogType.ERR, Progressione progressione = null) : base(urlFolder, priority, idSubsetOperaz, timeoutSec, progressione, tipoLogTimeout, tipoLogEccezione)
        {

            if (oggettoUpload.GetType() == typeof(byte)) {
                Data = (byte[])oggettoUpload;
            } else if (oggettoUpload.GetType() == typeof(String)) {
                if (File.Exists((string)oggettoUpload) == false) throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto oggettoUpload di tipo con tipo String ma il file non esiste, oggettoUpload:<" + oggettoUpload + ">")));
                Data = File.ReadAllBytes((string)oggettoUpload);
            } else {
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto oggettoUpload di tipo disatteso, oggettoUpload.GetType:<" + oggettoUpload.GetType().ToString() + ">")));
            }

            if (nomeFile != "") {
                this.FileName = nomeFile;
            } else {
                if (oggettoUpload.GetType() == typeof(string)) {
                    this.FileName = Path.GetFileName((string)oggettoUpload);
                } else {
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto nomeFile vuoto e oggettoUpload non è una stringa, impossibile ricavare nome file, oggettoUpload.GetType:<" + oggettoUpload.GetType().ToString() + ">")));
                }
            }

            this.CorruptFileNameSuffix = suffFileCorrotto;
            this.User = utente;
            this.Password = password;
            this.UploadType = tipoUpload;
        }
    }
}
