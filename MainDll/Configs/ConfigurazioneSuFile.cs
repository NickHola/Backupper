using System;
using Main.Salvable;

namespace Main.Configs
{
    [Serializable]
    public sealed class ConfigurazioneSuFile
    {
        public Type type;
        public UInt64 userId;
        public ISavable config; //ATTENZIONE qui nomePc non ci va poichè già che è salvato su file risiede su pc, NON cambiare ordine di dichiarazione
    }
}
