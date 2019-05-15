using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{

    public class SimpleBackupVM : BackupBaseVM
    {
        public static string BackupTypeName { get; private set; } = "Simple backup";
        public static int BackupTypeOrder { get; private set; } = 0;

        public SimpleBackupVM(BackupBaseM backupBase) : base(backupBase)
        { }

        //public AdvancedBackupVM(string backupName = null) : base(typeof(AdvancedBackupM), backupName)
        //{ }

        override public void ShowFilesSelector()
        {
            BackupsVM.Instance.ShowOrHideSelectedSetting(((SimpleBackupM)Backup).FilesSelectorM);
        }
    }
}
