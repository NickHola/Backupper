using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Www
{
    public abstract class OperazioneBase : INotifyPropertyChanged
    {
        int itemToProcess, itemInProcessing;
        internal string nome;
        protected Thread thrScoda;
        protected bool raggiuntoMaxKBSec;
        protected UInt64 idItem;
        protected const byte timeoutToStartSec = 2; //TODO rimettere a 2 secondi

        public int ItemToProcess //TODO implement in upload
        {
            get { return itemToProcess; }
            protected set
            {
                itemToProcess = value;
                OnPropertyChanged();
            }
        }

        public int ItemInProcessing //TODO implement in upload
        {
            get { return itemInProcessing; }
            protected set
            {
                itemInProcessing = value;
                OnPropertyChanged();
            }
        }
        


        internal OperazioneBase(string nome) {
            this.nome = nome;
            raggiuntoMaxKBSec = false;
            idItem = 0;
         }

        protected abstract void ThrCicloReadInputBufferAndElaborateMainQueue();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
