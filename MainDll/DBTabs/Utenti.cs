using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.DBTabs
{
    public class Utenti : Anags.Table
    {
        public Column nome, password;

        internal Utenti() : base(nomeUtente: "utente")  //Non istanziabile fuori dall'assembly
        {
            nome = new Column(this, "nome", new ColumnTypes.NVarChar("100"));
            password = new Column(this, "password", new ColumnTypes.NVarChar(ColumnTypes.NVarChar.Formati.Password, "20"));
        }
    }
}
