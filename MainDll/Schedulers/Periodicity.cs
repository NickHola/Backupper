using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Main.DataOre;
using Main.Logs;
using Main.Validations;

namespace Main.Schedulers
{
    //unità = arco temporale descritto in periodicità
    //ogniTotUnita = 0 disabilitato; 1 = ogni unità
    public class Periodicity : INotifyPropertyChanged
    {
        private PeriodicUnit unit;
        private UInt16 everyFewUnit;
        private bool isEveryFewUnitEnabled;

        private bool isOnOffUnitSeriesEnabled;
        private OnOffSeries onOffUnitSeries;  //BindingList(Of Tuple(Of UInt16, UInt16))
        private int indiceUltimaEsec, indiceUltimaEsecAggiornato;
        private UInt16 nrUnitàUltimaEsec, nrUnitàUltimaEsecAggiornato;



        public PeriodicUnit Unit
        {
            get { return unit; }
            set
            {
                Validation.CtrlValue(value);
                unit = value;
                OnPropertyChanged();
            }
        }
        public bool IsEveryFewUnitEnabled
        {
            get { return isEveryFewUnitEnabled; }
            set
            {
                Validation.CtrlValue(value);
                isEveryFewUnitEnabled = value;
                if (IsOnOffUnitSeriesEnabled == value) IsOnOffUnitSeriesEnabled = !value;
                OnPropertyChanged();
            }
        }
        public UInt16 EveryFewUnit
        {
            get { return everyFewUnit; }
            set
            {
                Validation.CtrlValue(value);
                if (value == 0) throw new Exception("Every unit can't be 0");
                everyFewUnit = value;
                OnPropertyChanged();
            }
        }
        public bool IsOnOffUnitSeriesEnabled
        {
            get { return isOnOffUnitSeriesEnabled; }
            set
            {
                Validation.CtrlValue(value);
                isOnOffUnitSeriesEnabled = value;
                if (IsEveryFewUnitEnabled == value) IsEveryFewUnitEnabled = !value;
                OnPropertyChanged();
            }
        }
        public OnOffSeries OnOffUnitSeries
        { //BindingList(Of Tuple(Of UInt16, UInt16))
            get { return onOffUnitSeries; }
            set
            { //BindingList(Of Tuple(Of UInt16, UInt16)))
                Validation.CtrlValue(value);
                onOffUnitSeries = value;
                OnPropertyChanged();
            }
        }
        public Periodicity(PeriodicUnit unit, bool isEveryFewUnitEnabled = false, UInt16 everyFewUnit = 1, bool isOnOffUnitSeriesEnabled = false, OnOffSeries onOffUnitSeries = null)
        { //BindingList(Of Tuple(Of UInt16, UInt16)) = Nothing) 'Non effettuo la verifica diei valori di ogniTotUnità e serieOnOffDiUnità
            this.Unit = unit;
            this.IsEveryFewUnitEnabled = isEveryFewUnitEnabled;
            this.EveryFewUnit = everyFewUnit;
            this.IsOnOffUnitSeriesEnabled = isOnOffUnitSeriesEnabled;
            this.OnOffUnitSeries = onOffUnitSeries == null ? new OnOffSeries() : onOffUnitSeries;
            indiceUltimaEsec = -1;
            nrUnitàUltimaEsec = 0;
        }


        public bool VerificaSeEseguibile(DateTime ultimaEsecuzione, DateTime dataAttuale, WeekDay giornoInizioSett, ref string firmaEsecuzione)
        {
            if (IsEveryFewUnitEnabled == true)
            {
                if (everyFewUnit > 0)
                {
                    firmaEsecuzione += "ogniTotUnità;" + unit.ToString() + ";";
                    return VSE_Algo(unit, ultimaEsecuzione, dataAttuale, giornoInizioSett, everyFewUnit, ref firmaEsecuzione);
                }
                else
                {
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "IsEveryFewUnitEnabled è a true ma everyFewUnit è a 0"));
                }
            }

