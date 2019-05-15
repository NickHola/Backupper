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
        public Colonna id, tipo, padre, nomePc, nome, valore, fk_utentiUte, ultData;
        
        internal ImpostApp() : base(nomeUtente: "impostazioni dell'aplicazione") //Non istanziabile fuori dall'assembly
        {
            id = new Colonna(this, "id", new TipiColonna.BigInt(), primKey: true, ident: true);
            tipo = new Colonna(this, "tipo", new TipiColonna.NVarChar("100"));
            padre = new Colonna(this, "padre", new TipiColonna.NVarChar("150"));
            nomePc = new Colonna(this, "nomePc", new TipiColonna.NVarChar("255"));
            nome = new Colonna(this, "nome", new TipiColonna.NVarChar("150"));
            valore = new Colonna(this, "valore", new TipiColonna.NVarChar("max"));  //Conterrà la serializzazione del suo configXXX in json 
            fk_utentiUte = new Colonna(this, "fk_utentiUte", new TipiColonna.BigInt(), tabFk: new List<Type>() { typeof(DBTabs.Utenti) }, nomeUtente: "utente");
            ultData = new Colonna(this, "ultData", new TipiColonna.DateTime(), nomeUtente: "ultima data di modifica");
        }

    }
}
