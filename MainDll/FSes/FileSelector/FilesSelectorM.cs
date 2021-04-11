using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Main.Binds;
using Main.Logs;
using Main.Thrs;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using static Main.Validations.Validation;
using Main.MsgBxes;

namespace Main.FSes
{
    [Serializable]
    public class FilesSelectorM : INotifyPropertyChanged
    {
        BindList<PathWithFilters> pathsWithFilters;
        BindingList<string> filesSelected;
        Type filterType;


        public BindList<PathWithFilters> PathsWithFilters
        {
            get { return pathsWithFilters; }
            set
            {
                CtrlValue(value);
                pathsWithFilters = value;
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
        public Type FilterType
        {
            get { return filterType; }
            private set
            {
                CtrlValue(value);
                filterType = value;
            }
        }
        public event EventHandler CalculationEnd;

        [JsonConstructor]
        public FilesSelectorM(Type filterType, BindList<PathWithFilters> pathsWithFilters = null)
        {
            FilterType = filterType;
            this.PathsWithFilters = pathsWithFilters != null ? pathsWithFilters : new BindList<PathWithFilters>();

            this.pathsWithFilters.ItemNumberChanged += PathWithFiltersPropertyChanged;
            this.pathsWithFilters.ObjectTPropertyChanged += PathWithFiltersPropertyChanged;
        }

        private void PathWithFiltersPropertyChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PathsWithFilters));
        }

        //private void PathsWithFiltersChanged(object sender, BindListEventArgs e)
        //{
        //    OnPropertyChanged(nameof(PathsWithFilters));
        //    ((PathWithFilters)e.oggetto).PropertyChanged += PathWithFiltersPropertyChanged;
        //}

        //private void PathWithFiltersPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(PathsWithFilters));
        //}


        public void SetSelectedPath(PathWithFilters selectedPath)
        {
            string msgBxResult = MsgBx.Show("", "Do you want to select file or folder?", MsgBxPicture.Info, new List<string> { "File", "Folder", "Cancel" });
            if (msgBxResult == "File")
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = false;
                bool? result = dlg.ShowDialog();

                if (result != null && result == true)
                    selectedPath.Path = dlg.FileName;
            }
            else if (msgBxResult == "Folder")
            {

                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                    selectedPath.Path = dialog.SelectedPath;
            }
        }

        public BindingList<string> TestFileList()
        {
            return CalculateFileListCore();
        }

        public void CalculateFileList(Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType._Nothing, Log.main.warnUserText);
            this.filesSelected = CalculateFileListCore(logMess);
        }

        private BindingList<string> CalculateFileListCore(Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType._Nothing, Log.main.warnUserText);
            BindingList<string> filesSelected = new BindingList<string>();

            foreach (PathWithFilters PathWithFilter in pathsWithFilters)
            {
                if (PathWithFilter.CalculateFileList() == true)
                {
                    foreach (string file in PathWithFilter.FilesSelected)
                        if (filesSelected.Contains(file) == false) filesSelected.Add(file);
                }
            }

            CalculationEnd?.Invoke(this, new EventArgs());
            return filesSelected;
        }



        //private void ThrCalcolaListaFile(Mess logMess) {

        //    Thr.SbloccaThrPadre();

        //    FilesList = new List<String>();
        //    List<String> filesGruppo;

        //        foreach (FolderWithFilterCriteria gruppoCriteri in listaCartellaCriteri) {
        //        if (Directory.Exists(gruppoCriteri.cartella) == false) {
        //            logMess.testoDaVisual = "La cartella:<" + gruppoCriteri.cartella + ">, non esiste";
        //            Log.main.Add(logMess);
        //            continue;
        //        }

        //        filesGruppo = Directory.GetFiles(gruppoCriteri.cartella, "*", (SearchOption)(int)gruppoCriteri.profondita).ToList();


        //        foreach (string file in filesGruppo) { //Scorro tutti i file
        //            string stringaDaValidare; bool daIncludere;
        //            daIncludere = true;

        //            foreach (PathFilterBase criterio in gruppoCriteri.listaCriteri) { //Scorro tutti i criteri su quel file e alla fine decido se è da includere o no

        //                switch (criterio.tipoAnalisi) {
        //                    case PathScope.PathFolderOnly:
        //                        stringaDaValidare = Path.GetFullPath(file);
        //                            break;
        //                    case PathScope.FileNameOnly:
        //                        stringaDaValidare = Path.GetFileName(file);
        //                        break;
        //                    case PathScope.FullPath:
        //                        stringaDaValidare = file;
        //                        break;
        //                    default:
        //                        logMess.testoDaVisual = "Valore sconosciuto per criterio.tipoAnalisi:<" + criterio.tipoAnalisi.ToString() + ">";
        //                        Log.main.Add(logMess);
        //                        continue;
        //                  }

        //                if (Regex.IsMatch(stringaDaValidare, criterio.regex) == true) {
        //                    if (criterio.tipoRegex == SelectionBehavior.Exclusive) {
        //                        daIncludere = false;
        //                        break;
        //                    }
        //                } else {
        //                    if (criterio.tipoRegex == SelectionBehavior.Inclusive) {
        //                        daIncludere = false;
        //                        break;
        //                    }
        //                }
        //            } //For each criteri

        //            if (daIncludere == true && FilesList.Contains(file) == false) FilesList.Add(file); //se già sta nella lista significa che 2 o più criteri pescano lo stesso file, ma se anche solo 1 lo include...
        //                //                                                                                        il file rimane incluso

        //            } //For filesGruppo

        //        } //For listaGruppiCriteri

        //    CalcoloTerminato?.Invoke(this, new EventArgs());

        //    }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
