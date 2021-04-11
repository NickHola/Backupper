using System;
using Main.Logs;

namespace Main.DBs
{
    public static class ColumnTypes
    {
        
        public abstract class Base {
            internal bool valQryTraApici;

            internal Base(bool valQryTraApici) {
                this.valQryTraApici = valQryTraApici;
            }
        }

        [Serializable] public class NVarChar : Base
        { //<Serializable> serve per il confronto con il tipo nel dataBase che effettua la func. ControlloStrutturaDB tramite serializzazione

            public enum Formati {
                Nessuno,
                OoDpMm,
                Password
            }

            public readonly string lunghezza; //Lunghezza è una stringa poichè posso ricevere come valore anche MAX
            [NonSerialized] public readonly Formati formato;

            public NVarChar(string lunghezza) : base(true) //Lunghezza è una stringa poichè posso ricevere come valore anche MAX
            { //Lunghezza è una stringa poichè posso ricevere come valore anche MAX

                if (lunghezza == null) {
                    Log.main.Add(new Mess(LogType.ERR, "", "ricevuto lunghezza a nothing"));
                    App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                }

                if (lunghezza.Trim() == "") {
                    Log.main.Add(new Mess(LogType.ERR, "", "ricevuto lunghezza vuota"));
                    App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                }

                this.lunghezza = lunghezza;
                this.formato = Formati.Nessuno;
            }

            public NVarChar(Formati formato, string lunghezza = "") : base(true) //Lunghezza è una stringa poichè posso ricevere come valore anche MAX
            {
                if (lunghezza == null) {
                    Log.main.Add(new Mess(LogType.ERR, "", "ricevuto lunghezza a nothing"));
                    App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                }

                switch (formato) {
                    case Formati.Nessuno:
                        Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto formato con valore nessuno"));
                        App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                        break;
                    case Formati.OoDpMm:
                        this.lunghezza = "5";
                        break;
                    case Formati.Password:
                        this.lunghezza = "MAX"; //Poichè se viene criptata si allunga
                        break;
                    default:
                        Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto valore disatteso per il parametro formato:<" + formato.ToString() + ">"));
                        App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                        break;
                }

                this.formato = formato;
            }
        }

        public class Decimal : Base {
            public readonly UInt32 numeroCifre, numeroDecimali; 

            public Decimal(UInt32 numeroCifre, UInt32 numeroDecimali) : base(false) {
                this.numeroCifre = numeroCifre;
                this.numeroDecimali = numeroDecimali;
            }
        }

        public class Date : Base {
            public Date() : base(true) { }
        }

        public class DateTime : Base { 
            public DateTime() : base(true) { }
        }

        public class Bit : Base {
            public Bit() : base(false) { }
        }

        public class BigInt : Base {
            public BigInt() : base(false) { }
        }

        public class Int : Base {
            public Int() : base(false) { }
        }

        public class SmallInt : Base {
            public SmallInt() : base(false) { }
        }

        public class TinyInt : Base
        {
            public TinyInt() : base(false) { }
        }
    }
}
