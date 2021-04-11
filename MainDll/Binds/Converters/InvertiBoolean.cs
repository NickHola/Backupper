using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Main.Binds
{
    class InvertiBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Il target deve essere boolean")));

            if (value == null) throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Rcevuto param value a nothing")));

            try
            {
                return !System.Convert.ToBoolean(value);
            }
            catch (Exception ex)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">")));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", Converters.errCnvBackNotImpl)));
        }
    }
}
