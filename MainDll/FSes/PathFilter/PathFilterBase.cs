using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Binds;
using Main.Validations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Main.Logs;

namespace Main.FSes
{
    [Serializable]
    public abstract class PathFilterBase : ISortBindObj, IValidation, INotifyPropertyChanged
    {

        #region "ISortBindObj"
        UInt64 indiceOrd; SincroValidazioneRiordino sincroValidazioneRiordino; UInt16 timeOutValidazionMs;
        public UInt64 IndiceOrd
        {
            get { return indiceOrd; }
            set
            {
                indiceOrd = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public SincroValidazioneRiordino SincroValidazioneRiordino
        {
            get { return sincroValidazioneRiordino; }
            set
            {
                sincroValidazioneRiordino = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public UInt16 TimeOutValidazionMs
        {
            get { return timeOutValidazionMs; }
            set
            {
                timeOutValidazionMs = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region "IValida"
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            private set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public ValidationResult ValidMySelf(string nomeProp = "")
        {
            ValidationResult result = DerivedClassValiditation(nomeProp);

            if (result.IsValid == false)
            {
                this.IsValid = result.IsValid;
                return result;
            }

            bool esito = true;
            string descErr = "";

            //if (nomeProp == "" || nomeProp == "blabla")
            //{
            //    if (Regex.SintassiRegex(this.regex) == false)
            //    {
            //        esito = false;
            //        descErr = "Sintassi regex non valida";
            //    }
            //}

            if (nomeProp == "")
            {
                this.IsValid = esito;
            }
            else
            {
                if (esito == false) this.IsValid = esito;
            }

            return new ValidationResult(esito, descErr);
        }
        #endregion

        //PathScope tipoAnalisi;
        SelectionBehavior selectionBehavior;

        public SelectionBehavior SelectionBehavior
        {
            get { return selectionBehavior; }
            set
            {
                selectionBehavior = value;
                OnPropertyChanged();
            }
        }
        //public PathScope TipoAnalisi
        //{
        //    get { return tipoAnalisi; }
        //    set
        //    {
        //        tipoAnalisi = value;
        //        OnPropertyChanged();
        //    }
        //}


        [JsonConstructor]
        public PathFilterBase(UInt64 indiceOrd, SelectionBehavior selectionBehavior, string regex) //: base(tipoRegex: tipoRegex, regex: regex)
        { //Costruttore principale
            this.IndiceOrd = indiceOrd;
            this.SelectionBehavior = selectionBehavior;
            //this.TipoAnalisi = tipoAnalisi;
        }

        //public bool CheckPathValidity(string fullPath)
        //{
        //    string text = fullPath;
        //    try
        //    {
        //        if (TipoAnalisi == PathScope.PathFolderOnly)
        //            text = System.IO.Path.GetDirectoryName(fullPath);
        //        else if (TipoAnalisi == PathScope.FileNameOnly)
        //            text = System.IO.Path.GetFileName(fullPath);
        //    }
        //    catch
        //    {
        //        throw new Exception("Sintassi percorso non valida"); //Intercettata dal Binding che mostrerà l'eccezione
        //    }
        //    return CheckFilter(text);
        //}

        public abstract bool? CheckFilter(string stringToCheck);

        protected abstract ValidationResult DerivedClassValiditation(string nomeProp);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
