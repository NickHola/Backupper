using System;
using SevenZip;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using Main.FSes;
using Main.Logs;
using Main.Thrs;
using Main.Regexes;

namespace Main.Zips
{
    public static class Zip
    {
        public static string nomeDll7Zip;
        static bool inizializzato = false;
        static Dictionary<Int32, Progressione> progressioniAttuali;

        internal static bool inizializza(Mess logMess = null)
        {

            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

            SevenZipBase.SetLibraryPath(dll.percorso + nomeDll7Zip);
            progressioniAttuali = new Dictionary<int, Progressione>();
            inizializzato = true;

            return true;
        }

        public class OggDaComprimere
        {
            public string cartellaOrFile, percorsoInZip, regexEsclusiva, regexInclusiva;

            public OggDaComprimere(string cartellaOrFile = "", string regexEsclusiva = "", string regexInclusiva = "")
            {
                this.cartellaOrFile = cartellaOrFile;
                percorsoInZip = Str.relativo;
                this.regexEsclusiva = regexEsclusiva;
                this.regexInclusiva = regexInclusiva;
            }
        }

        #region "Compressione"

        public static bool Comprimi(object oggDaCompr, string percNomeFileCompr, TipiArchivio formatoArchivio = TipiArchivio.sevenZip, CompressionLevels livelloCompr = CompressionLevels.Ultra,
                         string password = null, int timeOutMs = -1, Progressione progressione = null, Mess logMess = null)
        {
            Thread thrZip = null;
            return Comprimi(oggDaCompr, percNomeFileCompr, out thrZip, formatoArchivio: formatoArchivio, livelloCompr: livelloCompr, password: password, timeOutMs: timeOutMs, progressione: progressione
                , logMess: logMess);
        }
        
        /// <param name="oggDaCompr">può essere: string (file o cartella), List(Of String) (file o cartella), oggDaComprimere (file o cartella), List(Of oggDaComprimere) (file o cartella)</param>  
        /// <param name="timeOutMs">0  ==  Non attende la compressione ma ritorna subito  |  -1 ==  Attesa infinita  |  XX ==  Attesa massima fino a XX ms</param>  
        public static bool Comprimi(object oggDaCompr, string percNomeFileCompr, out Thread thrZip, TipiArchivio formatoArchivio = TipiArchivio.sevenZip, CompressionLevels livelloCompr = CompressionLevels.Ultra,
                         string password = null, int timeOutMs = -1, Progressione progressione = null, Mess logMess = null)
        {
            thrZip = null;

            try
            {
                if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

                if ((Zip.inizializzato == false) && inizializza(logMess) == false) return false;

                Dictionary<string, string> dizionario; bool esito;

                if (FS.ValidaPercorsoFile(percNomeFileCompr, true, percFileFormattato: out percNomeFileCompr, verEsistenza: CheckExistenceOf.PathFolderOnly, logMess: logMess) == false) return false;

                if (PreparazioneDizionario(oggDaCompr, out dizionario, logMess) == false) return false;

                if (dizionario.Count == 0)
                {
                    Log.main.Add(new Mess(Tipi.info, "Non ci sono file da comprimere"));
                    return true;
                }

                esito = false;
                thrZip = Thr.AvviaNuovo(() => esito = ThrComprimi(dizionario, percNomeFileCompr, formatoArchivio, livelloCompr, password, progressione));

                if (timeOutMs != 0)
                { //Non attendo il completamento del thr se = 0 
                    if (Thr.AttesaCompletamento(ref thrZip, timeOutMs) == false || esito == false) return false;
                }

                if (thrZip.ThreadState == ThreadState.Aborted) return false;

                return true;
            }
            catch
            {
                return false;
            }

        }


