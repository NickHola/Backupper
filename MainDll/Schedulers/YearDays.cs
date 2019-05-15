using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Schedulers
{
    //ATTENZIONE: ho dovuto per forza fare un oggetto YearWeek e non ho potuto fare BindingList<int> poichè appena si va in edit sulla cella si crea un elemento int che per default vale 0, alzo l'eccezione ma non viene 
    //ancora gestita dalla DataGridView (non so perchè) e la raccoglie il gestore delle eccezioni che ho realizzato nel file Excep.cs

    public class YearDays : BindingList<YearDay>
    {
        public YearDays()
        {
        }
        //protected override void InsertItem(int index, int item) { }
        //protected override void SetItem(int index, int item) { }
    }

    public class YearDay : INotifyPropertyChanged
    {
        private UInt16 day;
        public UInt16 Day
        {
            get { return day; }
            set
            {
                if (value == 0)
                    throw new Exception("Il valore minimo consentito è 1"); //throw new Excep per Validation.ErrorTemplate 
                if (value > 366)
                    throw new Exception("Il valore massimo consentito è 366");

                day = value;
                OnPropertyChanged();
            }
        }

        public YearDay()
        {
            Day = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
