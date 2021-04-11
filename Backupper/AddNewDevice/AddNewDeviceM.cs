using Main.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class AddNewDeviceM
    {
        public void AddNewBackupAtList(string backupTypeName)
        {
            BackupBaseM backupVM = null;

            if (backupTypeName == SimpleBackupVM.BackupTypeName)
            {
                backupVM = new SimpleBackupM();
            }
            else if (backupTypeName == AdvancedBackupVM.BackupTypeName)
            {
                backupVM = new AdvancedBackupM();
            }
            else
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, $"Received unexpected value for backupTypeName:<{backupTypeName}>"));
            }

            BackupsM.Instance.Add(backupVM);
        }
    }
}
