using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Logs
{
    [Serializable]
    public class Mess
    {
        public LogType tipo;
        public string testoDaVisual;
        public string testoDaLoggare;
        [NonSerialized]
        public DateTime oraCreazione;
        [NonSerialized]
        public readonly bool visualMsgBox;
        [NonSerialized]
        public bool visualiz, loggato;
        [NonSerialized]
        public byte nCicloInGestore;

        //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà

        [JsonConstructor]
        public Mess(LogType tipo, string testoDaVisual, string testoDaLoggare = "", DateTime oraCreazione = default(DateTime), bool visualMsgBox = true)
        {
            this.tipo = tipo;
            this.testoDaVisual = testoDaVisual;
            this.testoDaLoggare = Util.GetCallStack() + testoDaLoggare;
            this.oraCreazione = oraCreazione;  //Può essere anche DateTime.MinValue, lo valorizzo poi nel metodo Acc poichè il mess può essere creato anche da un metodo A che lo passa al metodo B e poi quest'ultimo lo visualizzerà
                                               //quindi l'effettiva data di creazione è quando lo si passa al metodo Acc.
            this.visualMsgBox = visualMsgBox;
            this.visualiz = false;
            this.loggato = false;
            this.nCicloInGestore = 0;
        }
    }
}
