using Main.FSes;
using Main.Logs;
using Main.MsgBxes;
using Main.Zips;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Backupper
{
    [Serializable]
    public class SimpleBackupM : BackupBaseM
    {
        public override event EventHandler<BackupCompressionResult> CompressionEnd;

        [JsonConstructor]
        public SimpleBackupM()
        {
            FilesSelectorM = new FilesSelectorM(typeof(StringMatchPathFilterM));
        }

        public override BackupCompressionResult StartStopCompression(Guid guid = default)
        {


            switch (this.State)
            {
                case BackupStates.Idle:
                    return StartCompression();

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
                        }
                    }
                    return null;

                case BackupStates.WaitToStop:
                    MsgBx.Show("", "Backup is already in wait to stop", MsgBxPicture.Info);
                    return null;

                default:
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, $"Unexpected value for State:<{State}>"));
                    return null;
            }
        }

        private BackupCompressionResult StartCompression()
        {
            BackupCompressionResult args;
            bool filesAreChanged = false;

            try
            {
                string fullPath;

                this.State = BackupStates.FilesToBackupCalculation;
                FilesSelectorM.CalculateFileList();

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

                if (Zip.Comprimi(FilesSelectorM.FilesSelected.ToList(), fullPath, out this.thrCompression, progressione: this.Progress) == true)
                {
                    args = new BackupCompressionResult(compressionResult: true, filesAreChanged: filesAreChanged, filesToBackup: FilesSelectorM.FilesSelected.Count);
                }
                else
                {
                    args = new BackupCompressionResult(compressionResult: false, filesAreChanged: filesAreChanged, filesToBackup: FilesSelectorM.FilesSelected.Count);
                }
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

        protected override ValidationResult DerivedClassValiditation(string propName)
        {
            return new ValidationResult(true, "");
        }
    }
}
