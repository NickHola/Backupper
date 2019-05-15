using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.Anags
{
    public abstract class Tabella : TabellaBase //ATTENZIONE per le foreignKey usare fk_[nomeTab][significatoCol]. Per i master detail, nella detail usare id_[nomeTabMaster]
    {
        //Dim utenti As New DBTabDef.Utenti //non si può istanziare DBTabDef.Utenti poichè anche lui è una tabella di tipo anagrafica e quindi andrebbe in loop
        public Colonna id, obs, fk_utentiUltUte, ultData, ultOperazione, keepAliveInMod;

        public Tabella(String nomeUtente = "", bool isView = false) : base(nomeUtente: nomeUtente, isView: isView)
        {
            id = new Colonna(this, "id", new TipiColonna.BigInt(), primKey: true, ident: true);
            obs = new Colonna(this, "obs", new TipiColonna.Bit());
            fk_utentiUltUte = new Colonna(this, "fk_utentiUltUte", new TipiColonna.BigInt(), tabFk: new List<Type> { typeof(DBTabs.Utenti) });
            ultData = new Colonna(this, "ultData", new TipiColonna.DateTime());  //Va bene anche in sostituzione di ModRnd
            ultOperazione = new Colonna(this, "ultOper", new TipiColonna.NVarChar("10"));
            keepAliveInMod = new Colonna(this, "kaInMod", new TipiColonna.NVarChar("31"));
        }
    }

    public struct TipoUltOper
    {
        public const string nuovo = "insert";
        public const string aggior = "update";
    }
}
