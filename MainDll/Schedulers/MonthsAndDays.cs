using Main.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Main.Validations;

namespace Main.Schedulers
{
    public class MonthsAndDays : BindingList<MonthAndDay>
    {

        public MonthsAndDays(params byte[] coppieGiornoMese)
        {
            if (coppieGiornoMese.Length % 2 > 0)
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "L'array coppieGiornoMese contiene un numero di giorni mesi non pari, coppieGiornoMese.Length:<" + coppieGiornoMese.Length + ">")));

            for (int i = 0; i < coppieGiornoMese.Length; i += 2)
                this.Items.Add(new MonthAndDay(coppieGiornoMese[i], coppieGiornoMese[i + 1]));
        }

        //protected override void InsertItem(int index, Tuple<byte, byte> item) { }
        //protected override void SetItem(int index, Tuple<byte, byte> item) { }
    }

    public class MonthAndDay : INotifyPropertyChanged
    {
        byte month, day;
        string strMonthDay;

        public byte Month
        {
            get { return month; }
            set
            {
                if (value > 12)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "Il mese non può essere maggiore di 12", "Rcevuto value maggiore di 12, value:<" + value + ">")));
                if (ItemValidation(value, day) == false)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "Mese e giorno non validi", "Combinazione di mese e giorno non validi, mese:<" + value + ">, giorno:<" + Day + ">")));

                month = value;
                OnPropertyChanged();
            }
        }
        public byte Day
        {
            get { return day; }
            set
            {
                if (value > 31)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "Il giorno non può essere maggiore di 31", "Rcevuto value maggiore di 31, value:<" + value + ">")));
                if (ItemValidation(month, value) == false)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "Mese e giorno non validi", "Combinazione di mese e giorno non validi, mese:<" + Month + ">, giorno:<" + value + ">")));

                day = value;
                OnPropertyChanged();
            }
        }


        public string StrMonthDay
        {
            get { return strMonthDay; }
            set
            {
                Validation.CtrlValue(value);

                Mess logMess = new Mess(LogType._Nothing, Log.main.errUserText);
                if (ChkStringSyntax(value, aggiornaProprietà: true, logMess: logMess) == false)
                {
                    bool logFound;
                    Excep.LeggiLogInEx(logMess.testoDaLoggare, out logMess, out logFound);
                    if (logFound == true)
                        throw new Exception(logMess.testoDaVisual);
                    else
                        throw new Exception("Formato non valido");
                }
                strMonthDay = value;
            }
        }


        public MonthAndDay()
        {
            Month = 1;
            Day = 1;
        }

        public MonthAndDay(byte month, byte day)
        {
            Month = month;
            Day = day;
        }

        private bool ItemValidation(byte month, byte day)
        {
            if (month == 0 || day == 0) // in questo caso bastano i controlli fatti dalle 2 property
                return true;
            try
            { DateTime time = new DateTime(2020, month, day); } //Anno 2020 bisestile   
            catch
            { return false; }

            return true;
        }

        private bool ChkStringSyntax(string strMonthDay, MonthAndDay monthDay = null, bool aggiornaProprietà = false, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType._Nothing, "");

            string[] splitProp;

            if (monthDay == null)
                monthDay = new MonthAndDay();

            strMonthDay.Replace("\"", "/");

            splitProp = strMonthDay.Split('/');

            if (splitProp.Length > 2)
            {
                logMess.testoDaLoggare = "Ricevuto strMonthDay contenente più di 2 '/', strMonthDay:<" + strMonthDay + ">";
                Log.main.Add(logMess);
                return false;
            }
            try
            {
                if (splitProp.Length >= 1) monthDay.Month = Convert.ToByte(splitProp[0]);
                if (splitProp.Length >= 2) monthDay.Day = Convert.ToByte(splitProp[1]);

                //If IsNothing(Orario) = False Then Orario = tmpOrario
                if (aggiornaProprietà == true)
                {
                    this.month = monthDay.Month;
                    this.day = monthDay.Day;
                }
            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = ex.Message; //Lasciare ex.Message poichè mi serve nel set di StrOrarioVM poichè all'interno ci potrebbe essere serializzato un ogg di tipo Mess
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
