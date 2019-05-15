using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Main.SQLes;
using Main.Serializes;
using Main.Logs;
using Main.DBs;
using static Main.Validations.Validation;
using Main.Salvable;

namespace Main.Configs
{
    [Serializable]
    public abstract class ConfigBase : ISavable
    {
        string savableName, savableParentName;
        public bool SavableMarkUser { get; }
        public bool SavableMarkPcName { get; }

        //[JsonIgnore] public SaveLocation SavableSaveLocation { get; set; }   //<JsonProperty>
        //[JsonIgnore] public ISavable SavableParent { get; set; }

        public string SavableName
        {
            get { return savableName; }
            set
            {
                CtrlValue(value);
                savableName = value;
            }
        }
        public string SavableParentName
        {
            get { return savableParentName; }
            set
            {
                CtrlValue(value, ctrlVoid: false);
                savableParentName = value;
            }
        }


        //public ConfigBase(string nome, SaveLocation saveLocation = null, string padre = "", bool distinguiUtente = true, bool distinguiNomePc = true) 
        public ConfigBase(string savableName, string padre = "", bool distinguiUtente = true, bool distinguiNomePc = true)
        {
            this.SavableName = savableName;
            //this.SavableSaveLocation = saveLocation;
            this.SavableParentName = padre;
            this.SavableMarkUser = false;
            this.SavableMarkPcName = distinguiNomePc;
        }

        ////Moved in Savable.Load
        //public static bool Load(ConfigBase config, Mess logMess = null)
        //{ //Attenzione sono sicuro che nome non sia nothing e ne vuoto
        //    if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

        //    if (config == null)
        //    {
        //        logMess.testoDaLoggare = "ricevuto config a nothing";
        //        Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
        //        return false;
        //    }

        //    if (config.ConfigName == "")
        //    {
        //        logMess.testoDaLoggare = "ricevuto config con nome vuoto";
        //        Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
        //        return false;
        //    }

        //    if (config.ConfigSaveLocation == null)
        //    {
        //        logMess.testoDaLoggare = "ricevuto config con saveLocation a null";
        //        Log.main.Add(logMess);
        //        return false;
        //    }

        //    string testo, tipoConfig;

        //    tipoConfig = config.GetType().Name;

        //    logMess.testoDaLoggare = "nome:<" + config.ConfigName + ">, padre:<" + config.ConfigParentName + ">, tipoConfig:<" + tipoConfig + ">";

        //    if (config.DistinguiUtentes == true)
        //    {
        //        if (App.CurrentUserId == 0)
        //        {
        //            logMess.testoDaLoggare = "per la config con " + logMess.testoDaLoggare + " l'utente non risulta loggato"; //Don't use +=
        //            Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
        //            return false;
        //        }

        //        logMess.testoDaLoggare += ", id utente:<" + App.CurrentUserId + ">";
        //    }



        //    if (config.ConfigSaveLocation.DestinationType == typeof(DBBase))
        //    { //Carico la config dalla tabella impostApp del DB
        //        DBTabDef.ImpostApp settingsTab = config.ConfigSaveLocation.Db.ImpostApp;

        //        SqlObj sql = new SqlObj(connStr: config.ConfigSaveLocation.Db.ConnString); string where; List<string> queries = new List<string>();

        //        where = settingsTab.nome.Nm + Sql.WhereFormat(config.ConfigName) + " AND " + settingsTab.padre.Nm + Sql.WhereFormat(config.ConfigParentName) + " AND " + settingsTab.tipo.Nm + Sql.WhereFormat(tipoConfig);

        //        if (config.DistinguiUtentes == true)
        //            where += " AND " + settingsTab.fk_UtentiUte.Nm + Sql.WhereFormat(App.CurrentUserId);
        //        else
        //            where += " AND " + settingsTab.fk_UtentiUte.Nm + Sql.WhereFormat(0);


        //        //Se distinguiNomePc allora verifico se cè la config per nomePc se non c'è allora eseguo la ricerca senza così eventualmente prendo la confg.dell'utente se
        //        if (config.DistinguiNomePc == true) queries.Add(Sql.qrySel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where + " AND " + settingsTab.nomePc.Nm + Sql.WhereFormat(Environment.MachineName));

        //        queries.Add(Sql.qrySel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where);

        //        foreach (string query in queries)
        //        {
        //            if (sql.ExecQuery(query) == false) return false;
        //            if (sql.ResDt.Rows.Count > 0) break;
        //        }

        //        if (sql.ResDt.Rows.Count == 0)
        //        {
        //            //If tipoLogErr <> Tipi._Nothing Then log.Acc(New Mess(tipoLogErr, log.testoUteGen(tipoLogErr), "nella tabella:<" & .nmTab & ">, non ho trovato impostazioni con " & descErr))
        //            logMess.testoDaLoggare = "nella tabella:<" + settingsTab.Nm + ">, non ho trovato impostazioni con " + logMess.testoDaLoggare;
        //            Log.main.Add(logMess);
        //            return false;
        //        }
        //        else if (sql.ResDt.Rows.Count > 1)
        //        {
        //            logMess.testoDaLoggare = "nella tabella:<" + settingsTab.Nm + ">, ho trovato più di una impostazione con " + logMess.testoDaLoggare + ", sarà preso il primo valore";
        //            Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
        //        }

