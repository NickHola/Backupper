using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Www
{
    [Serializable] public class ConfigDownload : ConfigBase
    {
        [JsonConstructor] internal ConfigDownload(string savableName) : base(savableName) { //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
            MaxParallelStreams = 1;
            SleepAfterOneOperationMs = 3000;
        }
    }
}
