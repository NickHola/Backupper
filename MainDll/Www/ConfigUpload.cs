using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Www
{
    [Serializable] public class ConfigUpload : ConfigBase
    {
        internal string suffFileCorrotto;

        [JsonConstructor] internal ConfigUpload(string savableName) : base(savableName) { //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
            suffFileCorrotto = "_corrotto";
        }
    }
}
