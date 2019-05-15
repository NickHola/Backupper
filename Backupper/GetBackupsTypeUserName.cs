using Main;
using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Backupper
{
    public class GetBackupsTypeUserName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var vmBackupTypes = from tmp in Assembly.GetExecutingAssembly().GetTypes()
                                    where tmp.IsClass && tmp.BaseType == typeof(BackupBaseVM)
                                    select tmp;

                //List<string> backupsTypeName = (from tmpTypes in vmBackupTypes select tmpTypes.).ToList();
                SortedList<int, string> backupsTypeName = new SortedList<int, string>();

                foreach (Type type in vmBackupTypes)
                {
                    PropertyInfo propInfoName = type.GetProperty("BackupTypeName");
                    PropertyInfo propInfoOrder = type.GetProperty("BackupTypeOrder");
                    backupsTypeName.Add((int)propInfoOrder.GetValue(null), (string)propInfoName.GetValue(null));
                }

                return backupsTypeName.Values.ToList();
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
