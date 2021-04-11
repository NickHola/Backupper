using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Main.Logs;
using Main.Validations;


namespace Main.DataOre
{
    public class Orario : INotifyPropertyChanged
    {
        private DateTime time;
        //private byte ora, minuti, secondi; private UInt16 millesimi;
        private string strOrario;  //variabile ViewModel per la modifica tramite controllo xaml

        public int Ora
        {
            get { return time.Hour; }
            set
            {
                Validation.CtrlValue(value);
                if (value <0 || value > 23)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "L'ora deve essere compresa tra 0 e 23", "Ricevuto value non compreso tra 0 e 23, value:<" + value + ">")));

                time = new DateTime(1,1,1, value, time.Minute, time.Second, time.Millisecond);
                ImpostaStringaDaProprietà();
            }
        }
        public int Minuti
        {
            get { return time.Minute; }
            set
            {
                Validation.CtrlValue(value);
                if (value < 0 || value > 59)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "I minuti devono essere compresi tra 0 e 59", "Ricevuto value non compreso tra 0 e 59, value:<" + value + ">")));

                time = new DateTime(1, 1, 1, time.Hour, value, time.Second, time.Millisecond);
                ImpostaStringaDaProprietà();
            }
        }
        public int Secondi
        {
            get { return time.Second; }
            set
            {
                Validation.CtrlValue(value);
                if (value < 0 || value > 59)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "I secondi devono essere compresi tra 0 e 59", "Ricevuto value non compreso tra 0 e 59, value:<" + value + ">")));

                time = new DateTime(1, 1, 1, time.Hour, time.Minute, value, time.Millisecond);
                ImpostaStringaDaProprietà();
            }
        }
        public int Millesimi
        {
            get { return time.Millisecond; }
            set
            {
                Validation.CtrlValue(value);
                if (value < 0 || value > 999)
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "I millisecondi devono essere compresi tra 0 e 999", "Ricevuto value non compreso tra 0 e 999, value:<" + value + ">")));

                time = new DateTime(1, 1, 1, time.Hour, time.Minute, time.Second, value);
                ImpostaStringaDaProprietà();
            }
        }

        public string StrOrario
        {
            get { return strOrario; }
            set
            {
                Validation.CtrlValue(value);

                Mess logMess = new Mess(LogType._Nothing, Log.main.errUserText);
                if (ChkSintassiStringa(value, aggiornaProprietà: true, logMess: logMess) == false)
                {
                    bool logFound;
                    Excep.LeggiLogInEx(logMess.testoDaLoggare, out logMess, out logFound);
                    if (logFound == true)
                        throw new Exception(logMess.testoDaVisual);
                    else
                        throw new Exception("Formato non valido");
                }
                strOrario = value;
            }
        }

        public Orario() : this(0)
        { //Per il dataGrid
            time = new DateTime();
        }
        public Orario(string strOrario)
        {
            Mess logMess = new Mess(LogType._Nothing, Log.main.errUserText);
            if (ChkSintassiStringa(strOrario, aggiornaProprietà: true, logMess: logMess) == false)
            {
                logMess.tipo = LogType.ERR;
                throw new Exception(Excep.ScriviLogInEx(logMess));
            }
        }

        public Orario(byte ora, byte minuti = 0, byte secondi = 0, UInt16 millesimi = 0)
        { //ora As Int32 per poter utilizzare new(0)
            time = new DateTime(1, 1, 1, ora, minuti, secondi, millesimi);
        }

        private void ImpostaStringaDaProprietà()
        {

            try
            {
                strOrario = Ora + ":" + Minuti + ":" + Secondi + "." + Millesimi;  //uso la var strOrarioVM_ e non la prop. poichè non deve andare a risettare le property
            }
            catch (Exception ex)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, ex.Message)));
            }
        }

        /// <param name="orario">Se valorizzato </param>
        /// <returns></returns>
        private bool ChkSintassiStringa(string strOrario, Orario orario = null, bool aggiornaProprietà = false, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType._Nothing, "");

            string[] splitProp, splitSecMilli;
            if (orario == null)
                orario = new Orario();
            //else
            //{
            //   this.time = new DateTime(1, 1, 1, 0, 0, 0, 0);
            //}

            splitProp = strOrario.Split(':');

            if (splitProp.Length > 3)
            {
                logMess.testoDaLoggare = "Ricevuto strOrario contenente più di 2 ':', strOrario:<" + strOrario + ">";
                Log.main.Add(logMess);
                return false;
            }
            try
            {
                if (splitProp.Length >= 1) orario.Ora = Convert.ToByte(splitProp[0]);
                if (splitProp.Length >= 2) orario.Minuti = Convert.ToByte(splitProp[1]);
                if (splitProp.Length >= 3)
                {
                    splitSecMilli = splitProp[2].Split('.');
                    orario.Secondi = Convert.ToByte(splitSecMilli[0]);
                    if (splitSecMilli.Length >= 2) orario.Millesimi = Convert.ToUInt16(splitSecMilli[1]);
                }
                //If IsNothing(Orario) = False Then Orario = tmpOrario
                if (aggiornaProprietà == true)
                    this.time = new DateTime(1, 1, 1, orario.Ora, orario.Minuti, orario.Secondi, orario.Millesimi);
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
