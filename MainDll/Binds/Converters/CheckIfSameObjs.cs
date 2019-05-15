using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Main.Binds
{
    public class CheckIfSameObjs : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return false;

            if (values[0] == null)
            {
                for (int i = 1; i < values.Length; i++)
                    if (values[i] == null) return false;
            } else {
                for (int i = 1; i < values.Length; i++)
                    if (!values[0].Equals(values[i])) return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
