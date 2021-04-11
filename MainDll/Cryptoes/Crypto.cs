using System;
using System.IO;
using Main.FSes;
using Main.Logs;

namespace Main.Cryptoes
{
    public static class Crypto
    {
        public static bool Cripta(object testoOFlusso, ref byte[] flussoCript, byte numApplicazioni, byte gapCba = 255, string keyDes = "")
        {
            flussoCript = new byte[0];

            DES des;
            byte[] tmpCripto1, tmpCripto2;

            tmpCripto1 = null;
            tmpCripto2 = null;

            if (keyDes == "")
                des = new DES("", true);
            else
                des = new DES(keyDes, false);


            if (testoOFlusso.GetType() == typeof(string))
            {
                if (des.DaTestoADatiCript((string)testoOFlusso, out tmpCripto1) == false) return false;

            }
            else if (testoOFlusso.GetType() == typeof(byte[]))
            {
                des.CriptaDati((byte[])testoOFlusso, out tmpCripto1);

            }
            else
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto oggetto testoOFlusso di tipo sconosciuto, type:<" + testoOFlusso.GetType().Name + ">, baseType:<" + testoOFlusso.GetType().BaseType.Name + ">"));
                return false;
            }

            for (UInt16 i = 1; i <= numApplicazioni; i++)
            {
                des.CriptaDati(tmpCripto1, out tmpCripto2);
                tmpCripto1 = tmpCripto2;
            }

            flussoCript = tmpCripto1;
            return true;
        }

        public static bool Decripta<T>(byte[] flussoCript, ref T testoOFlusso, byte numApplicazioni, string keyDes = "", Mess logMess = null)
        {
            DES des;
            byte[] tmpCripto1, tmpCripto2;
            if (logMess == null) logMess = new Mess(LogType.ERR, "");

            des = keyDes == "" ? new DES("", true) : new DES(keyDes, false);

            tmpCripto2 = null;
            tmpCripto1 = flussoCript;

            for (UInt16 i = 1; i <= numApplicazioni; i++)
            {
                if (des.DecriptaDati(tmpCripto1, out tmpCripto2) == false) return false;
                tmpCripto1 = tmpCripto2;
            }

            if (testoOFlusso.GetType() == typeof(string)) //is Text
            {  
                string tmpStr = (string)Convert.ChangeType(testoOFlusso, typeof(string));
                if (des.DaDatiCriptATesto(tmpCripto1, out tmpStr) == false) return false;
                testoOFlusso = (T)Convert.ChangeType(tmpStr, typeof(T));

            }
            else if (testoOFlusso.GetType() == typeof(byte)) //is Stream
            { 
                byte[] tmpByteArr = (byte[])Convert.ChangeType(testoOFlusso, typeof(byte[]));
                if (des.DecriptaDati(tmpCripto1, out tmpByteArr) == false) return false;
                testoOFlusso = (T)Convert.ChangeType(tmpByteArr, typeof(T));
            }
            else
            {
                logMess.testoDaLoggare = "Ricevuto oggetto testoOFlusso di tipo sconosciuto, type:<" + testoOFlusso.GetType().Name + ">, baseType:<" + testoOFlusso.GetType().BaseType.Name + ">";
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }

        internal static bool ScriviFlussoInFile(byte[] flusso, string percorsoENomeFile, bool sovraScrivi)
        {

            if (sovraScrivi == false && File.Exists(percorsoENomeFile))
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Il percorsoENomeFile:<" + percorsoENomeFile + "> è già esistente"));
                return false;
            }

            if (FS.ValidaPercorsoFile(percorsoENomeFile, true, out percorsoENomeFile, verEsistenza: CheckExistenceOf.PathFolderOnly) == false) return false;

            try
            {
                FileStream file = new FileStream(percorsoENomeFile, FileMode.Create);
                file.Write(flusso, 0, flusso.Length);
                file.Close();
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Eccezione ex.mess:<" + ex.Message + ">"));
                return false;
            }

            return true;
        }
    }
}
