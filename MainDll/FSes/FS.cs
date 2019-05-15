using System;
using System.IO;
using System.Text.RegularExpressions;
using Main.Logs;

namespace Main.FSes
{
    public static class FS
    {
        /// <summary>
        /// </summary>
        /// <param name="percorsoFile">ATTENZIONE: in caso di percorso relativo(Str.relativo) o di percorso multi drive(senza lettera del drive) se formattaPerc=true il parametro potrà cambiare di valore </param>
        /// <returns></returns>
        public static bool ValidaPercorsoFile(string percorsoFile, bool contieneNomeFile, out string percFileFormattato, CheckExistenceOf verEsistenza = CheckExistenceOf.Nothing,
                                           Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(Tipi.ERR, Log.main.errUserText);

            percFileFormattato = percorsoFile;

            if (percorsoFile == null)
            {
                logMess.testoDaLoggare = "percorsoFile è a null";
                Log.main.Add(logMess);
                return false;
            }

            string tmpStr, nomePercorso, prefissoErrLog; bool percMultiDrive;
            tmpStr = "";

            prefissoErrLog = "Il percorsoFile:<" + percorsoFile + "> non è valido, ";
            percMultiDrive = false;

            if (contieneNomeFile == false)
            {
                if (percorsoFile == Str.relativo) App.GetAppFullPath(out _, path: out percorsoFile, name: out _);
                if (percorsoFile.Right(1) != @"\") percorsoFile += @"\";
                //If formattaPerc = True Then If Right(paramPercFile, 1) <> "\" Then paramPercFile += "\"
            }


            if (Regex.IsMatch(percorsoFile, "^\\.+$") == true)
            { //(es. \pippo\pluto) percorso multi drive (non contine la lettera del drive bisogna trovarlo)
                percMultiDrive = true;

                if (verEsistenza == CheckExistenceOf.PathFolderOnly || verEsistenza == CheckExistenceOf.FolderAndFile)
                {
                    bool esito = false;
                    for (int i = 97; i <= 122; i++)
                    { //lettere nell'ascii
                        if (contieneNomeFile == true && verEsistenza == CheckExistenceOf.FolderAndFile)
                        {
                            tmpStr = (char)i + ":" + percorsoFile;
                            if (File.Exists(tmpStr) == true)
                            {
                                esito = true;
                                break;
                            }
                        }
                        else
                        {
                            tmpStr = (char)i + ":" + Path.GetDirectoryName(percorsoFile); //anche se contieneNomeFile=True, mi prendo la cartella con GetDirectoryName
                            if (Directory.Exists(tmpStr) == true)
                            {
                                esito = true;
                                break;
                            }
                        }
                    }

                    if (esito == false) return false;
                    percorsoFile = tmpStr;

                }
            }

            if (percMultiDrive == false && Regex.IsMatch(percorsoFile, @"^[a-zA-Z]{1}:\\.+$") == false)
            {
                logMess.testoDaLoggare = prefissoErrLog + @"non inizia con X:\...";
                Log.main.Add(logMess);
                return false;
            }

            tmpStr = percMultiDrive == false ? percorsoFile.Substring(3) : percorsoFile; //Se il percorso non è multi driver, tolgo i primi 3 caratteri per controllare la presenza dei : nel resto della stringa
            if (Regex.IsMatch(tmpStr, @"^[^\/:*?""<>]+$") == false)
            {
                logMess.testoDaLoggare = prefissoErrLog + @"contiene uno dei seguenti caratteri non ammessi / : * ? "" < >";
                Log.main.Add(logMess);
                return false;
            }

            if (Regex.IsMatch(percorsoFile, @"\\\\") == true)
            {
                logMess.testoDaLoggare = prefissoErrLog + "contiene 2 o più barre inverse contigue";
                Log.main.Add(logMess);
                return false;
            }

            try
            {
                nomePercorso = Path.GetDirectoryName(percorsoFile);
            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = prefissoErrLog + "ex.mess:<" + ex.Message + ">";
                Log.main.Add(logMess);
                return false;
            }

            if (contieneNomeFile == true && percorsoFile.Right(1) == @"\")
            {
                logMess.testoDaLoggare = prefissoErrLog + "non contiene il nome del file";
                Log.main.Add(logMess);
                return false;
            }

            if (percMultiDrive == false && Path.IsPathRooted(nomePercorso) == false)
            {
                logMess.testoDaLoggare = prefissoErrLog + "IsPathRooted = false";
                Log.main.Add(logMess);
                return false;
            }

            if (verEsistenza == CheckExistenceOf.PathFolderOnly && Directory.Exists(nomePercorso) == false)
            {
                logMess.testoDaLoggare = prefissoErrLog + "il percorso non esiste";
                Log.main.Add(logMess);
                return false;
            }
            else if (verEsistenza == CheckExistenceOf.FolderAndFile && File.Exists(percorsoFile) == false)
            {
                logMess.testoDaLoggare = prefissoErrLog + "il file non esiste";
                Log.main.Add(logMess);
                return false;
            }

            percFileFormattato = percorsoFile;
            return true;
        }


        public static bool ValidaNomeFile(string file, bool contienePercorso, CheckExistenceOf verificaEsistenza = CheckExistenceOf.Nothing, Mess logMess = null) {

            if (file == null) return false;
            if (logMess == null) logMess = new Mess(Tipi.ERR, Log.main.errUserText);

            string prefissoErrLog, percorsoFile, nomeFile;
            percorsoFile = "";
            prefissoErrLog = "Il nomeFile:<" + file + "> non è valido, ";

            //If file = Str.relativo Then App.DammiPercorsoENomeExe(nome:=file)

            if (contienePercorso == true) {
                nomeFile = Path.GetFileName(file);
                percorsoFile = Path.GetFullPath(file);
            } else { 
                nomeFile = file;
            }

            if (Regex.IsMatch(nomeFile, @"^[^\/:*?""<>]+$") == false) {
                logMess.testoDaLoggare = prefissoErrLog + @"contiene uno dei seguenti caratteri non ammessi / : * ? "" < >";
                Log.main.Add(logMess);
                return false;
            }

            switch (verificaEsistenza) {
                case CheckExistenceOf.Nothing:
                    break;
                case CheckExistenceOf.FolderAndFile:
                    if (contienePercorso) {
                        if (ValidaPercorsoFile(percorsoFile, false, out _, verEsistenza: CheckExistenceOf.PathFolderOnly) == false) return false;

                        if (File.Exists(file) == false) {
                            logMess.testoDaLoggare = prefissoErrLog + "il file non esiste";
                            Log.main.Add(logMess);
                            return false;
                        }
                    } else {
                        logMess.testoDaLoggare = "ricevuto verificaEsistenza:<percorsoEFile> ma il parametro contienePercorso:<false>, errore nel passaggio parametri";
                        Log.main.Add(logMess);
                    }
                    break;
                default:
                    logMess.testoDaLoggare = "ricevuto valore disatteso per il parametro verificaEsistenza:<" + verificaEsistenza + ">";
                    logMess.tipo = Tipi.Warn;
                    Log.main.Add(logMess);     //Non ritorno false, poichè non è grave;
                    break;
            }

            return true;
        }
    }

   


}
