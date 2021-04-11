using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.DBTabs
{
    public class ImpostApp : TabellaBase
    {
        public Column id, tipo, padre, nomePc, nome, valore, fk_utentiUte, ultData;
        
        internal ImpostApp() : base(nomeUtente: "impostazioni dell'aplicazione") //Non istanziabile fuori dall'assembly
        {
            id = new Column(this, "id", new ColumnTypes.BigInt(), primKey: true, ident: true);
            tipo = new Column(this, "tipo", new ColumnTypes.NVarChar("100"));
            padre = new Column(this, "padre", new ColumnTypes.NVarChar("150"));
            nomePc = new Column(this, "nomePc", new ColumnTypes.NVarChar("255"));
            nome = new Column(this, "nome", new ColumnTypes.NVarChar("150"));
            valore = new Column(this, "valore", new ColumnTypes.NVarChar("max"));  //Conterrà la serializzazione del suo configXXX in json 
            fk_utentiUte = new Column(this, "fk_utentiUte", new ColumnTypes.BigInt(), tabFk: new List<Type>() { typeof(DBTabs.Utenti) }, nomeUtente: "utente");
            ultData = new Column(this, "ultData", new ColumnTypes.DateTime(), nomeUtente: "ultima data di modifica");
        }

    }
}
