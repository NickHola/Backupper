using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Schedulers
{
    [Serializable]
    public class SchedulerVM : INotifyPropertyChanged
    {
        public SchedulerM SchedulerM { get; set; } 
        string testResult;
        ViewStates viewState;

        public string TestResult
        {
            get { return testResult; }
            set
            {
                testResult = value; //ctrlValue non serve, la imposto solo io da codice
                OnPropertyChanged();
            }
        }
        public ViewStates ViewState
        {
            get { return viewState; }
            set
            {
                viewState = value;
                OnPropertyChanged();
            }
        }


        public SchedulerVM()
        {
            SchedulerM = new SchedulerM();
        }

        public SchedulerVM(SchedulerM schedulerM) //SchedulerM parameter constructor for CreateVmFromM Converter
        {
            SchedulerM = schedulerM;
        }


        async internal void TestScheduler()
        {
            ViewState = ViewStates.InTestCalculation;
            StringBuilder result = new StringBuilder();

            SchedulerM.resetFirmeUltimeEsecuzione();
            Task<SortedDictionary<DateTime, string>> taskThread = Task<SortedDictionary<DateTime, string>>.Factory.StartNew(() => SchedulerM.TestScheduler(false));
            await taskThread;

            foreach (KeyValuePair<DateTime, string> date in taskThread.Result)
                result.Append(date.Value + " - " + date.Key.DayOfWeek + Util.crLf);

            //TestResult += date.Value + " - " + date.Key.DayOfWeek + Util.crLf;
            TestResult = result.ToString().RemoveFinal(Util.crLf);

            ViewState = ViewStates.InTestResultShowing;
        }


        internal void CloseTest()
        {
            ViewState = ViewStates.Normal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
