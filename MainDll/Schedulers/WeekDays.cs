using Main.Binds;
using Main.DataOre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Schedulers
{
    //ATTENZIONE: non si può fare WeekDays direttamente BindingList<WeekDay> con l'enum nel T1 della BindingList poichè alla combobox gli serve 
    public class WeekDays : BindingList<WeekDayObj>
    {
        public WeekDays()
        {

        }
        public WeekDays(List<WeekDayObj> weekDays)
        {
            foreach (WeekDayObj day in weekDays)
                Add(day);
        }
    }

    public class WeekDayObj : INotifyPropertyChanged
    {
        private WeekDay selectedDay;

        public WeekDay SelectedDay
        {
            get { return selectedDay; }
            set
            {
                selectedDay = value;
                OnPropertyChanged();
            }
        }

        public WeekDayObj() { }

        public WeekDayObj(WeekDay selectedDay)
        { SelectedDay = selectedDay; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