            if (IsOnOffUnitSeriesEnabled == true)
            {
                if (IsEveryFewUnitEnabled == true)
                {
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Sia IsOnOffUnitSeriesEnabled che IsEveryFewUnitEnabled sono a true"));
                }
                else if (OnOffUnitSeries == null || OnOffUnitSeries.Count == 0)
                {
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "IsOnOffUnitSeriesEnabled a true ma OnOffUnitSeries a null o con 0 elementi"));
                }

                firmaEsecuzione += "SerieOnOffDiUnità;" + unit.ToString() + ";";
                if (ultimaEsecuzione == DateTime.MinValue)
                {
                    indiceUltimaEsecAggiornato = 0;
                    nrUnitàUltimaEsecAggiornato = 1;
                    return true;
                }
                else
                {
                    int UnitàPerProssimaEsec = VSE_NrUnitàPerProssimaEsecuzione();
                    return VSE_Algo(unit, ultimaEsecuzione, dataAttuale, giornoInizioSett, UnitàPerProssimaEsec, ref firmaEsecuzione);
                }
            }

            if (IsEveryFewUnitEnabled == false && IsOnOffUnitSeriesEnabled == false)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Sia IsEveryFewUnitEnabled che IsOnOffUnitSeriesEnabled sono a false"));
                return false;
            }

            return false;
        }

        private bool VSE_Algo(PeriodicUnit unita, DateTime ultimaEsecuzione, DateTime dataAttuale, WeekDay giornoInizioSett, int NrUnitàProssimaEsec, ref string firmaEsecuzione)
        {
            if (ultimaEsecuzione == DateTime.MinValue) { return true; }
            //ATTENZIONE da qui in poi il Return True deve stare solo alla fine

            DateTime dataMinProssEsecuzione = DateTime.MinValue;
            switch (unita)
            {
                case PeriodicUnit.Daily:
                    dataMinProssEsecuzione = new DateTime(ultimaEsecuzione.Year, ultimaEsecuzione.Month, ultimaEsecuzione.Day).AddDays(NrUnitàProssimaEsec); //la parte time deve essere 00:00:00.000
                    if (dataAttuale < dataMinProssEsecuzione) return false;
                    break;
                case PeriodicUnit.Weekly:
                    int giorniPerInizioSettPross = (int)giornoInizioSett - (int)ultimaEsecuzione.DayOfWeek;

                    if (giorniPerInizioSettPross < 1) giorniPerInizioSettPross += 7;

                    //If giorniPerInizioSettPross < 0 Then
                    //    giorniPerInizioSettPross = Math.Abs(giorniPerInizioSettPross)
                    //ElseIf giorniPerInizioSettPross = 0 Then
                    //    giorniPerInizioSettPross = 7
                    //End If

                    int giorniMancanti = giorniPerInizioSettPross + (7 * (NrUnitàProssimaEsec - 1)); //(ogniTotUnità - 1) poichè addizionando giorniPerInizioSettPross già salto di una settimana

                    dataMinProssEsecuzione = new DateTime(ultimaEsecuzione.Year, ultimaEsecuzione.Month, ultimaEsecuzione.Day).AddDays(giorniMancanti); //la parte time deve essere 00:00:00.000

                    if (dataAttuale < dataMinProssEsecuzione) return false;
                    break;
                case PeriodicUnit.Monthly:
                    int meseProssimo = ultimaEsecuzione.Month + NrUnitàProssimaEsec;
                    int annoProssimo = ultimaEsecuzione.Year;

                    while (meseProssimo > 12)
                    {
                        meseProssimo -= 12;
                        annoProssimo += 1;
                    }

                    dataMinProssEsecuzione = new DateTime(annoProssimo, meseProssimo, 1); //la parte time deve essere 00:00:00.000

                    if (dataAttuale < dataMinProssEsecuzione) return false;
                    break;

                case PeriodicUnit.Yearly:
                    dataMinProssEsecuzione = new DateTime(ultimaEsecuzione.Year + NrUnitàProssimaEsec, 1, 1); //la parte time deve essere 00:00:00.000
                    if (dataAttuale < dataMinProssEsecuzione) return false;
                    break;

                default:
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Valore disatteso per unità:<" + unita.ToString() + ">"));
                    break;
            }

            firmaEsecuzione += "data:" + dataMinProssEsecuzione + "|";
            return true;
        }

        private int VSE_NrUnitàPerProssimaEsecuzione()
        {
            indiceUltimaEsecAggiornato = indiceUltimaEsec;
            nrUnitàUltimaEsecAggiornato = nrUnitàUltimaEsec;

            OnOff onOff = OnOffUnitSeries[indiceUltimaEsec];
            int restoOn = onOff.On - nrUnitàUltimaEsecAggiornato;

            if (restoOn > 0)
            { //Sono ancora nel periodo di On e davanti ho almeno ancora una unità di On
                nrUnitàUltimaEsecAggiornato += 1;
                indiceUltimaEsecAggiornato = indiceUltimaEsec;
                return 1;
            } //Se sono arrivato fino a qui significa (restoOn <= 0) che con l'ultima esecuzione ho finito il periodo di On e davanti ho il periodo off, quindi adesso deve comunicare quante unità mancano per la prossima fase on(esecuzione)


            if ((indiceUltimaEsec + 1) > (OnOffUnitSeries.Count - 1)) indiceUltimaEsecAggiornato = 0; //Verifico se sono arrivato all'ultima serie di OnOff, se si imposto indiceUltimaEsecAggiornato = 0 per riiniziare dalla 1° serie
            else indiceUltimaEsecAggiornato += 1;

            nrUnitàUltimaEsecAggiornato = 1;                                                            //se no incremento l'indiceUltimaEsecAggiornato di 1

            return onOff.Off + 1;  //+1 perchè devo saltare tutto il periodo di Off 
        }

        public void AggiornaIndiciUltimaEsec()
        {
            indiceUltimaEsec = indiceUltimaEsecAggiornato;
            nrUnitàUltimaEsec = nrUnitàUltimaEsecAggiornato;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public enum PeriodicUnit
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
