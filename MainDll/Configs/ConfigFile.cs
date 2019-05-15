using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Main.Cryptoes;
using Main.FSes;
using Main.Serializes;
using Main.Logs;
using System.Text;
using Main.Salvable;
using static Main.Validations.Validation;

namespace Main.Configs
{
    [Serializable]
    public abstract class ConfigFile
    {
        string fullFilePath;
        [JsonProperty] protected List<ConfigurazioneSuFile> configurations; //As Dictionary(Of String, Object) 'configGeneriche: usato se l'app non dispone di un DB

        [JsonIgnore] public string FullFilePath
        {
            get { return fullFilePath; }
            protected set {
                CtrlValue(value);
                fullFilePath = value;
            }
        }

        protected ConfigFile() { }

        public ConfigFile(string fullFilePath)
        {
            this.fullFilePath = fullFilePath;
            configurations = new List<ConfigurazioneSuFile>(); //Lasciare il value di tipo object altrimenti il serializzatore dice che non riesce ad instanziare Configs poichè MustInherits
        }

        public bool SaveOnFile(bool encrypt = true, byte cicli = 9)
        {
            string text; byte[] streamToSave;
            text = "";
            streamToSave = null;

            if (Serialize.SerializeInText(this, ref text) == false) return false;

            if (encrypt == true)
            { if (Crypto.Cripta(text, ref streamToSave, cicli) == false) return false; }
            else
            { streamToSave = Encoding.Unicode.GetBytes(text); }

            if (FS.ValidaPercorsoFile(fullFilePath, true, out _, verEsistenza: CheckExistenceOf.PathFolderOnly) == false) return false;

            try
            {
                File.WriteAllBytes(fullFilePath, streamToSave);
                //IO.File.WriteAllText(percorsoENomeFile, testo)
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "Eccezione in WriteAllBytes, ex.mess:<" + ex.Message + ">"));
                return false;
            }

            return true;
        }

        //Friend Function DammiConfig(ByRef config As Configs.Base, distinguiUtente As Boolean, descErr As String, Optional tipoLogErr As Tipi = Tipi._Nothing) As Boolean
        internal bool GetConfig(ref ISavable config, bool markUser, Mess logMess)
        {

            IEnumerable<ConfigurazioneSuFile> configFiltrate;

            string name, parent;
            Type type;
            name = config.SavableName;
            parent = config.SavableParentName;
            type = config.GetType();

            configFiltrate = from tmp in configurations where tmp.config.SavableName == name && tmp.config.SavableParentName == parent && tmp.type == type select tmp;

            if (markUser == true) configFiltrate = from tmp in configFiltrate where tmp.userId == App.CurrentUserId select tmp;

            if (configFiltrate.Count() == 0)
            {
                logMess.testoDaLoggare = "nel file di configurazione:<" + fullFilePath + ">, non ho trovato impostazioni con " + logMess.testoDaLoggare;
                Log.main.Add(logMess);
                return false;
            }
            else if (configFiltrate.Count() > 1)
            {
                logMess.testoDaLoggare = "nel file di configurazione:<" + fullFilePath + ">, ho trovato più di una impostazione con " + logMess.testoDaLoggare + ", sarà preso il primo valore";
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //In questo caso deve essere sempre errore per questo ho utilizzato un nuovo mess
            }

            config = configFiltrate.ElementAt(0).config;
            return true;
        }

        internal bool SaveConfig(ISavable config, bool markUser, string descErr)
        {

            IEnumerable<ConfigurazioneSuFile> selectedConfig; UInt64 userId;

            //Check if config already exists
            selectedConfig = from tmp in configurations where tmp.config.SavableName == config.SavableName && tmp.config.SavableParentName == config.SavableParentName && tmp.type == config.GetType() select tmp;

            userId = markUser == true ? App.CurrentUserId : 0;

            selectedConfig = from tmp in selectedConfig where tmp.userId == userId select tmp;

            if (selectedConfig.Count() == 0)
            { //Creazione di una nuova
                ConfigurazioneSuFile configurazione = new ConfigurazioneSuFile();
                configurazione.type = config.GetType();
                configurazione.userId = userId;
                configurazione.config = config;
                configurations.Add(configurazione);

            }
            else if (selectedConfig.Count() == 1)
            { //Aggiornamento della config esistente
                selectedConfig.ElementAt(0).config = config;
            }
            else
            {
                Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, "nel file di configurazione:<" + fullFilePath + ">, ho trovato più di una impostazione con " + descErr + ", l'impostazione non sarà salvata"));
                return false;
            }
            return true;
        }
    }
}
