using Main.Schedulers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{

    public class AdvancedBackupVM : BackupBaseVM
    {
        //protected new static string backupTypeName;

        public static string BackupTypeName { get; private set; } = "Advanced backup";
        public static int BackupTypeOrder { get; private set; } = 1;


        public AdvancedBackupVM(BackupBaseM backupBase) : base(backupBase)
        { }

        //public AdvancedBackupVM(string backupName = null) : base(typeof(AdvancedBackupM), backupName)
        //{ }

        override public void ShowFilesSelector()
        {
            BackupsVM.Instance.ShowOrHideSelectedSetting(((AdvancedBackupM)Model).FilesSelectorM);
        }

        public void ShowScheduler()
        {
            BackupsVM.Instance.ShowOrHideSelectedSetting(((AdvancedBackupM)Model).SchedulerM);
        }
    }
}