        //        testo = (string)sql.ResDt.Rows[0][settingsTab.valore.Nm];

        //        if (Serializ.DeserializzaDaTesto(testo, ref config) == false) return false;

        //    }
        //    else if (config.ConfigSaveLocation.DestinationType == typeof(FileDiConfig))  //Carico la config dal file di configurazione dell'applicazione
        //    {
        //        //scommentare riga sotto
        //        //if (config.ConfigSaveLocation.ConfigFile.DammiConfig(ref config, config.DistinguiUtentes, logMess) == false) return false;
        //    }
        //    //else if () //fare restApi
        //    //{
        //    //}
        //    else
        //    {
        //        //errore 
        //    }
        //    return true;
        //}

        //public bool Save(Mess logMess = null)
        //{
        //    string testo, tipoConfig, descErr;
        //    testo = "";

        //    if (this.ConfigSaveLocation == null)
        //    {
        //        logMess.testoDaLoggare = "saveLocation è a null";
        //        Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, logMess.testoDaLoggare));
        //        return false;
        //    }

        //    tipoConfig = Str.ParoleMinuMaiu(this.GetType().Name, Str.MinMai.minu, true);

        //    descErr = "nome:<" + this.ConfigName + ">, padre:<" + this.ConfigParentName + ">, tipoConfig:<" + tipoConfig + ">";

        //    if (this.DistinguiUtentes == true)
        //    {
        //        if (App.CurrentUserId == 0)
        //        {
        //            Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "per la config con " + descErr + " l'utente non risulta loggato"));
        //            return false;
        //        }

        //        descErr += ", id utente:<" + App.CurrentUserId + ">";
        //    }

        //    if (Serializ.SerializzaInTesto(this, ref testo) == false) return false;



        //    //if (App.Db != null && (this.memorizzaIn == memorizzaIn.@default || this.memorizzaIn == memorizzaIn.dbPrincipaleApp)) {  //Salvo la config nella tabella impostApp del DB
        //    if (this.ConfigSaveLocation.DestinationType == typeof(DBBase)) { 
        //        SqlObj sql = new SqlObj(connStr: ConfigSaveLocation.Db.ConnString); string where; DBTabDef.ImpostApp settingsTab = ConfigSaveLocation.Db.ImpostApp;
        //        where = settingsTab.nome.Nm + Sql.WhereFormat(this.ConfigName) + " AND " + settingsTab.padre.Nm + Sql.WhereFormat(this.ConfigParentName) + " AND " + settingsTab.tipo.Nm + Sql.WhereFormat(tipoConfig);

        //        if (DistinguiUtentes == true)
        //        {
        //            where += " AND " + settingsTab.fk_UtentiUte.Nm + Sql.WhereFormat(App.CurrentUserId);
        //            settingsTab.fk_UtentiUte.Value = App.CurrentUserId;
        //        }
        //        else
        //        {
        //            settingsTab.fk_UtentiUte.Value = 0;
        //        }

        //        if (DistinguiNomePc == true)
        //        {
        //            where += " AND " + settingsTab.nomePc.Nm + Sql.WhereFormat(Environment.MachineName);
        //            settingsTab.nomePc.Value = Environment.MachineName;
        //        }
        //        else
        //        {
        //            settingsTab.nomePc.Value = "";
        //        }

        //        if (sql.ExecQuery(Sql.qrySel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where) == false) return false;

        //        settingsTab.nome.Value = this.ConfigName;
        //        settingsTab.padre.Value = this.ConfigParentName;
        //        settingsTab.tipo.Value = tipoConfig;
        //        settingsTab.valore.Value = testo;
        //        settingsTab.ultDataMod.Value = DateTime.Now;

        //        if (sql.ResDt.Rows.Count == 0)
        //        {
        //            if (settingsTab.Insert(sql: ref sql) == false) return false;
        //        }
        //        else if (sql.ResDt.Rows.Count == 1)
        //        {
        //            if (settingsTab.Update(where: where, sql: ref sql) == false) return false;
        //        }
        //        else
        //        {
        //            Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "nella tabella:<" + settingsTab.Nm + ">, ho trovato più di una impostazione con " + descErr + ", l'impostazione non sarà salvara"));
        //            return false;
        //        }

        //    } else if (this.ConfigSaveLocation.DestinationType == typeof(FileDiConfig)) { //Salvo la config nel file di configurazione dell'applicazione
        //        if (ConfigSaveLocation.ConfigFile.SalvaConfig(this, this.DistinguiUtentes, descErr) == false) return false;
        //    }
        //    else
        //    {
        //        // errore 
        //    }
        //    return true;
        //}

    }
}
