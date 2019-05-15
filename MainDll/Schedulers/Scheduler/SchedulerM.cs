using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Main.DataOre;
using Main.Logs;

namespace Main.Schedulers
{
    //Quando la firma validazione dell'ultimaEsecuzione e differente da quella creata tramite DateTime.Now significa che posso inviare l'evento avvia, poichè sono cambiate le condizioni di validità
    [Serializable]
    public class SchedulerM : INotifyPropertyChanged
    {
        bool isEnabled;
        DateTime lastTimeExec;
        List<Tuple<DateTime, string>> firmeUltimeEsecuzione;  //Segna i parametri che hanno permesso le ultime esecuzione dello scheduler, verrà confrontato di volta in volta con la firma generata durante la verifica delle condizioni. ...
                                                              //...lista di tupla e non una stringa poichè se c'è 1 orario a cavallo della mezza notte e in mezzo un altro orario(es. 23:00-01:00 ; 10:00-12:00) veniva eseguito 3 volte poichè: ...
                                                              //...1° esec dopo la mezzanotte, 2° esecuzione tra le 10:00-12:00 (e la firma sostituiva quella precedente), 3° esecuzione dopo le 23:00 ma prima della mezza notte
        WeekDay weekStartsDay;
        Periodicity periodicCycle; bool isPeriodicCycleEnabled;
        HourlyPeriods hourlyPeriods; bool isHourlyPeriodsEnabled;
        WeekDays weekDays; bool isWeekDaysEnabled;
        MonthsAndDays monthsAndDays; bool isMonthsAndDaysEnabled;
        YearWeeks yearWeeks; bool isYearWeeksEnabled;
        YearDays yearDays; bool isYearDaysEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                if (isEnabled == false)
                    tmrCheckIfExecutable.Stop();
                else
                    if (CheckIntervalMs > 0) tmrCheckIfExecutable.Start();
                OnPropertyChanged();
            }
        }
        public WeekDay WeekStartsDay
        {
            get { return weekStartsDay; }
            set
            {
                weekStartsDay = value;
                OnPropertyChanged();
            }
        }
        public bool IsPeriodicCycleEnabled
        {
            get { return isPeriodicCycleEnabled; }
            set
            {
                isPeriodicCycleEnabled = value;
                if (value == true && PeriodicCycle == null) PeriodicCycle = new Periodicity(PeriodicUnit.Daily);
                OnPropertyChanged();
            }
        }
        public Periodicity PeriodicCycle
        {
            get { return periodicCycle; }
            set
            {
                periodicCycle = value;
                OnPropertyChanged();
            }
        }
        public bool IsHourlyPeriodsEnabled
        {
            get { return isHourlyPeriodsEnabled; }
            set
            {
                isHourlyPeriodsEnabled = value;
                if (value == true && HourlyPeriods == null) HourlyPeriods = new HourlyPeriods();
                OnPropertyChanged();
            }
        }
        public HourlyPeriods HourlyPeriods
        {
            get { return hourlyPeriods; }
            set
            {
                hourlyPeriods = value;
                OnPropertyChanged();
            }
        }
        public bool IsWeekDaysEnabled
        {
            get { return isWeekDaysEnabled; }
            set
            {
                isWeekDaysEnabled = value;
                if (value == true && WeekDays == null) WeekDays = new WeekDays();
                OnPropertyChanged();
            }
        }
        public WeekDays WeekDays
        {
            get { return weekDays; }
            set
            {
                weekDays = value;
                OnPropertyChanged();
            }
        }
        public bool IsMonthsAndDaysEnabled
        {
            get { return isMonthsAndDaysEnabled; }
            set
            {
                isMonthsAndDaysEnabled = value;
                if (value == true && MonthsAndDays == null) MonthsAndDays = new MonthsAndDays();
                OnPropertyChanged();
            }
        }
        public MonthsAndDays MonthsAndDays
        {
            get { return monthsAndDays; }
            set
            {
                monthsAndDays = value;
                OnPropertyChanged();
            }
        }
        public bool IsYearWeeksEnabled
        {
            get { return isYearWeeksEnabled; }
            set
            {
                isYearWeeksEnabled = value;
                if (value == true && YearWeeks == null) YearWeeks = new YearWeeks();
                OnPropertyChanged();
            }
        }
        public YearWeeks YearWeeks //Fare classe YearWeeks per convalida dei valori durante add e set
        {
            get { return yearWeeks; }
            set
            {
                yearWeeks = value;
                OnPropertyChanged();
            }
        }
        public bool IsYearDaysEnabled
        {
            get { return isYearDaysEnabled; }
            set
            {
                isYearDaysEnabled = value;
                if (value == true && YearDays == null) YearDays = new YearDays();
                OnPropertyChanged();
            }
        }
        public YearDays YearDays
        {
            get { return yearDays; }
            set
            {
                yearDays = value;
                OnPropertyChanged();
            }
        } //Fare classe YearWeeks per convalida dei valori durante add e set
        public DateTime LastTimeExec
        {
            get { return lastTimeExec; }

        }
        public event EventHandler Esegui;

        private System.Timers.Timer tmrCheckIfExecutable = new System.Timers.Timer();

        public double CheckIntervalMs
        {
            get { return tmrCheckIfExecutable.Interval; }
            set
            {
                if (value <= 0)
                    tmrCheckIfExecutable.Stop();
                else
                {
                    tmrCheckIfExecutable.Interval = value;
                    if (IsEnabled == true) tmrCheckIfExecutable.Start();
                }
            }
        }
        //Public Sub New() 'Questo costruttore serve per fare un nuovo ogg prima del deserializzatore?
        //End Sub

        /// <summary>
        /// </summary>
        /// <param name="periodicCycle">Se valorizzato verrà eseguita non più di una volta ogni unità di periodicita, es perdiocità settimanale = al massimo 1 volta a settimana</param>
        /// <param name="intervalloVerificaMs">Se 0 non verrà scatenato l'evento "Esegui" in automatico ma si dovrà richiamare il metodo Eseguibile per sapere se si ha il permesso dello scheduler</param>
        public SchedulerM(Periodicity periodicCycle = null, HourlyPeriods hourlyPeriods = null, WeekDays weekDays = null, MonthsAndDays monthsAndDays = null, YearWeeks yearWeeks = null,
                        YearDays yearDays = null, WeekDay weekStartsDay = WeekDay.Monday, UInt32 intervalloVerificaMs = 0)
        {
            //Creo gli oggetti per far in modo che i vari dataGrid abbiano gli oggetti su cui fare il binding
            this.PeriodicCycle = periodicCycle;
            this.HourlyPeriods = hourlyPeriods;
            this.WeekDays = weekDays;
            this.MonthsAndDays = monthsAndDays;
            this.YearWeeks = yearWeeks;
            this.YearDays = yearDays;
            this.WeekStartsDay = weekStartsDay;
            this.CheckIntervalMs = intervalloVerificaMs;

            this.lastTimeExec = DateTime.MinValue;
            this.firmeUltimeEsecuzione = new List<Tuple<DateTime, String>>();

            tmrCheckIfExecutable.Elapsed += TmrCheckIfExecutable_Elapsed;
        }

        private void TmrCheckIfExecutable_Elapsed(Object sender, System.Timers.ElapsedEventArgs args)
        {
            if (CheckIfExecutable() == true) Esegui?.Invoke(this, new EventArgs());
        }

        public bool CheckIfExecutable(Boolean ignoraChkUltimeFirme = false)
        {
            return CheckIfExecutable(ignoraChkUltimeFirme, DateTime.MinValue);
        }

        private bool CheckIfExecutable(bool ignoraChkUltimeFirme, DateTime debugData)
        {
            DateTime dataAtt; string firmaEsecuzioneAttuale;
            dataAtt = DateTime.Now;
            if (debugData != DateTime.MinValue) dataAtt = debugData;

            firmaEsecuzioneAttuale = "";

            if (isPeriodicCycleEnabled == false && isHourlyPeriodsEnabled == false && isWeekDaysEnabled == false && isMonthsAndDaysEnabled == false && isYearWeeksEnabled == false && isYearDaysEnabled == false)
                return false;

            if (isPeriodicCycleEnabled == true)
            {
                if (PeriodicCycle != null)
                    if (PeriodicCycle.VerificaSeEseguibile(lastTimeExec, dataAtt, WeekStartsDay, ref firmaEsecuzioneAttuale) == false)
                        return false;
                    else
                        Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto PeriodicCycle a null ma isPeriodicCycleEnabled a true"));
            }

            //Attenzione: i vari Exec possono ritornare anche null
            if (isHourlyPeriodsEnabled == true)
                if (!ExecHourlyPeriods(HourlyPeriods, dataAtt, ref firmaEsecuzioneAttuale) == true) return false;
            if (isWeekDaysEnabled == true)
                if (!ExecWeekDays(WeekDays, dataAtt, ref firmaEsecuzioneAttuale) == true) return false;
            if (isMonthsAndDaysEnabled == true)
                if (!ExecMonthsAndDays(MonthsAndDays, dataAtt, ref firmaEsecuzioneAttuale) == true) return false;
            if (isYearWeeksEnabled == true)
                if (!ExecYearWeeks(YearWeeks, dataAtt, ref firmaEsecuzioneAttuale) == true) return false;
            if (isYearDaysEnabled == true)
                if (!ExecYearDays(YearDays, dataAtt, ref firmaEsecuzioneAttuale) == true) return false;


            if (ignoraChkUltimeFirme == false)
            {
                if ((from tmp in firmeUltimeEsecuzione where tmp.Item2 == firmaEsecuzioneAttuale select tmp).Count() > 0) return false; //Verifico se è già presente la firma attuale nelle firme delle ultime esecuzioni
                                                                                                                                        //Elimino le firme che non sono più di oggi poichè non mi servono solo quelle del giorno per ovviare al problema dell'orario a cavallo della mezza notte
                firmeUltimeEsecuzione = (from tmp in firmeUltimeEsecuzione where tmp.Item1 >= new DateTime(dataAtt.Year, dataAtt.Month, dataAtt.Day) select tmp).ToList();
                firmeUltimeEsecuzione.Add(new Tuple<DateTime, String>(dataAtt, firmaEsecuzioneAttuale)); //Aggiungo la firma dell'ultima esecuzione
            }

            lastTimeExec = dataAtt;
            if (PeriodicCycle != null) PeriodicCycle.AggiornaIndiciUltimaEsec();

            return true;
        }

        private bool? ExecHourlyPeriods(HourlyPeriods hourlyPeriods, DateTime dataAtt)
        {
            string tmp = "";
            return ExecHourlyPeriods(hourlyPeriods, dataAtt, ref tmp);
        }
        private bool? ExecHourlyPeriods(HourlyPeriods hourlyPeriods, DateTime dataAtt, ref string firmaEsecuzione)
        {
            if (hourlyPeriods == null)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto hourlyPeriods a null ma isPeriodicCycleEnabled a true"));
                return null;      //Non ci sono filtri per periodi di orari quindi torno null
            }
            if (hourlyPeriods.Count == 0) return null;

            DateTime oraInizio = DateTime.MinValue, oraFine = DateTime.MinValue;
            bool esitoControllo = false;

            foreach (var periodo in hourlyPeriods)
            {
                oraInizio = new DateTime(dataAtt.Year, dataAtt.Month, dataAtt.Day, periodo.Da.Ora, periodo.Da.Minuti, periodo.Da.Secondi, periodo.Da.Millesimi);
                oraFine = new DateTime(dataAtt.Year, dataAtt.Month, dataAtt.Day, periodo.A.Ora, periodo.A.Minuti, periodo.A.Secondi, periodo.A.Millesimi);

                if (oraInizio < oraFine) //Significa che l'ora fine non va oltre la mezzanotte, esempio oraInizio = 20:00 - oraFine = 23:50
                { if (dataAtt >= oraInizio && dataAtt <= oraFine) esitoControllo = true; }
                else if (oraInizio > oraFine) { if (dataAtt >= oraInizio || dataAtt <= oraFine) esitoControllo = true; } //Significa che l'ora fine VA OLTRE la mezzanotte, esempio oraInizio = 20:00 - oraFine = 00:30
                else
                { //Date uguali 
                    Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "Inserite date identiche nel periodo orario"));
                    if (dataAtt == oraInizio) esitoControllo = true;
                }

                if (esitoControllo == true) break;
            }

            firmaEsecuzione += "giorno:" + oraInizio.ToString("dd/MM/yyyy") + ";oraInizio:" + oraInizio.ToString("HH:mm:ss.ff") + ";oraFine:" + oraFine.ToString("HH:mm:ss.ff") + "|";

            return esitoControllo;
        }
        private bool? ExecWeekDays(WeekDays weekDays, DateTime dataAtt)
        {
            string tmp = "";
            return ExecWeekDays(weekDays, dataAtt, ref tmp);
        }
        private bool? ExecWeekDays(WeekDays weekDays, DateTime dataAtt, ref string firmaEsecuzione)
        {
            if (weekDays == null)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto weekDays a null ma isWeekDaysEnabled a true"));
                return null;     //Non ci sono filtri per GiorniSett
            }
            if (weekDays.Count == 0) return null;

            //if (weekDays.Contains((WeekDay)dataAtt.DayOfWeek) == false) return false;
            if ((from tmp in weekDays where tmp.SelectedDay == ((WeekDay)dataAtt.DayOfWeek) select tmp).Count() == 0) return false;
            firmaEsecuzione += "WeekDay:" + dataAtt.DayOfWeek + dataAtt.Day + "|"; //Non metto il DayOfWeek.ToString preferisco il numero

            return true;
        }
        private bool? ExecMonthsAndDays(MonthsAndDays monthsAndDays, DateTime dataAtt)
        {
            string tmp = "";
            return ExecMonthsAndDays(monthsAndDays, dataAtt, ref tmp);
        }
        private bool? ExecMonthsAndDays(MonthsAndDays monthsAndDays, DateTime dataAtt, ref string firmaEsecuzione)
        {
            if (monthsAndDays == null)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto monthsAndDays a null ma isMonthsAndDaysEnabled a true"));
                return null;
            }
            if (monthsAndDays.Count == 0) return null;

            foreach (var monthDay in monthsAndDays)
            {
                if ((monthDay.Month == 0 && monthDay.Day == dataAtt.Day) ||
                    (monthDay.Month == dataAtt.Month && monthDay.Day == 0) ||
                    (monthDay.Month == dataAtt.Month && monthDay.Day == dataAtt.Day))
                {
                    firmaEsecuzione += "MeseGiorno:" + dataAtt.Year + dataAtt.Month + dataAtt.Day + "|";
                    return true;
                }
            }
            return false;
        }
        private bool? ExecYearWeeks(YearWeeks yearWeeks, DateTime dataAtt)
        {
            string tmp = "";
            return ExecYearWeeks(yearWeeks, dataAtt, ref tmp);
        }
        private bool? ExecYearWeeks(YearWeeks yearWeeks, DateTime dataAtt, ref string firmaEsecuzione)
        {
            if (yearWeeks == null)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto yearWeeks a null ma isYearWeeksEnabled a true"));
                return null;
            }
            if (yearWeeks.Count == 0) return null;

            Calendar calendario = DateTimeFormatInfo.CurrentInfo.Calendar;

            var nrSettimana = calendario.GetWeekOfYear(dataAtt, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, (DayOfWeek)WeekStartsDay);

            if ((from tmp in yearWeeks where tmp.Week == nrSettimana select tmp).Count() == 0) return false;

            //if (yearWeeks.Contains(nrSettimana) == false) return false;
            firmaEsecuzione += "nrSettDellAnno:" + nrSettimana + "|";

            return true;
        }
        private bool? ExecYearDays(YearDays yearDays, DateTime dataAtt)
        {
            string tmp = "";
            return ExecYearDays(yearDays, dataAtt, ref tmp);
        }
        private bool? ExecYearDays(YearDays yearDays, DateTime dataAtt, ref string firmaEsecuzione)
        {
            if (yearDays == null)
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "oggetto yearDays a null ma isYearDaysEnabled a true"));
                return null;     //Non ci sono filtri per GiorniSett
            }
            if (yearDays.Count == 0) return null;

            //if (yearDays.Contains(dataAtt.DayOfYear) == false) return false;
            if ((from tmp in yearDays where tmp.Day == dataAtt.DayOfYear select tmp).Count() == 0) return false;
            firmaEsecuzione += "nrGiorniDellAnno:" + dataAtt.DayOfYear + "|";
            return true;
        }
        public SortedDictionary<DateTime, string> TestScheduler(bool ignoraChkUltimeFirme, DateTime lastDateToCheck = default, UInt16 nrMaxPositiveCheck = 15, UInt64 checkIntervalSec = 59)
        {
            SortedDictionary<DateTime, string> dates = new SortedDictionary<DateTime, string>();
            DateTime timeToCheck = DateTime.Now;
            if (lastDateToCheck == default) lastDateToCheck = DateTime.Now.AddYears(2);
            UInt16 PositiveCheck = 0;

            //System.Threading.Thread.Sleep(3000);
            //wndTest.testoDaVisualizzare = "avviato";
            while (timeToCheck < lastDateToCheck && PositiveCheck < nrMaxPositiveCheck)
            {
                if (CheckIfExecutable(ignoraChkUltimeFirme, timeToCheck) == true)
                {
                    dates.Add(timeToCheck, timeToCheck.ToString());
                    PositiveCheck += 1;
                }

                timeToCheck = timeToCheck.AddSeconds(checkIntervalSec);
            }
            return dates;
            //wndTest.testoDaVisualizzare += vbCrLf & "fine"
        }

        public void resetFirmeUltimeEsecuzione()
        {
            firmeUltimeEsecuzione = new List<Tuple<DateTime, string>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
