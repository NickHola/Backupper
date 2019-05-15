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
    public class CreateVmFromM : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //if (value.GetType().BaseType != typeof(BackupBaseM))
                //{ Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "E' stato ricevuto un tipo che non eredita da BackupBaseM, val.GetType.BaseType:<" + value.GetType().BaseType.ToString() + ">")); }

                if (parameter == null) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter a null")));

                if (parameter.GetType().FullName != "System.RuntimeType")
                    throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter di tipo:<" + parameter.GetType().ToString() + ">, deve essere di tipo Type")));


                return Activator.CreateInstance((Type)parameter, value);
            }
            catch (Exception ex)
            {
                throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">")));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, "", Main.Binds.Converters.errCnvBackNotImpl)));
        }
    }
}
