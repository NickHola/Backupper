using Main.Binds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace Main.FSes
{
    [JsonConverter(typeof(StringEnumConverter))]
    [TypeConverter(typeof(EnumToStringa))]
    [Serializable]
    public enum DirectoryDepth
    {
        TopDirectoryOnly,
        AllDirectories
    }
}
