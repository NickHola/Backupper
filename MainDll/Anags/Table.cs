using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.DBs;

namespace Main.Anags
{
    public abstract class Table : TabellaBase //ATTENZIONE per le foreignKey usare fk_[nomeTab][significatoCol]. Per i master detail, nella detail usare id_[nomeTabMaster]
    {
        //Dim utenti As New DBTabDef.Utenti //non si può istanziare DBTabDef.Utenti poichè anche lui è una tabella di tipo anagrafica e quindi andrebbe in loop
        public Column id, obs, fk_utentiUltUte, ultData, ultOperazione, keepAliveInMod;

        public Table(String nomeUtente = "", bool isView = false) : base(nomeUtente: nomeUtente, isView: isView)
        {
            id = new Column(this, "id", new ColumnTypes.BigInt(), primKey: true, ident: true);
            obs = new Column(this, "obs", new ColumnTypes.Bit());
            fk_utentiUltUte = new Column(this, "fk_utentiUltUte", new ColumnTypes.BigInt(), tabFk: new List<Type> { typeof(DBTabs.Utenti) });
            ultData = new Column(this, "ultData", new ColumnTypes.DateTime());  //Va bene anche in sostituzione di ModRnd
            ultOperazione = new Column(this, "ultOper", new ColumnTypes.NVarChar("10"));
            keepAliveInMod = new Column(this, "kaInMod", new ColumnTypes.NVarChar("31"));
        }
    }

    public struct TipoUltOper
    {
        public const string nuovo = "insert";
        public const string aggior = "update";
    }
}
