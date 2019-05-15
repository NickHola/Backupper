using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

//Esempio utilizzo:  <DataTrigger Binding="{Binding ***NomeProp***>, Converter={StaticResource cnvObjTypeNameIs}, ConverterParameter='***NomeDellaClasseOnull***'}" Value="***ScrivereTrueOFalse***">
//                      <Setter ....../> </DataTrigger>


namespace Main.Binds
{
    public class ObjTypeNameIs : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter a null")));

            if (parameter.GetType() != typeof(string))
                throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(Tipi.ERR, Log.main.errUserText, "Ricevuto parameter di tipo:<" + parameter.GetType().ToString() + ">, deve essere una stringa")));

            try
            {
                if (value == null)
                    return (string)parameter == "null" ? true : false;


                if (value.GetType() == typeof(Type).GetType()) //The type of value parameter is Type, in this case compare the property Name
                    return (value as Type).Name == (string)parameter ? true : false;


                return (value.GetType().Name == (string)parameter) ? true : false;
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
