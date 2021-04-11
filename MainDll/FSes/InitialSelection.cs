using Main.Binds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace Main.FSes
{
    [JsonConverter(typeof(StringEnumConverter))]
    [TypeConverter(typeof(EnumToString))]
    public enum InitialSelection
    {
        AllFiles,
        NoFile
    }
}
