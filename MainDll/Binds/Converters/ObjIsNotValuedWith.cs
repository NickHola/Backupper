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
    public class ObjIsNotValuedWith : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value != parameter;
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
