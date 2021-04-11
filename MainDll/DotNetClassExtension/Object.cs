using Main.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Main
{
    public static partial class DotNetClassExtension
    {
        public static bool IsEqualContent(this object data1, object data2, string[] excluded = null, bool checkFields = true, bool checkProperties = true)
        {
            if (data1 == null && data2 == null) return true;

            if (data1 == null) return false;

            if (data2 == null) return false;

            if (data1.GetType() != data2.GetType())
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "data1.GetType:<" + data1.GetType().Name + "> diverso da data2.GetType:<" + data2.GetType().Name + ">", visualMsgBox: false));
                return false;
            }

            if (checkFields == true)
            {
                foreach (FieldInfo field in data1.GetType().GetFields())
                {
                    if (excluded != null && excluded.Contains(field.Name)) continue;
                    if ((dynamic)field.GetValue(data1) != (dynamic)field.GetValue(data2)) return false; //dynamic poichè il confronto darebbe sempre esito negativo poichè le 2 istruzioni ritornano sempre un oggeto di byte
                                                                                                        //per esempio e non il tipo nativo byte
                }
            }                                                                                           

            if (checkProperties == true)
            {
                foreach (PropertyInfo prop in data1.GetType().GetProperties())
                {
                    if (excluded != null && excluded.Contains(prop.Name)) continue;
                    if ((dynamic)prop.GetValue(data1) != (dynamic)prop.GetValue(data2)) return false;
                }
            }

            return true;
        }

        public static bool IsNumeric(this object obj)
        {
            object converted = null;
            return obj.IsNumeric(ref converted);
        }

        public static bool IsNumeric(this object obj, ref object converted)
        {

            return Double.TryParse(obj.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out _);
            //try  //ATTENZIONE: Non va bene poichè mostra comunque l'eccezione nella finestra di output
            //{   converted = Convert.ToDouble(obj, CultureInfo.InvariantCulture.NumberFormat);
            //    return true;
            //}
            //catch
            //{ return false; }
        }


    }
}
