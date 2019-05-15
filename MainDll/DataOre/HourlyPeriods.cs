using Main.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.DataOre
{
    public class HourlyPeriods : BindingList<PeriodoOrario>
    {
        public HourlyPeriods() { } //Per il dataGrid
        public HourlyPeriods(params Orario[] coppieOnOff)
        {
            if (coppieOnOff.Length % 2 > 0)
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "L'array coppieOnOff contiene un numero di orari non pari, coppieOnOff.Length:<" + coppieOnOff.Length + ">")));

            for (int i = 0; i < coppieOnOff.Length - 1; i += 2)
                this.Items.Add(new PeriodoOrario(coppieOnOff[i], coppieOnOff[i + 1]));
        }
        public HourlyPeriods(params string[] coppieOnOff)
        {
            if (coppieOnOff.Length % 2 > 0)
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "L'array coppieOnOff contiene un numero di orari non pari, coppieOnOff.Length:<" + coppieOnOff.Length + ">")));

            for (int i = 0; i < coppieOnOff.Length - 1; i += 2)
                this.Items.Add(new PeriodoOrario(new Orario(coppieOnOff[i]), new Orario(coppieOnOff[i + 1])));
        }
        
        protected override void InsertItem(int index, PeriodoOrario item)
        {
            string descErr;
            if (ItemValidation(item, out descErr) == false)
                throw new Exception(descErr);

            base.InsertItem(index, item);
            //throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Formato data non valido, ricevuto giorno:<" + item.Item1 + ">, mese:<" + item.Item2 + ">"))); 
        }

        protected override void SetItem(int index, PeriodoOrario item)
        {
            string descErr;
            if (ItemValidation(item, out descErr) == false)
                throw new Exception(descErr);

            Items[index] = item;
        }

        private bool ItemValidation(PeriodoOrario item, out string descErr)
        {
            descErr = "";
            if (item.Da == null)
            {
                descErr = "Ora di inizio non valorizzata";
                return false;
            }
            if (item.A == null)
            {
                descErr = "Ora di fine non valorizzata";
                return false;
            }
            return true;
        }
    }

    public class PeriodoOrario : INotifyPropertyChanged
    {
        Orario da, a;

        public Orario Da
        {
            get { return da; }
            set
            {
                da = value;
                OnPropertyChanged();
            }
        }
        public Orario A
        {
            get { return a; }
            set
            {
                a = value;
                OnPropertyChanged();
            }
        }

        public PeriodoOrario()
        {
            Da = new Orario();
            A = new Orario();
        }

        public PeriodoOrario(Orario da, Orario a)
        {
            Da = da;
            A = a;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
