using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.DBTabs
{
    public class GruppiUtente : Anags.Tabella
    {
        public Colonna nome;
        internal GruppiUtente() : base(nomeUtente: "gruppi di utenti") //Non istanziabile fuori dall'assembly
        {
            nome = new Colonna(this, "nome", new TipiColonna.NVarChar("100"));
        }
    }
}
