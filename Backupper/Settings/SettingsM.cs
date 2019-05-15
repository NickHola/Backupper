using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Main;
using Main.Configs;
using Main.Salvable;
using Newtonsoft.Json;
using static Main.Validations.Validation;
using Main.Serializes;
using Main.Logs;

namespace Backupper
{
    [Serializable]
    public class SettingsM : INotifyPropertyChanged, ISavable
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

        static SettingsM instance;
        RESTsettingM restSetting;
        BackupsSaveLocation saveBackupIn;
        int timeoutStopCompressingSignalMs;

        public BackupsSaveLocation SaveBackupIn
        {
            get { return saveBackupIn; }
            set
            {
                saveBackupIn = value;
                OnPropertyChanged();
            }
        }
        public RESTsettingM RestSetting
        {
            get { return restSetting; }
            set
            {
                restSetting = value;
                OnPropertyChanged();
            }
        }
        public int TimeoutStopCompressingSignalMs
        {
            get { return timeoutStopCompressingSignalMs; }
            set
            {
                CtrlValue(value);
                timeoutStopCompressingSignalMs = value;
                OnPropertyChanged();
            }
        }
        public static SettingsM Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsM();
                return instance;
            }
        }

        [JsonConstructor]
        private SettingsM()
        {
            this.SavableName = nameof(SettingsM);
            this.SavableParentName = "";
            this.SavableMarkUser = true;
            this.SavableMarkPcName = false;
            this.TimeoutStopCompressingSignalMs = 2000;
            this.RestSetting = new RESTsettingM();
        }

        static public bool DeserializeFromText(string stringaSerializatta, SerializerType tipoSerial = SerializerType.ntsJson, bool visualErr = true)
        {
            return Serialize.DeserializeFromText(stringaSerializatta, ref instance);
        }

        public bool Load(object source, Mess logMess = null)
        {
            instance = (SettingsM)this.Load(source, out bool inErr, logMess);
            return inErr;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
