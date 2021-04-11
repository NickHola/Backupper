using Main;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Main.Validations.Validation;
using Main.Salvable;
using Main.Serializes;
using Main.Logs;

namespace Backupper
{
    [Serializable]
    public class BackupsM : BindingList<BackupBaseM>, INotifyPropertyChanged, ISavable //Singleton class
    {

        #region ISavable implementation
        string savableName, savableParentName;
        public bool SavableMarkUser { get; }
        public bool SavableMarkPcName { get; }

        public string SavableName
        {
            get
            {
                return savableName;
            }
            set
            {
                CtrlValue(value);
                savableName = value;
            }
        }

        public string SavableParentName
        {
            get
            {
                return savableParentName;
            }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                savableParentName = value;
            }
        }

        #endregion

        private static Lazy<BackupsM> instance = new Lazy<BackupsM>(() => new BackupsM()); //Thread-safe singleton
        bool isEnabled;
        static private bool inLoading;

        public static BackupsM Instance
        {
            get
            {
                return instance.Value;
            }
        }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        [JsonConstructor]
        private BackupsM()
        {
            this.SavableName = "Backups";
            this.SavableParentName = "";
            this.SavableMarkUser = false;
            this.SavableMarkPcName = false;

            this.ListChanged += MyListChanged;
        }

        private void MyListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                BackupBaseM backup = ((BackupsM)sender)[e.NewIndex];
                backup.PropertyChanged += BackupBasePropertyChanged;
                if (inLoading == false) Task.Run(() => SaveBackups());
            }
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                Task.Run(() => SaveBackups());
            }
        }

        private void BackupBasePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Task.Run(() => SaveBackups());
        }

        private void SaveBackups()
        {
            switch (SettingsM.Instance.SaveBackupIn)
            {
                case BackupsSaveLocation.File:
                    this.Save(BackupsConfigFile.Instance);
                    BackupsConfigFile.Instance.SaveOnFile(false);
                    break;
                case BackupsSaveLocation.RESTService:
                    string serialization = "";
                    if (Serialize.SerializeInText(this, ref serialization) == false) return;
                    RESTBackups restBackups = new RESTBackups();
                    //restBackups.PutBackupsAsync(serialization);

                    Task put = restBackups.PutBackupsAsync(serialization);
                    //await put;
                    try { put.Wait(); }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            throw ex.InnerException;
                        else
                            throw ex;
                    }
                    break;
                case BackupsSaveLocation.Db:
                    break;
                default:
                    Log.main.Add(new Mess(LogType.ERR, "", $"Received unexpected value for SettingsM.Instance.SaveBackupIn:<{SettingsM.Instance.SaveBackupIn}>"));
                    break;
            }
        }

        public bool CheckIfBackupNameExist(string backupName)
        {
            //if ((from tmp in this where tmp.Model.Name.ToLower() == backupName.ToLower() select tmp).Count() > 0) return true;
            if ((from tmp in this where tmp.Name.ToLower() == backupName.ToLower() select tmp).Count() > 0) return true;

            return false;
        }

        public string GetAvailableBackupName()
        {
            string finalName;
            string backupName = finalName = "NewBackup";
            UInt64 numb = 0;
            while (CheckIfBackupNameExist(finalName) == true)
            {
                numb += 1;
                finalName = backupName + numb;
            }
            return finalName;
        }

        public bool Load(object source, Mess logMess = null) //Necessario poichè singleton
        {
            bool inErr = false;
            try
            {
                inLoading = true;
                if (source.GetType() == typeof(string))
                {
                    string backups = (string)source;
                    inErr = Serialize.DeserializeFromText(backups, ref instance);
                }
                else
                {
                    instance = new Lazy<BackupsM>(() => (BackupsM)this.Load(source, out inErr, logMess));
                }
            }
            catch (Exception ex)
            {
                inErr = true;
                Log.main.Add(new Mess(LogType.Warn, "", $"Exception ex.mess:<{ex.Message}>"));
            }
            finally
            {
                inLoading = false;
            }

            return inErr;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