        private static bool PreparazioneDizionario(object oggDaCompr, out Dictionary<string, string> dizionario, Mess logMess)
        {
            dizionario = new Dictionary<string, string>();

            if (oggDaCompr == null)
            {
                logMess.testoDaLoggare = "ricevuto oggDaCompr a nothing";
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui ci va sempre err
                return false;
            }

            if (oggDaCompr.GetType() == typeof(string))
            {
                if ((string)oggDaCompr == "")
                {
                    logMess.testoDaLoggare = "ricevuto oggDaCompr vuoto";
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui ci va sempre err
                    return false;
                }
                if (AnalisiOggDaCompr((string)oggDaCompr, ref dizionario, logMess) == false) return false;
            }
            else if (oggDaCompr.GetType() == typeof(List<string>))
            {
                if ((oggDaCompr as List<string>).Count == 0) {
                    logMess.testoDaLoggare = "ricevuto oggDaCompr come lista vuota";
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui ci va sempre err
                    return false;
                    }

                    foreach (string singoloOggDaCompr in (List<string>)oggDaCompr) {
                        if (AnalisiOggDaCompr(singoloOggDaCompr, ref dizionario, logMess) == false) continue;
                    }
            }
            else if (oggDaCompr.GetType() == typeof(OggDaComprimere))
            {
                OggDaComprimere tmpObj = (OggDaComprimere)oggDaCompr;
                if (AnalisiOggDaCompr(tmpObj.cartellaOrFile, ref dizionario, logMess, percorsoInZip: tmpObj.percorsoInZip, regexEsclusiva: tmpObj.regexEsclusiva, regexInclusiva: tmpObj.regexInclusiva) == false) return false;
            }
            else if (oggDaCompr.GetType() == typeof(List<OggDaComprimere>))
            {
                foreach (OggDaComprimere ogg in (List<OggDaComprimere>)oggDaCompr) {
                    if (AnalisiOggDaCompr(ogg.cartellaOrFile, ref dizionario, logMess, percorsoInZip: ogg.percorsoInZip, regexEsclusiva: ogg.regexEsclusiva, regexInclusiva: ogg.regexInclusiva) == false) continue;
                   }
            }

            return true;
        }

