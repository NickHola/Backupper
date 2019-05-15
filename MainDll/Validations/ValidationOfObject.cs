using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Main.Logs;
using Main.Validations;

namespace Main.Validations
{
    public class ValidationOfObject : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {

            BindingExpression bindingExpr = (BindingExpression)value;
            string nomeProp = bindingExpr.ResolvedSourcePropertyName;
            object ogg = bindingExpr.DataItem;

            if (typeof(IValidation).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo degli oggetti T implementa l'interfaccia Validation
                Log.main.Add(new Mess(Tipi.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement IValidation", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            try {
                ValidationResult validationResult = (ogg as IValidation).ValidMySelf(nomeProp: nomeProp); //New ValidationResult(False, "Non va bene")
                return validationResult;
            } catch (Exception ex) {
                Log.main.Add(new Mess(Tipi.ERR, "", "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }
        }
    }
}
