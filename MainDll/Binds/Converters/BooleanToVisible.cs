using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Main.Logs;

namespace Main.Binds
{
    public class BooleanToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility)) throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "The target must be a Visibility"))); 

            if (value == null) throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Rcevuto param value a nothing")));


            if (value.GetType() == typeof(Boolean))
            {
                if ((Boolean)value == true) return Visibility.Visible;
                if ((Boolean)value == false) return Visibility.Hidden;
            }

            if (value.IsNumeric() == false)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Rcevuto param value non booleano ne numerico")));
            }
            switch (value)
            {
                case 0:
                    return Visibility.Hidden;
                case 1:
                    return Visibility.Visible;
                case 2:
                    return Visibility.Collapsed;
                default:
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Ricevuto valore numerico disatteso per param value:<" + value.ToString() + ">")));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", Converters.errCnvBackNotImpl)));
        }
    }
}
