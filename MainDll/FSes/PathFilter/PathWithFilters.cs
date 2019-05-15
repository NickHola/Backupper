using Main.Binds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Main.Validations;
using static Main.Validations.Validation;
using Newtonsoft.Json;
using Main.Logs;

namespace Main.FSes
{
    [Serializable]
    public class PathWithFilters : INotifyPropertyChanged, IValidation
    {

        #region "IValidation"
        private bool isValid;
        public bool IsValid
        { //Implements IValidation.isValid
            get { return isValid; }

            private set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public ValidationResult ValidMySelf(string nomeProp = "")
        {
            bool esito = true;
            string descErr = "";

            if (nomeProp == "" || nomeProp == nameof(this.Path))
            {
                if (isFilePath == false)
                {
                    if (System.IO.Directory.Exists(this.Path) == false)
                    {
                        esito = false;
                        descErr = "Folder path doesn't exist";
                    }
                }
                else
                {
                    if (System.IO.File.Exists(this.Path) == false)
                    {
                        esito = false;
                        descErr = "File doesn't exist";
                    }
                }
            }

            if (nomeProp == "")
                this.IsValid = esito;
            else
                if (esito == false) this.IsValid = esito;


            return new ValidationResult(esito, descErr);
        }
        #endregion

        DirectoryDepth depth;
        InitialSelection initialSelection;
        string path;
        bool isFilePath;
        SortBindList<StringMatchPathFilterM> stringMatchFilters;
        SortBindList<RegexPathFilterM> regexFilters;
        BindingList<string> filesSelected;

        public DirectoryDepth Depth
        {
            get { return depth; }
            set
            {
                CtrlValue(value);
                depth = value;
                OnPropertyChanged();
            }
        }
        public InitialSelection InitialSelection
        {
            get { return initialSelection; }
            set
            {
                initialSelection = value;
                OnPropertyChanged();
            }
        }
        public String Path
        {
            get { return path; }
            set
            {
                path = value;
                IsFilePath = System.IO.File.Exists(path) == true ? true : false;
                OnPropertyChanged();
            }
        }
        public bool IsFilePath
        {
            get { return isFilePath; }
            set
            {
                isFilePath = value;
                OnPropertyChanged();
            }
        }
        public SortBindList<StringMatchPathFilterM> StringMatchFilters
        {
            get { return stringMatchFilters; }
            set
            {
                CtrlValue(value);
                stringMatchFilters = value;
                OnPropertyChanged();
            }
        }
        public SortBindList<RegexPathFilterM> RegexFilters
        {
            get { return regexFilters; }
            set
            {
                CtrlValue(value);
                regexFilters = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public BindingList<string> FilesSelected
        {
            get { return filesSelected; }
            private set
            {
                filesSelected = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler CalculateFileListEnded;

        public PathWithFilters() : this("", DirectoryDepth.AllDirectories, InitialSelection.AllFiles, null, null) { } //Constructor for dataGrid

        [JsonConstructor]
        public PathWithFilters(string path, DirectoryDepth depth, InitialSelection initialSelection, SortBindList<StringMatchPathFilterM> stringMatchFilters, SortBindList<RegexPathFilterM> regexFilters)
        {
            Path = path;
            Depth = depth;
            InitialSelection = initialSelection;
            StringMatchFilters = stringMatchFilters ?? new SortBindList<StringMatchPathFilterM>(); //Per far trovare al dataGrid la lista altrimenti non si possono aggiugere righe nella view
            RegexFilters = regexFilters ?? new SortBindList<RegexPathFilterM>(); //Per far trovare al dataGrid la lista altrimenti non si possono aggiugere righe nella view

            this.stringMatchFilters.ItemNumberChanged += StringMatchFiltersChanged;
            this.stringMatchFilters.ObjectTPropertyChanged += StringMatchFiltersChanged;
            this.regexFilters.ItemNumberChanged += RegexFiltersChanged;
            this.regexFilters.ObjectTPropertyChanged += RegexFiltersChanged;
        }


        private void StringMatchFiltersChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(StringMatchFilters));
        }

        private void RegexFiltersChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(RegexFilters));
        }


        public bool CalculateFileList(Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(Tipi._Nothing, Log.main.warnUserText);
            FilesSelected = new BindingList<string>();

            ValidMySelf();

            if (isValid == false)
            {
                logMess.testoDaLoggare = "Path not valid, Path:<" + Path + ">";
                Log.main.Add(logMess);
                CalculateFileListEnded?.Invoke(this, new GenericEventArgs(descErr: logMess.testoDaLoggare, inErr: true));
                return false;
            }

            try
            {
                if (initialSelection == InitialSelection.AllFiles)
                    foreach (string file in System.IO.Directory.GetFiles(Path, "*", (System.IO.SearchOption)Depth))
                        FilesSelected.Add(file);

                if (StringMatchFilters.Count > 0)
                {
                    if (IsFilePath == true)  //Path is a file
                    {
                        ApplayStringMatchPathFilter(Path);
                    }
                    else  //Path is a directory
                    {
                        foreach (string fileFullPath in System.IO.Directory.GetFiles(Path, "*", (System.IO.SearchOption)Depth))
                            ApplayStringMatchPathFilter(fileFullPath);
                    }
                }

                CalculateFileListEnded?.Invoke(this, new GenericEventArgs());
            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = "ex.mess:<" + ex.Message + ">";
                Log.main.Add(logMess);
                CalculateFileListEnded?.Invoke(this, new GenericEventArgs(descErr: logMess.testoDaLoggare, inErr: true));
                return false;
            }

            return true;
        }

        private void ApplayStringMatchPathFilter(string fileFullPath)
        {
            foreach (StringMatchPathFilterM stringMatchPathFilter in StringMatchFilters)
            {  //StringMatchPathFilte
                bool? filterResult = stringMatchPathFilter.CheckFilter(System.IO.Path.GetFileName(fileFullPath));
                if (filterResult == true)
                { if (FilesSelected.Contains(fileFullPath) == false) FilesSelected.Add(fileFullPath); }
                else if (filterResult == false)
                { if (FilesSelected.Contains(fileFullPath) == true) FilesSelected.Remove(fileFullPath); }
            }
        }

        //private void ApplayRegexPathFilter(string path)
        //{
        //    foreach (RegexPathFilterM regexPathFilter in RegexFilters)
        //    {
        //        if (regexPathFilter.CheckIfFileIsValid(System.IO.Path.GetFileName(Path)) == true) FilesSelected.Add(Path);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
