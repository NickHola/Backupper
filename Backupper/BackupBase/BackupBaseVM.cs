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

        ViewMode viewMode;

        public BackupBaseM Model { get; set; }

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

        public ViewMode ViewMode
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
            Model = backupBase;
        }

        public void SetDestinationFolder()
        {
            Model.SetDestinationFolder();
        }

        public void StartStopCompression()
        {
            Thread thrStartStopCompression = new Thread(() => Model.StartStopCompression());
            thrStartStopCompression.SetApartmentState(ApartmentState.STA);
            thrStartStopCompression.IsBackground = true;
            thrStartStopCompression.Start();
        }

        public void SrcBackupBaseSwitchViewMode()
        {
            if (ViewMode == ViewMode.Compact)
                ViewMode = ViewMode.Full;

            else if (ViewMode == ViewMode.Full)
                ViewMode = ViewMode.Compact;
        }

        abstract public void ShowFilesSelector();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
