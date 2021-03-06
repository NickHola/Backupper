﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Main.Binds;

namespace Main
{
    [JsonConverter(typeof(StringEnumConverter))]
    [TypeConverter(typeof(EnumToString))]
    [Serializable]
    public enum SelectionBehavior
    {
        Inclusion,
        Exclusion
    }

}
