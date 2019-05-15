
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Binds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backupper
{
    [TypeConverter(typeof(EnumToStringa))]
    public enum BackupStates
    {
        Idle,
        FilesToBackupCalculation,
        MD5Calculation,
        Compressing,
        SavingInFTP,
        WaitToStop
    }
}
