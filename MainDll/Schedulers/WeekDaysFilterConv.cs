using Main.Binds;
using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Main.Schedulers
{
    class WeekDaysFilterConv : IMultiValueConverter //IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() < 3) 
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", "values array ha meno di 3 elementi, values.Count:<" + values.Count() + ">")));

            if (values[0].GetType() != typeof(DataOre.WeekDay[]))
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", "l'elemento 0 dell'array values non è un array di DataOre.WeekDay, values[0].GetType:<" + values[0].GetType().ToString() + ">")));

            if (values[1].GetType() != typeof(SchedulerVM))
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", "l'elemento 1 dell'array values non è un SchedulerVM, values[1].GetType:<" + values[1].GetType().ToString() + ">")));

            if (values[2].GetType() != typeof(DataGrid) && !values[2].GetType().IsSubclassOf(typeof(DataGrid)))
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", "l'elemento 2 dell'array values non è un DataGrid, values[2].GetType:<" + values[2].GetType().ToString() + ">")));

            List<DataOre.WeekDay> availableWeekDays = new List<DataOre.WeekDay>();
            availableWeekDays.AddRange((DataOre.WeekDay[])values[0]);
 
            SchedulerVM schedulerVm = (SchedulerVM)values[1];

            DataGrid dataGrid = (DataGrid)values[2];

            foreach (WeekDayObj dayInScheduler in schedulerVm.SchedulerM.WeekDays)
            {
                //ogni giorno già presente nello scheduler lo tolgo dalla lista solo se non è il giorno già valorizzato nella combobox
                if (availableWeekDays.Contains(dayInScheduler.SelectedDay) && (dataGrid.SelectedItem == null || dayInScheduler != (WeekDayObj)dataGrid.SelectedItem))
                    availableWeekDays.Remove(dayInScheduler.SelectedDay);
            }

            if ((from tmp in schedulerVm.SchedulerM.WeekDays where tmp.SelectedDay == ((WeekDayObj)dataGrid.SelectedItem).SelectedDay && tmp != (WeekDayObj)dataGrid.SelectedItem select tmp).Count() > 0)
            { //Nel caso in cui il valore di default dell'enum è già stato preso, cioè presente già nello scheduler e oggetto diverso da quello della cella attuale...
                if (availableWeekDays.Count > 0) //...allora imposto il primo disponibile
                    ((WeekDayObj)dataGrid.SelectedItem).SelectedDay = availableWeekDays[0];
                else //...altrimenti cancello l'attuale elemento poichè non si sono più giorni disponibili
                {
                    WeekDays days = (WeekDays)dataGrid.ItemsSource;
                    days.Remove((WeekDayObj)dataGrid.SelectedItem);
                }
            }
            
            return availableWeekDays.ToArray();
        }

      
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", Converters.errCnvBackNotImpl)));
        }
    }

}
