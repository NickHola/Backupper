using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Main.Logs;
using Main.Validations;

namespace Main.Validations
{
    public class ValidationOfDataGridRow : ValidationRule
    {
         
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            BindingGroup bindingGrp = (BindingGroup)value;

            if (bindingGrp.Items.Count == 0) {
                Log.main.Add(new Mess(LogType.ERR, "", "bindingGrp.Items.Count == 0", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            Object ogg = bindingGrp.Items[0];

            if (typeof(IValidation).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo degli oggetti T implementa l'interfaccia SortBindObj
                Log.main.Add(new Mess(LogType.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement IValidation", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            ValidationResult validationResult; 

            try {
                validationResult = (ogg as IValidation).ValidMySelf();

            } catch (Exception ex) {
                Log.main.Add(new Mess(LogType.ERR, "", "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            return validationResult;
        }
    }
}
