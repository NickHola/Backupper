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
    public class ObjTypeIs : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter a null")));

            if (parameter.GetType().FullName != "System.RuntimeType")
                throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter di tipo:<" + parameter.GetType().ToString() + ">, deve essere di tipo Type")));

            try
            {
                if (value == null)
                {
                    if (parameter == null) return true;
                    else return false;
                }
                if (parameter == null)
                {
                    if (value == null) return true;
                    else return false;
                }

                return value.GetType().FullName == ((Type)parameter).FullName ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">")));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, "", Converters.errCnvBackNotImpl)));
        }
    }
}
