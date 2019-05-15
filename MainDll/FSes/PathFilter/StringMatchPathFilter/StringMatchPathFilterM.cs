using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Main.Validations.Validation;
using Main;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Main.FSes
{
    [Serializable]
    public class StringMatchPathFilterM : PathFilterBase
    {
        string stringToCompair;
        StringMatchType matchType;
        bool isCaseSensitive;

        public string StringToCompair
        {
            get { return stringToCompair; }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                stringToCompair = value;
                OnPropertyChanged();
            }
        }
        public StringMatchType MatchType
        {
            get { return matchType; }
            set
            {
                matchType = value;
                OnPropertyChanged();
            }
        }
        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                isCaseSensitive = value;
                OnPropertyChanged();
            }
        }

        public StringMatchPathFilterM() : base(indiceOrd: 0, selectionBehavior: SelectionBehavior.Exclusion, regex: "") { } //Costruttore per il dataGrid

        //[JsonConstructor] public StringMatchPathFilter() :base (indiceOrd: 0, tipoAnalisi: PathScope.FileNameOnly, tipoRegex: SelectionBehavior.Inclusive, regex: "")
        //{        }

        override public bool? CheckFilter(string stringToCheck)
        {
            ValidMySelf();
            if (IsValid == false) return null;
            bool fileMatch = false;

            switch (matchType)
            {
                case StringMatchType.Contains:
                    if (isCaseSensitive == true)
                    { if (stringToCheck.Contains(stringToCompair)) fileMatch = true; }
                    else
                    { if (stringToCheck.ContainsIngnoreCase(stringToCompair)) fileMatch = true; }
                    break;

                case StringMatchType.StartWith:
                    if (isCaseSensitive == true)
                    { if (stringToCheck.StartsWith(stringToCompair)) fileMatch = true; }
                    else
                    { if (stringToCheck.StartsWith(stringToCompair, StringComparison.OrdinalIgnoreCase)) fileMatch = true; }
                    break;

                case StringMatchType.EndWith:
                    if (isCaseSensitive == true)
                    { if (stringToCheck.EndsWith(stringToCompair)) fileMatch = true; }
                    else
                    { if (stringToCheck.EndsWith(stringToCompair, StringComparison.OrdinalIgnoreCase)) fileMatch = true; }
                    break;

                case StringMatchType.Exactly:
                    if (isCaseSensitive == true)
                    { if (stringToCheck == stringToCompair) fileMatch = true; }
                    else
                    { if (stringToCheck.ToLower() == stringToCompair.ToLower()) fileMatch = true; }
                    break;
                default:
                    break;
            }

            //Here, I can add other checks


            if (fileMatch == true)
            {
                if (SelectionBehavior == SelectionBehavior.Inclusion)
                    return fileMatch;
                else
                    return !fileMatch;
            }
            else
                return null;


        }

        protected override ValidationResult DerivedClassValiditation(string nomeProp = "")
        {
            bool esito = true;
            string descErr = "";

            if (nomeProp == "" || nomeProp.ToLower() == "stringtocompair")
            {
                if (StringToCompair == null || StringToCompair == "")
                {
                    esito = false;
                    descErr = "String to compair can't be void";
                }
            }

            return new ValidationResult(esito, descErr);
        }


    }
}
