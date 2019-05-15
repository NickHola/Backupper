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
    //SortableObjDataGridRow
    public class ValidationOfDataGridRowOfSortableObj : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            BindingGroup bindingGrp = (BindingGroup)value;

            if (bindingGrp.Items.Count == 0) {
                Log.main.Add(new Mess(Tipi.ERR, "", "bindingGrp.Items.Count == 0", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            object ogg = bindingGrp.Items[0];

            if (typeof(ISortBindObj).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo dell'oggetto implementa ISortBindObj
                Log.main.Add(new Mess(Tipi.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement ISortBindObj", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }
            if (typeof(IValidation).IsAssignableFrom(ogg.GetType()) == false) { //Verifico se il tipo degli oggetti T implementa l'interfaccia SortBindObj
                Log.main.Add(new Mess(Tipi.ERR, "", "ogg.GetType():<" + ogg.GetType().ToString() + "> doesn't implement IValidation", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }

            ValidationResult validationResult; 

            try {
                (ogg as ISortBindObj).SincroValidazioneRiordino = SincroValidazioneRiordino.InValidazione;
                validationResult = (ogg as IValidation).ValidMySelf();
            } catch (Exception ex) {
                Log.main.Add(new Mess(Tipi.ERR, "", "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return new ValidationResult(false, "Internal exception, see log");
            }
             finally {
                (ogg as ISortBindObj).SincroValidazioneRiordino = SincroValidazioneRiordino.ValidazioneTerminata;
            }
            return validationResult;
        }
    }
}
