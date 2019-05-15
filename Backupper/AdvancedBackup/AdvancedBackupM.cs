using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Main.FSes;
using System.Windows.Controls;
using Main.Schedulers;
using System.Security.Cryptography;
using Main.MsgBxes;
using Main.Zips;
using static Main.Validations.Validation;
using Main.Logs;

namespace Backupper
{
    [Serializable]
    public class AdvancedBackupM : BackupBaseM
    {
        SchedulerM schedulerM;
        bool checkMD5Files;
        [JsonProperty] Dictionary<string, byte[]> MD5Files;

        public override event EventHandler<BackupCompressionResult> CompressionEnd;

        public SchedulerM SchedulerM
        {
            get { return schedulerM; }
            set
            {
                schedulerM = value;
                schedulerM.Esegui -= Scheduler_Esegui;
                schedulerM.Esegui += Scheduler_Esegui; //Durante la deserializzazione viene assegnato un nuovo SchedulerM e quindi bisogna rieffettuare la sottoscrizione all'evento.
                OnPropertyChanged();
            }
        }
        public bool CheckMD5Files
        {
            get { return checkMD5Files; }
            set
            {
                CtrlValue(value);
                checkMD5Files = value;
                OnPropertyChanged();
            }
        }

        [JsonConstructor]
        public AdvancedBackupM(string name = null) : base(name)
        {
            this.State = BackupStates.Idle;

            this.CheckMD5Files = true;
            this.MD5Files = new Dictionary<string, byte[]>();
            FilesSelectorM = new FilesSelectorM(typeof(StringMatchPathFilterM));
            SchedulerM = new SchedulerM(intervalloVerificaMs: 10000);
        }

        private void Scheduler_Esegui(object sender, EventArgs e)
        {
            if (this.State == BackupStates.Idle) StartStopCompression();
        }

        public override BackupCompressionResult StartStopCompression(Guid guid = default)
        {
            if (guid == null) guid = Guid.NewGuid();

            switch (this.State)
            {
                case BackupStates.Idle:
                    return StartCompression(guid);

                case BackupStates.Compressing:
                    if (this.ThrCompression != null && this.ThrCompression.IsAlive == true)
                    {
                        if (MsgBx.Show("", "Kill compression thread could cause app crash, do you want continue?", MsgBxPicture.Alert, MsgBxButtonSet.YesNo) == MsgBxButton.No) return null;
                        this.State = BackupStates.WaitToStop;
                        //this.IsInWaitToStop = true;
                        ThrCompression.Abort();
                        DateTime startTime = DateTime.Now;

                        while (ThrCompression.IsAlive)
                        {
                            System.Threading.Thread.Sleep(30);
                            if ((DateTime.Now - startTime).TotalMilliseconds > SettingsM.Instance.TimeoutStopCompressingSignalMs) break;
                        }

                        if (ThrCompression.IsAlive == false)
                        {
                            this.State = BackupStates.Idle;
                            this.Progress.Percentage = 0;
                            //this.IsInWaitToStop = false;
                            //this.InCompressing = false;
                        }
                    }
                    return null;

                case BackupStates.SavingInFTP:
                    return null;

                case BackupStates.WaitToStop:
                    MsgBx.Show("", "Backup is already in wait to stop", MsgBxPicture.Info);
                    return null;

                default:
                    Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "Unexpected value for State:<" + State.ToString() + ">"));
                    return null;
            }
        }

        private BackupCompressionResult StartCompression(Guid guid)
        {
            BackupCompressionResult args;
            bool filesAreChanged = false;
            bool compressResult;
            Dictionary<string, byte[]> newMD5Files = new Dictionary<string, byte[]>();

            try
            {
                string fullPath;

                this.State = BackupStates.FilesToBackupCalculation;
                FilesSelectorM.CalculateFileList();

                if (checkMD5Files == true)
                {
                    this.State = BackupStates.MD5Calculation;
                    CalcMD5Files(out filesAreChanged, out newMD5Files);
                    if (filesAreChanged == false)
                    {
                        //outParam["filesAreChanged"] = filesAreChanged;
                        args = new BackupCompressionResult(compressionResult: true, filesAreChanged: filesAreChanged);
                        this.CompressionEnd?.Invoke(this, args);
                        return args;
                    }
                }

                if (FilesSelectorM.FilesSelected.Count == 0)
                {
                    args = new BackupCompressionResult(compressionResult: true, filesAreChanged: filesAreChanged, filesToBackup: 0);
                    this.CompressionEnd?.Invoke(this, args);
                    return args;
                }

                do
                {
                    string prefix = DateTime.Now.ToString("yy-MM-dd_HH.mm.ss_");
                    fullPath = System.IO.Path.Combine(this.DestinationFolder, prefix + this.Name);
                } while (System.IO.File.Exists(fullPath));

                this.State = BackupStates.Compressing;

                compressResult = Zip.Comprimi(FilesSelectorM.FilesSelected.ToList(), fullPath, out this.thrCompression, progressione: this.Progress);

                args = new BackupCompressionResult(compressionResult: compressResult, filesAreChanged: filesAreChanged, filesToBackup: FilesSelectorM.FilesSelected.Count);

                if (compressResult == true && checkMD5Files == true && filesAreChanged == true) this.MD5Files = newMD5Files;

                this.CompressionEnd?.Invoke(this, args);
                return args;
            }
            catch
            {
                args = new BackupCompressionResult(compressionResult: false, filesAreChanged: filesAreChanged, filesToBackup: FilesSelectorM.FilesSelected.Count);
                this.CompressionEnd?.Invoke(this, args);
                return args;
            }
            finally
            {
                this.State = BackupStates.Idle;
                this.Progress.Percentage = 0;
            }
        }

        private void CalcMD5Files(out bool filesAreChanged, out Dictionary<string, byte[]> newMD5Files)
        {
            filesAreChanged = false;
            newMD5Files = new Dictionary<string, byte[]>();

            MD5 md5 = MD5.Create();
            byte[] tmpFile;

            foreach (string fileSelected in FilesSelectorM.FilesSelected)
            {
                if (System.IO.File.Exists(fileSelected))
                    tmpFile = System.IO.File.ReadAllBytes(fileSelected);
                else
                    continue;


                byte[] hash = md5.ComputeHash(tmpFile);
                newMD5Files.Add(fileSelected, hash);

                if (filesAreChanged == true) continue;

                if (this.MD5Files.ContainsKey(fileSelected) == false)
                    filesAreChanged = true;
                else
                    if (!this.MD5Files[fileSelected].SequenceEqual(hash)) filesAreChanged = true;
            }
        }

        protected override ValidationResult DerivedClassValiditation(string propName)
        {
            return new ValidationResult(true, "");
        }

    }
}
