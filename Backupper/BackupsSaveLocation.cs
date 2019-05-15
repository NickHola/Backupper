using Main.Binds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    [JsonConverter(typeof(StringEnumConverter))]
    [TypeConverter(typeof(EnumToStringa))]
    [Serializable]
    public enum BackupsSaveLocation
    {
        File,
        RESTService,
        Db
    }
}
