using Main.Configs;
using Main.DBs;
using Main.Logs;
using Main.Serializes;
using Main.SQLes;
using Main.Salvable;
using System;
using System.Collections.Generic;


namespace Main //namespace Main.Salvable
{
    public static class ISavableExtension
    {
     
        public static bool Save(this ISavable obj, object destination, Mess logMess = null)
        {
            string testo, configType, errDesc;
            testo = "";

            configType = obj.GetType().Name.ParoleMinuMaiu(Str.MinMai.minu, true);

            errDesc = "name:<" + obj.SavableName + ">, parent:<" + obj.SavableParentName + ">, configType:<" + configType + ">";

            if (obj.SavableMarkUser == true)
            {
                if (App.CurrentUserId == 0)
                {
                    Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "for the config: " + errDesc + ", there is no logged in user"));
                    return false;
                }

                errDesc += ", user id:<" + App.CurrentUserId + ">";
            }

            if (Serialize.SerializeInText(obj, ref testo) == false) return false;



            //if (App.Db != null && (this.memorizzaIn == memorizzaIn.@default || this.memorizzaIn == memorizzaIn.dbPrincipaleApp)) {  //Salvo la config nella tabella impostApp del DB
            if (destination.GetType() == typeof(DBBase) || destination.GetType().IsSubclassOf(typeof(DBBase)))
            {
                DBBase Db = (DBBase)destination;
                SqlObj sql = new SqlObj(connStr: Db.ConnString);
                DBTabs.ImpostApp settingsTab = Db.ImpostApp;
                string where = settingsTab.nome.Nm + Sql.WhereFormat(obj.SavableName) + " AND " + settingsTab.padre.Nm + Sql.WhereFormat(obj.SavableParentName) + " AND " + settingsTab.tipo.Nm + Sql.WhereFormat(configType);

                if (obj.SavableMarkUser == true)
                {
                    where += " AND " + settingsTab.fk_utentiUte.Nm + Sql.WhereFormat(App.CurrentUserId);
                    settingsTab.fk_utentiUte.Value = App.CurrentUserId;
                }
                else
                {
                    settingsTab.fk_utentiUte.Value = 0;
                }

                if (obj.SavableMarkPcName == true)
                {
                    where += " AND " + settingsTab.nomePc.Nm + Sql.WhereFormat(Environment.MachineName);
                    settingsTab.nomePc.Value = Environment.MachineName;
                }
                else
                {
                    settingsTab.nomePc.Value = "";
                }

                if (sql.ExecQuery(Sql.sel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where) == false) return false;

                settingsTab.nome.Value = obj.SavableName;
                settingsTab.padre.Value = obj.SavableParentName;
                settingsTab.tipo.Value = configType;
                settingsTab.valore.Value = testo;
                settingsTab.ultData.Value = DateTime.Now;

                if (sql.ResDt.Rows.Count == 0)
                {
                    if (settingsTab.Insert(sql: ref sql) == false) return false;
                }
                else if (sql.ResDt.Rows.Count == 1)
                {
                    if (settingsTab.Update(where: where, sql: ref sql) == false) return false;
                }
                else
                {
                    Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "in the table:<" + settingsTab.Nm + ">, there are more than one setting with: " + errDesc + ", the setting will not be saved"));
                    return false;
                }

            }
            else if (destination.GetType().IsSubclassOf(typeof(ConfigFile)))
            { //Salvo la config nel file di configurazione dell'applicazione
                ConfigFile configFile = (ConfigFile)destination;
                if (configFile.SaveConfig(obj, obj.SavableMarkUser, errDesc) == false) return false;
            }
            else
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "unexpected value for DestinationType:<" + destination.GetType().ToString() + ">"));
                return false;
            }
            return true;
        }

        public static ISavable Load(this ISavable obj, object source, out bool inErr, Mess logMess = null)
        {
            inErr = true;
            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

            if (String.IsNullOrEmpty(obj.SavableName))
            {
                logMess.testoDaLoggare = "received null or void SavableName";
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
                return obj;
            }

            if (obj == null)
            {
                logMess.testoDaLoggare = "received null savable object";
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
                return obj;
            }

            string text, configType, errDesc;

            configType = obj.GetType().Name;

            errDesc = "name:<" + obj.SavableName + ">, parent:<" + obj.SavableParentName + ">, configType:<" + configType + ">";

            if (obj.SavableMarkUser == true)
            {
                if (App.CurrentUserId == 0)
                {
                    logMess.testoDaLoggare = "for the config: " + errDesc + ", there is no logged in user";
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
                    return obj;
                }
                errDesc += ", id utente:<" + App.CurrentUserId + ">";
            }

            if (source.GetType() == typeof(DBBase) || source.GetType().IsSubclassOf(typeof(DBBase)))
            { //Carico la config dalla tabella impostApp del DB
                DBBase Db = (DBBase)source;
                DBTabs.ImpostApp settingsTab = Db.ImpostApp;

                SqlObj sql = new SqlObj(connStr: Db.ConnString); string where; List<string> queries = new List<string>();

                where = settingsTab.nome.Nm + Sql.WhereFormat(obj.SavableName) + " AND " + settingsTab.padre.Nm + Sql.WhereFormat(obj.SavableParentName) + " AND " + settingsTab.tipo.Nm + Sql.WhereFormat(configType);

                if (obj.SavableMarkUser == true)
                    where += " AND " + settingsTab.fk_utentiUte.Nm + Sql.WhereFormat(App.CurrentUserId);
                else
                    where += " AND " + settingsTab.fk_utentiUte.Nm + Sql.WhereFormat(0);


                //Se distinguiNomePc allora verifico se cè la config per nomePc se non c'è allora eseguo la ricerca senza così eventualmente prendo la confg.dell'utente se
                if (obj.SavableMarkPcName == true) queries.Add(Sql.sel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where + " AND " + settingsTab.nomePc.Nm + Sql.WhereFormat(Environment.MachineName));

                queries.Add(Sql.sel + settingsTab.valore.Nm + " FROM " + settingsTab.Nm + " WHERE " + where);

                foreach (string query in queries)
                {
                    if (sql.ExecQuery(query) == false) return obj;
                    if (sql.ResDt.Rows.Count > 0) break;
                }

                if (sql.ResDt.Rows.Count == 0)
                {
                    //If tipoLogErr <> Tipi._Nothing Then log.Acc(New Mess(tipoLogErr, log.testoUteGen(tipoLogErr), "nella tabella:<" & .nmTab & ">, non ho trovato impostazioni con " & descErr))
                    logMess.testoDaLoggare = "in the table:" + settingsTab.Nm + ">, there are no settings with: " + errDesc;
                    Log.main.Add(logMess);
                    return obj;
                }
                else if (sql.ResDt.Rows.Count > 1)
                {
                    logMess.testoDaLoggare = "in the table:" + settingsTab.Nm + ">, there are more than one setting with: " + errDesc + ", sarà preso il primo valore";
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare));
                }

                text = (string)sql.ResDt.Rows[0][settingsTab.valore.Nm];

                if (Serialize.DeserializeFromText(text, ref obj) == false) return obj;

            }
            else if (source.GetType().IsSubclassOf(typeof(ConfigFile)))  //Carico la config dal file di configurazione dell'applicazione
            {
                ConfigFile configFile = (ConfigFile)source;
                if (configFile.GetConfig(ref obj, obj.SavableMarkUser, logMess) == false) return obj;
            }
            else
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "unexpected value for source type:<" + source.GetType().ToString() + ">"));
            }
            inErr = false; 
            return obj;
        }
    }
}