        private static bool AnalisiOggDaCompr(string oggDaCompr, ref Dictionary<string, string> dizionario, Mess logMess, string percorsoInZip = Str.relativo, string regexEsclusiva = null, string regexInclusiva = null)
        {
            string nomeFile;

            if (percorsoInZip == Str.relativo || percorsoInZip == "") percorsoInZip = "";

            if (Directory.Exists(oggDaCompr)) {
                PopolaDizionarioDaCartella(oggDaCompr, ref dizionario, percorsoInZip, regexEsclusiva, regexInclusiva, logMess);

            } else if (File.Exists(oggDaCompr)) {
                nomeFile = Path.GetFileName(oggDaCompr);
                dizionario.Add(percorsoInZip + nomeFile, oggDaCompr);

            } else {
                logMess.testoDaLoggare = "Il valore contenuto in oggDaCompr:<" + oggDaCompr + "> non rappresenta né un file né una cartella";
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }

        private static bool PopolaDizionarioDaCartella(string percorsoCartella, ref Dictionary<string, string> dizionario, string percorsoInZip, string regexEsclusiva, string regexInclusiva, Mess logMess) {

            DirectoryInfo cartellaInfo = new DirectoryInfo(percorsoCartella);

            FileInfo[] filesInfo = cartellaInfo.GetFiles("*.*", SearchOption.AllDirectories);

            bool valida, match;

            foreach (FileInfo fileInfo in filesInfo) {
                valida = true;

                if (regexEsclusiva != null && regexEsclusiva != "") {
                    if (Regex.RegexMatch(fileInfo.FullName, regexEsclusiva, out match) == false) continue;
                    valida = !match;

                } else if (regexInclusiva != null && regexInclusiva != "") {
                    if (Regex.RegexMatch(fileInfo.FullName, regexInclusiva, out match) == false) continue;
                    valida = match;
                }

                if (valida == false) continue;

                dizionario.Add(percorsoInZip + fileInfo.FullName.Replace(cartellaInfo.Parent.FullName + @"\", ""), fileInfo.FullName);
            }
            return true;
        }

        private static bool ThrComprimi(Dictionary<string, string> dizionarioDaCompr, string percNomeFileCompr, TipiArchivio formatoArchivio, CompressionLevels livelloCompr, string passsword, Progressione Progressione)
        {

            Thr.SbloccaThrPadre();
            SevenZipCompressor zipCompressor; 

            try {
                zipCompressor = new SevenZipCompressor();

                if (Progressione != null) progressioniAttuali.Add(zipCompressor.UniqueID, Progressione);

                zipCompressor.CompressionMode = CompressionMode.Create;
                zipCompressor.TempFolderPath = Path.GetTempPath();
                zipCompressor.ArchiveFormat = (OutArchiveFormat)(int)formatoArchivio;
                zipCompressor.CompressionMethod = CompressionMethod.Lzma; //ATTENZIONE: la libreria 16.04 con Lzma2 in alcuni casi va in errore
                if (passsword != null) {
                    zipCompressor.EncryptHeaders = true;
                    zipCompressor.ZipEncryptionMethod = ZipEncryptionMethod.Aes256;
                }

                //il formato 7zip se la dll viene eseguita a 32bit non accetta un livello di compressione superiore a Normal
                if (formatoArchivio == TipiArchivio.sevenZip && livelloCompr > CompressionLevels.Normal && Environment.Is64BitProcess == false) livelloCompr = CompressionLevels.Normal;
                zipCompressor.CompressionLevel = (SevenZip.CompressionLevel)(int)livelloCompr;

                zipCompressor.Compressing += PercentualeCompressa;
                zipCompressor.CompressionFinished += CompressioneTerminata;

                zipCompressor.CompressFileDictionary(dizionarioDaCompr, percNomeFileCompr, passsword);

                return true;
            } catch (Exception ex) {
                if (Progressione != null) Progressione.ScatenaEventoTerminataConErrori(ex.Message);
                //If progressioniAttuali.ContainsKey(zipCompressor.UniqueID) Then progressioniAttuali(zipCompressor.UniqueID).ScatenaEventoTerminataConErrori(ex.Message)
                return false;
            }
        }

        private static void PercentualeCompressa(object sender, ProgressEventArgs e) {
            SevenZipCompressor zipCompressor = (SevenZipCompressor)sender;

            //Application.Current.Dispatcher.Invoke(New Action(Sub() progressioniAttuali(zipCompressor.UniqueID).progressione = e.PercentDone))
            if (progressioniAttuali.ContainsKey(zipCompressor.UniqueID)) progressioniAttuali[zipCompressor.UniqueID].Percentage = e.PercentDone;

        }

        private static void CompressioneTerminata(object sender, EventArgs e) {
            SevenZipCompressor zipCompressor = (SevenZipCompressor)sender;

            if (progressioniAttuali.ContainsKey(zipCompressor.UniqueID)) {
                progressioniAttuali[zipCompressor.UniqueID].IsComplete = true;
                progressioniAttuali.Remove(zipCompressor.UniqueID);
            }
        }

        #endregion

        #region "DeCompressione"

        public static bool DeComprimi(string percNomeFileZip, string percorsoEstrazione, string password = null, int timeOutMs = -1, Progressione progressione = null) {
            Thread thrZip = null;
            return DeComprimi(percNomeFileZip, percorsoEstrazione, ref thrZip, password: password, timeOutMs: timeOutMs, progressione: progressione);
        }

        public static bool DeComprimi(string percNomeFileZip, string percorsoEstrazione, ref Thread thrZip, string password = null, int timeOutMs = -1, Progressione progressione = null)
        {
            if (Zip.inizializzato == false && inizializza() == false) return false;

            if (FS.ValidaNomeFile(percNomeFileZip, true, CheckExistenceOf.FolderAndFile) == false) return false;

            if (FS.ValidaPercorsoFile(percorsoEstrazione, true, percFileFormattato: out percorsoEstrazione, verEsistenza: CheckExistenceOf.PathFolderOnly) == false) return false;

            bool esito = false;
            Thread thread = Thr.AvviaNuovo(() => esito = ThrDeComprimi(percNomeFileZip, percorsoEstrazione, password, progressione));

            if (timeOutMs != 0) { //Non attendo il completamento del thr se = 0 
                if (Thr.AttesaCompletamento(ref thread, timeOutMs) == false || esito == false) return false;
            }

            return true;

        }

        private static bool ThrDeComprimi(string percNomeFileZip, string percorsoEstrazione, string passsword, Progressione progressione = null) {
            Thr.SbloccaThrPadre();

            SevenZipExtractor zipExtractor;
            try
            {

                if (passsword == null)
                    zipExtractor = new SevenZipExtractor(percNomeFileZip);
                else 
                    zipExtractor = new SevenZipExtractor(percNomeFileZip, passsword);


                if (progressione != null) progressioniAttuali.Add(zipExtractor.UniqueID, progressione);

                zipExtractor.Extracting += PercentualeEstratta;
                zipExtractor.ExtractionFinished += EstrazioneTerminata;

                zipExtractor.ExtractArchive(percorsoEstrazione);

                return true;

            } catch (Exception ex) {
                if (progressione != null) progressione.ScatenaEventoTerminataConErrori(ex.Message);
                //If progressioniAttuali.ContainsKey(zipExtractor.UniqueID) Then progressioniAttuali(zipExtractor.UniqueID).ScatenaEventoTerminataConErrori(ex.Message)
                return false;
            }
        }

        private static void PercentualeEstratta(object sender, ProgressEventArgs e) {
            SevenZipExtractor zipExtractor = (SevenZipExtractor)sender;

            //Application.Current.Dispatcher.Invoke(New Action(Sub() progressioniAttuali(zipCompressor.UniqueID).progressione = e.PercentDone))
            if (progressioniAttuali.ContainsKey(zipExtractor.UniqueID)) progressioniAttuali[zipExtractor.UniqueID].Percentage = e.PercentDone;

        }

        private static void EstrazioneTerminata(object sender, EventArgs e) {
            SevenZipExtractor zipExtractor = (SevenZipExtractor)sender;

            if (progressioniAttuali.ContainsKey(zipExtractor.UniqueID)) {
                progressioniAttuali[zipExtractor.UniqueID].IsComplete = true;
                progressioniAttuali.Remove(zipExtractor.UniqueID);
            }
        }

        #endregion


    }
}
