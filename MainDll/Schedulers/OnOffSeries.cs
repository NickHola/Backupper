using Main.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Schedulers
{
    public class OnOffSeries : BindingList<OnOff>  //Non si può usare il tuple<UInt16, UInt16>
    {
        public OnOffSeries() { } //Costruttore per datagrid

        public OnOffSeries(params UInt16[] onOffTuple)
        {
            if (onOffTuple.Length % 2 > 0)
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "L'array onOffTuple contiene un numero di on off non pari, onOffTuple.Length:<" + onOffTuple.Length + ">")));

            for (int i = 0; i < onOffTuple.Length; i += 2)
                this.Items.Add(new OnOff(onOffTuple[i], onOffTuple[i + 1]));
        }
    }

    public class OnOff : INotifyPropertyChanged
    {
        UInt16 on, off;

        public UInt16 On
        {
            get { return on; }
            set
            {
                if (value == 0) throw new Exception("Il valore di On non può essere 0"); //throw new Excep per Validation.ErrorTemplate 
                on = value;
                OnPropertyChanged();
            }
        }
        public UInt16 Off
        {
            get { return off; }
            set
            {
                if (value == 0) throw new Exception("Il valore di Off non può essere 0"); //throw new Excep per Validation.ErrorTemplate 
                off = value;
                OnPropertyChanged();
            }
        }

        public OnOff() {
            On = 1;
            Off = 1;
        }

        public OnOff(UInt16 on, UInt16 off) {
            On = on;
            Off = off;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
