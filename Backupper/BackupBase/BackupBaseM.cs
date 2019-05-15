using System;
using Newtonsoft.Json;
using Main.FSes;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Main.Validations.Validation;
using System.Windows.Controls;
using Main.Validations;
using Main;
using Main.MsgBxes;

namespace Backupper
{
    [Serializable]
    public abstract class BackupBaseM : INotifyPropertyChanged, IValidation
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

        public ValidationResult ValidMySelf(string propName = "")
        {

            //var x = BackupsM.Instance;

            ValidationResult derivedClassResult = DerivedClassValiditation(propName);

            if (derivedClassResult.IsValid == false)
            {
                this.IsValid = derivedClassResult.IsValid;
                return derivedClassResult;
            }

            bool result = true;
            string errDesc = "";

            if (propName == "" || propName == "Name")
            {
                if (BackupsM.Instance.CheckIfBackupNameExist(this.name))
                {
                    result = false;
                    errDesc = "Name alredy exist";
                }
            }

            if (propName == "")
            {
                this.IsValid = result;
            }
            else
            {
                if (result == false) this.IsValid = result;
            }

            return new ValidationResult(result, errDesc);
        }
        #endregion

        string name, destinationFolder;
        bool isEnabled;
        BackupStates state;
        Progressione progress;
        FilesSelectorM filesSelectorM;
        protected Thread thrCompression;

        public string Name
        {
            get { return name; }
            set
            {
                if (value.Length > 20) throw new Exception("Maximum backup name length allowed is 20");
                if (value != name && BackupsM.Instance.CheckIfBackupNameExist(value)) throw new Exception("Backup name already exists");
                name = value;
                OnPropertyChanged();
            }
        }
        public string DestinationFolder
        {
            get { return destinationFolder; }
            set
            {
                if (System.IO.Directory.Exists(value) == false) throw new Exception("Directory doesn't exist");
                destinationFolder = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                CtrlValue(value);
                isEnabled = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public BackupStates State
        {
            get { return state; }
            set
            {
                CtrlValue(value);
                state = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public Progressione Progress
        {
            get { return progress; }
            set
            {
                CtrlValue(value);
                progress = value;
                OnPropertyChanged();
            }
        }
        public FilesSelectorM FilesSelectorM
        {
            get { return filesSelectorM; }
            set
            {
                filesSelectorM = value;
                if (filesSelectorM != null)
                {
                    filesSelectorM.PropertyChanged -= filesSelectorMPropertyChanged;
                    filesSelectorM.PropertyChanged += filesSelectorMPropertyChanged;
                }
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public Thread ThrCompression
        {
            get { return thrCompression; }
            set
            {
                thrCompression = value;
                OnPropertyChanged();
            }
        }

        public abstract event EventHandler<BackupCompressionResult> CompressionEnd;

        public BackupBaseM(string name = null)
        {
            this.IsEnabled = true;
            this.Name = name ?? BackupsM.Instance.GetAvailableBackupName();
            this.progress = new Progressione();
            this.DestinationFolder = @"C:\";
        }

        private void filesSelectorMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(filesSelectorM));
        }

        public void Delete()
        {
            if (MsgBx.Show("", "Do you want to delete " + Name + " backup?", MsgBxPicture.Question, MsgBxButtonSet.YesNo) == MsgBxButton.Yes)
                BackupsM.Instance.Remove(this);
        }

        public abstract BackupCompressionResult StartStopCompression(Guid guid = default); //object inputParam, out object outParam

        protected abstract ValidationResult DerivedClassValiditation(string propName);

        public void SetDestinationFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                DestinationFolder = dialog.SelectedPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
