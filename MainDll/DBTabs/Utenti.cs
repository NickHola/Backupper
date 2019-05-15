using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.DBTabs
{
    public class Utenti : Anags.Tabella
    {
        public Colonna nome, password;

        internal Utenti() : base(nomeUtente: "utente")  //Non istanziabile fuori dall'assembly
        {
            nome = new Colonna(this, "nome", new TipiColonna.NVarChar("100"));
            password = new Colonna(this, "password", new TipiColonna.NVarChar(TipiColonna.NVarChar.Formati.Password, "20"));
        }
    }
}
