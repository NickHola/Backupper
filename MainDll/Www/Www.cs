using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Www
{
 
    [Serializable]
    public enum DwlItemState
    {
        Iniziale,            //IMPORTANTE: Deve essere il primo dato che all'inizio deve valere di default Iniziale
        DwlAvviato,
        DwlCompletato,
        TimeoutToStart,
        Timeout,
        Eccezione
    }

    [Serializable]
    public enum UplItemState
    {
        Iniziale,            //IMPORTANTE: Deve essere il primo dato che all'inizio deve valere di default Iniziale
        UplAvviato,
        UplCompletato,
        TimeoutToStart,
        Timeout,
        Eccezione
    }

    [Serializable]
    public enum UploadType
    {
        Ftp
    }

}
