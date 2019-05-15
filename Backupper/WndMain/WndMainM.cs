using Newtonsoft.Json;
using System;
using Main.Salvable;
using static Main.Validations.Validation;
using Main.Configs;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Backupper
{
    [Serializable]
    public class WndMainM : ISavable, INotifyPropertyChanged
    {
        #region ISavable implementation
        string savableName, savableParentName;
        public bool SavableMarkUser { get; }
        public bool SavableMarkPcName { get; }

        public string SavableName
        {
            get { return savableName; }
            set
            {
                CtrlValue(value);
                savableName = value;
            }
        }

        public string SavableParentName
        {
            get { return savableParentName; }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                savableParentName = value;
            }
        }
        #endregion

        static WndMainM instance;

        private double zoomLevel;

        [JsonIgnore]
        public double ZoomLevel
        {
            get
            { return zoomLevel; }
            set
            {
                zoomLevel = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public static WndMainM Instance
        {
            get
            {
                if (instance == null)
                    instance = new WndMainM();
                return instance;
            }
        }

        [JsonConstructor]
        private WndMainM()
        {
            this.SavableName = nameof(WndMainM);
            this.SavableParentName = "";
            this.SavableMarkUser = false;
            this.SavableMarkPcName = false;

            ZoomLevel = 1.3;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
