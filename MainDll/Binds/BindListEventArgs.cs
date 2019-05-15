using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Binds
{
    public class BindListEventArgs : EventArgs
    {
        public bool interrompiOperazione;
        public Int32 indice;
        public object oggetto;

        public BindListEventArgs(Int32 indice = -1, object oggetto = null) {
            this.interrompiOperazione = false;
            this.indice = indice;
            this.oggetto = oggetto;
        }
    }
}
