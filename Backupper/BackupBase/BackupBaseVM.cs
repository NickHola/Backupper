using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Main.Validations.Validation;
using Main;

namespace Backupper
{
    public abstract class BackupBaseVM : INotifyPropertyChanged
    {
        bool isViewInEdit;

        ViewModes viewMode;

        public BackupBaseM Backup { get; set; }

        public bool IsViewInEdit
        {
            get { return isViewInEdit; }
            set
            {
                CtrlValue(value);
                isViewInEdit = value;
                OnPropertyChanged();
            }
        }

        public ViewModes ViewMode
        {
            get { return viewMode; }
            set
            {
                CtrlValue(value);
                viewMode = value;
                OnPropertyChanged();
            }
        }

        //public string BackupTypeName { get; set; }

        private BackupBaseVM()
        { }

        public BackupBaseVM(BackupBaseM backupBase) : this()
        {
            Backup = backupBase;
        }
        
        public void SetDestinationFolder()
        {
            Backup.SetDestinationFolder();
        }

        public void StartStopCompression()
        {
            Thread thrStartStopCompression = new Thread(() => Backup.StartStopCompression());
            thrStartStopCompression.SetApartmentState(ApartmentState.STA);
            thrStartStopCompression.IsBackground = true;
            thrStartStopCompression.Start();
        }
        
        public void SrcBackupBaseSwitchViewMode()
        {
            if (ViewMode == ViewModes.Compact)
                ViewMode = ViewModes.Full;

            else if (ViewMode == ViewModes.Full)
                ViewMode = ViewModes.Compact;
        }

        abstract public void ShowFilesSelector();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
