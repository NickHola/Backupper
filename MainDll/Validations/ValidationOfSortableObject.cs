using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Main.Binds;
using Main.Logs;

namespace Main.Validations
{
    public class ValidationOfSortableObject : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            BindingExpression bindingExpr = (BindingExpression)value;
            string nomeProp = bindingExpr.ResolvedSourcePropertyName;
            object ogg = bindingExpr.DataItem;

            if (typeof(ISortBindObj).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo dell'oggetto implementa ISortBindObj
                Log.main.Add(new Mess(LogType.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement ISortBindObj", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }
            if (typeof(IValidation).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo degli oggetti T implementa l'interfaccia Validation
                Log.main.Add(new Mess(LogType.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement IValidation", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            try {
                ValidationResult validationResult = (ogg as IValidation).ValidMySelf(nomeProp: nomeProp); //New ValidationResult(False, "Non va bene")
                return validationResult;
            } catch (Exception ex) {
                Log.main.Add(new Mess(LogType.ERR, "", "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

        }
    }
}
