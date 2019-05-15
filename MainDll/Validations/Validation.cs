using Main.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Validations
{
    public static class Validation
    {
        //Public Class IValidation
        //    <JsonIgnore> Property isValid As Boolean
        //    MustOverride Function Validation(Optional nomeProp As String = "") As ValidationResult
        //End Class

        public static bool CtrlValue(Object value, string nomeVar = "value", bool ctrlNothing = true, bool ctrlVoid = true, bool throwEx = true)
        {
            if (ctrlNothing == true && value == null)
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "ricevuto " + nomeVar + " a nothing"));
                if (throwEx == true) throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi._Nothing, "")));
                return false;
            }
            if (value != null && ctrlVoid == true)
            {
                if (value.GetType() == typeof(string))
                {
                    if ((string)value == "")
                    {
                        Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "ricevuto " + nomeVar + " vuoto"));
                        if (throwEx == true) throw new Exception(Excep.ScriviLogInEx(new Mess(Tipi._Nothing, "")));
                        return false;
                    }
                }
                else { }
            }
            return true;
        }
    }
}
