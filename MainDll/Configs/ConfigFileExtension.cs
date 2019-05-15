using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Main.Configs;
using Main.Cryptoes;
using Main.FSes;
using Main.Logs;
using Main.Serializes;

namespace Main
{
    public static class ConfigFileExtension  
    {


        public static ConfigFile LoadFromFile(this ConfigFile config, out bool inErr, Mess logMess = null)
        {
            bool encrypted;
            return config.LoadFromFile(out encrypted, out inErr, logMess);
        }

        public static ConfigFile LoadFromFile(this ConfigFile config, out bool encrypted, out bool inErr, Mess logMess = null)
        {
            string testo = "";
            return config.LoadFromFile(ref testo, out encrypted, out inErr, logMess);
        }

        public static ConfigFile LoadFromFile(this ConfigFile config, ref string testo, out bool encrypted, out bool inErr, Mess logMess = null)
        {
            bool fileEsiste = false;
            byte cicli = 9;
            return config.LoadFromFile(out fileEsiste, ref testo, cicli, out encrypted, out inErr, logMess);
        }

        public static ConfigFile LoadFromFile(this ConfigFile config, out bool fileEsiste, ref string testo, byte cicli, out bool encrypted, out bool inErr, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

            byte[] byteStream; string logErrPrefix;
            encrypted = false;
            inErr = true;
            string fullFilePath;
            Util.GetPropertyOrFieldValue(config, nameof(ConfigFile.FullFilePath), out fullFilePath);

            logErrPrefix = "Config.fullPath=" + fullFilePath + " - ";

            fileEsiste = false;

            if (FS.ValidaPercorsoFile(fullFilePath, true, out _, verEsistenza: CheckExistenceOf.FolderAndFile) == false) return config;

            fileEsiste = true;

            try
            {
                byteStream = File.ReadAllBytes(fullFilePath);
                //testo = IO.File.ReadAllText(config.percorsoENomeFile)
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(Tipi.ERR, "", logErrPrefix + "Eccezione in ReadAllBytes, ex.mess:<" + ex.Message + ">"));
                return config;
            }

            if (byteStream.Length == 0)
            {
                Log.main.Add(new Mess(Tipi.ERR, "", logErrPrefix + " the file is empty"));
                return config;
            }

            if (Crypto.Decripta(byteStream, ref testo, cicli, logMess: logMess) == false)
            {
                testo = Encoding.Unicode.GetString(byteStream);
                if (testo.Substring(0, 3) != "{\r\n")
                {
                    Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, logErrPrefix + "Lettura file fallita sia tramite Crypto.Decripta che file in chiaro, errore in Crypto.Decripta:<" + logMess.testoDaLoggare + ">"));
                    return config;
                }
            }
            else
            { encrypted = true; }

            if (Serialize.DeserializeFromText(testo, ref config) == false) return config;

            inErr = false;
            return config;
        }

        //public static bool LoadFromFile(this ConfigFile config)
        //{
        //    bool encrypted;
        //    return config.LoadFromFile(out encrypted);
        //}

        //public static bool LoadFromFile(this ConfigFile config, out bool encrypted)
        //{
        //    string testo = "";
        //    return config.LoadFromFile(ref testo, out encrypted);
        //}

        //public static bool LoadFromFile(this ConfigFile config, ref string testo, out bool encrypted)
        //{
        //    bool fileEsiste = false;
        //    byte cicli = 9;
        //    return config.LoadFromFile(ref fileEsiste, ref testo, ref cicli, out encrypted);
        //}

        //public static bool LoadFromFile(this ConfigFile config, ref bool fileEsiste, ref string testo, ref byte cicli, out bool encrypted)
        //{
        //    byte[] byteStream; string logErrPrefix;
        //    encrypted = false;
        //    string fullFilePath;
        //    Util.GetPropertyOrFieldValue(config, nameof(ConfigFile.FullFilePath), out fullFilePath);

        //    logErrPrefix = "Config.fullPath=" + fullFilePath + " - ";

        //    fileEsiste = false;

        //    if (FS.ValidaPercorsoFile(fullFilePath, true, out _, verEsistenza: CheckExistenceOf.FolderAndFile) == false) return false;

        //    fileEsiste = true;

        //    try
        //    {
        //        byteStream = File.ReadAllBytes(fullFilePath);
        //        //testo = IO.File.ReadAllText(config.percorsoENomeFile)
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.main.Add(new Mess(Tipi.ERR, "", logErrPrefix + "Eccezione in ReadAllBytes, ex.mess:<" + ex.Message + ">"));
        //        return false;
        //    }

        //    Mess logMess = new Mess(Tipi._Nothing, "");  //Avoid to log error message with Tipi._Nothing

        //    if (Crypto.Decripta(byteStream, ref testo, cicli, logMess: logMess) == false)
        //    {
        //        testo = Encoding.Unicode.GetString(byteStream);
        //        if (testo.Substring(0, 3) != "{\r\n")
        //        {
        //            Log.main.Add(new Mess(Tipi.Warn, Log.main.warnUserText, logErrPrefix + "Lettura file fallita sia tramite Crypto.Decripta che file in chiaro, errore in Crypto.Decripta:<" + logMess.testoDaLoggare + ">"));
        //            return false;
        //        }
        //    }
        //    else
        //    { encrypted = true; }

        //    if (Serialize.DeserializeFromText(testo, ref config) == false) return false;
        //    return true;
        //}
    }
}
