using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Main.Regexes;
using static Main.Validations.Validation;

namespace Main.FSes
{
    [Serializable]
    public class RegexPathFilterM : PathFilterBase
    {
        string regexStr;
        public string RegexStr
        {
            get
            { return regexStr; }
            set
            {
                CtrlValue(value);
                regexStr = value;
                OnPropertyChanged();
            }
        }
        
        public RegexPathFilterM() : base(indiceOrd: 0,  selectionBehavior: SelectionBehavior.Inclusion, regex: "") { } //Costruttore per il dataGrid
        
        protected override ValidationResult DerivedClassValiditation(string nomeProp = "")
        {
            bool esito = true;
            string descErr = "";

            if (nomeProp == "" || nomeProp.ToLower() == "regexstr")
            {
                if (Regex.CheckRegexSyntax(this.RegexStr) == false)
                {
                    esito = false;
                    descErr = "Sintassi regex non valida";
                }
            }
                    
            return new ValidationResult(esito, descErr);
        }

        override public bool? CheckFilter(string stringToCheck)
        {
            bool match;

            if (Regex.RegexMatch(stringToCheck, RegexStr, out match) == false) return false; 

            return match;
        }

    }
}
