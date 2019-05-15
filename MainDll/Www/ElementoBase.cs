using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Main.Logs;
using Main.Validations;

namespace Main.Www
{
    public abstract class ItemBase : INotifyPropertyChanged
    {
        string descErr; int timeoutSec; byte[] data;
        DateTime operationStartDate, operationFinishDate; bool isOperationListEnded;
        byte priority;
        UInt64 idItemInList;

        public readonly string url, idSubsetInList;
        public Progressione progressione;
        internal Tipi tipoLogTimeout, tipoLogEccezione;

               
        public event EventHandler SubsetOperationEnded;

        public string DescErr
        {
            get { return descErr; }
            internal set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                descErr = value;
            }
        }
        public int TimeoutSec
        {
            get { return timeoutSec; }
            internal set //Settabile solo all'interno della dll
            {
                Validation.CtrlValue(value);
                timeoutSec = value;
            }
        }
        public byte[] Data
        {
            get { return data; }
            internal set //Settabile solo all'interno della dll
            {
                Validation.CtrlValue(value);
                data = value;
            }
        }
        public DateTime OperationStartDate
        {
            get { return operationStartDate; }
            internal set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                operationStartDate = value;
            }
        }
        public DateTime OperationFinishDate
        {
            get { return operationFinishDate; }
            protected set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                operationFinishDate = value;
            }
        }
        public bool IsOperationListEnded
        {
            get { return isOperationListEnded; }
            protected set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                isOperationListEnded = value;
            }
        }
        public byte Priority
        {
            get { return priority; }
            private set
            {
                Validation.CtrlValue(value);
                priority = value;
            }
        }
        public UInt64 IdItemInList
        {
            get { return idItemInList; }
            set
            { //Si può settare solamente 1 volta per sicurezza
                if (idItemInList != 0)
                {
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "idInList già è stato settato, non si può modificare", visualMsgBox: false));
                    return;
                }
                idItemInList = value;
                OnPropertyChanged();
            }
        }

        /// <param name="idSubsetOperaz">Serve per far scatenare l'evento SubsetOperationEnded, quando tutti gli elementi di download con un certo id sono terminati</param>  
        /// <param name="timeoutSec">Se omesso o 0 si prende il valore defaultTimeoutSec dell'oggetto di tipo ConfigDownload, non può essere infinito</param>
        public ItemBase(string url, byte priority = 128, string idSubsetOperaz = "", int timeoutSec = 0, Progressione progressione = null, Tipi tipoLogTimeout = Tipi.Warn, Tipi tipoLogEccezione = Tipi.ERR) {
            this.url = url;
            this.Priority = priority;
            this.idSubsetInList = idSubsetOperaz;
            this.TimeoutSec = timeoutSec;
            this.progressione = progressione == null ? new Progressione() : progressione;
            this.tipoLogTimeout = tipoLogTimeout;
            this.tipoLogEccezione = tipoLogEccezione;
            idItemInList = 0;
        }

        internal void RaiseEvent_SubsetOperationEnded() {
            SubsetOperationEnded?.Invoke(this, new GenericEventArgs()); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            IsOperationListEnded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
