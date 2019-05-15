using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.DBTabs
{
    public class UtentiGruppi : Anags.Tabella
    {
        public Colonna fk_utenti, fk_gruppiUtente;

        internal UtentiGruppi() : base(nomeUtente: "gruppi dell'utente") //Non istanziabile fuori dall'assembly
        {
            fk_utenti = new Colonna(this, "fk_utenti", new TipiColonna.BigInt(), tabFk: new List<Type> { typeof(DBTabs.Utenti) }, univoca: true, nomeUtente: "utente");
            fk_gruppiUtente = new Colonna(this, "fk_gruppiUtente", new TipiColonna.BigInt(), tabFk: new List<Type> { typeof(DBTabs.GruppiUtente) }, univoca: true, nomeUtente: "gruppo");
        }
    }
}
