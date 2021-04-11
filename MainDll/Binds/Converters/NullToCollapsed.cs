using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Main.Binds
{
    public class NullToCollapsed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility)) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Il target deve essere Visibility")));

            if (value == null) {
                return Visibility.Collapsed;
            } else {
                if (parameter == null || parameter.GetType() != typeof(Visibility)) {
                    return Visibility.Visible;
                } else {
                    return parameter;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(LogType.ERR, "", Converters.errCnvBackNotImpl)));
        }
    }
}
