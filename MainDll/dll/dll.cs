using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Main.Logs;
using Main.Zips;

namespace Main
{
    public static class dll
    {
        public static string percorso;

        private static Dictionary<string, Assembly> listaDllInRam = new Dictionary<string, Assembly>();

        internal static bool inizializza()
        {
            if (App.GetAppFullPath(out _, path: out dll.percorso, name: out _) == false) return false;

            dll.percorso += @"dll\";

            if (Directory.Exists(dll.percorso) == false) Directory.CreateDirectory(dll.percorso);

            CaricaDllEmbeddedInRam("Main.dll.MahApps.Metro.dll", "MahApps.Metro.dll");
            CaricaDllEmbeddedInRam("Main.dll.Newtonsoft.Json.dll", "Newtonsoft.Json.dll");
            //CaricaDllEmbeddedInRam("Main.SevenZipSharp.dll", "SevenZipSharp.dll") 'Se si carica SevenZipSharp dalla ram non so perchè ma va in erore, bisogna caricarlo da file

            AppDomain.CurrentDomain.AssemblyResolve += CaricaDllAssenti;

            if (Environment.Is64BitProcess == false)
                Zip.nomeDll7Zip = "7z.dll";
            else
                Zip.nomeDll7Zip = "7z-x64.dll";

            if (ScriviDllEmbeddedSuDisco(Zip.nomeDll7Zip, dll.percorso) == false) return false;

            return true;


            //******OLD*****************************************************************************
            //AppDomain.CurrentDomain.AppendPrivatePath(dll.percorso) '***FUNZIONA: serve per far caricare una dll da un percorso diverso ripetto a quello dell'eseguibile
            //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") & ";" & dll.percorso) ' * **NON FUNZIONA

            //*****Alternativa a AppendPrivatePath poichè dprecato, da far funzionare in futuro
            //Dim setupInfo As AppDomainSetup = New AppDomainSetup()
            //setupInfo.ApplicationName = "PluginsDomain"
            //setupInfo.ApplicationBase = percorsoo
            //setupInfo.ConfigurationFile = "TestMainDll.vshost.exe.config"
            //setupInfo.PrivateBinPath = "dll"
            //Dim newDomain As AppDomain = AppDomain.CreateDomain("My New AppDomain", Nothing, setupInfo)
            //AppDomain.Load()
            //AppDomain.Unload(newDomain)
            //*************************************************************************************
        }

        private static Assembly CaricaDllAssenti(object sender, ResolveEventArgs args)
        { //ATTENZIONE il caricamento delle dll direttamente in RAM con alcune dll può dare problemi (esempio: SevenZipBase.SetLibraryPath(dll.nomePercorso & nomeDll7Zip)) 
            string nomeDllDaCaricare;
            nomeDllDaCaricare = args.Name.Split(',')[0].Trim();
            //If Right(nomeDllDaCaricare, 9) = "resources" Then nomeDllDaCaricare = Left(nomeDllDaCaricare, -9)
            nomeDllDaCaricare += ".dll";

            if (nomeDllDaCaricare == "Main.dll") return null; //resources.dll è la dll contenente le risorse del progetto, è contenuta nell'eseguibile e non devo fare nulla, viene richiesta...
            //                                                                ...quando si usa l'istruzione "Main.My.Resources"
            if (nomeDllDaCaricare == "")
            {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "ricavato nomeDllDaCaricare vuoto da args.Name:<" + args.Name + ">"));
                App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
            }

            //Se il file lo avevo caricato in ram (varibile listaDllInRam) lo prendo da li.....
            if (listaDllInRam.ContainsKey(args.Name)) return listaDllInRam[args.Name];

            //.....altrimenti lo scrivo su disco e lo prendo dal disco
            if (File.Exists(dll.percorso + nomeDllDaCaricare) == false)
            {
                if (ScriviDllEmbeddedSuDisco(nomeDllDaCaricare, dll.percorso, terminaAppSeErr: false) == false) return null;
            }

            //****ATTENZIONE: non usare Assembly.LoadFrom perchè non carica correttamente le dll tipo SevenZipSharp: Also, note that if you use LoadFrom you'll likely get a FileNotFound exception because the Assembly resolver will attempt to find the assembly you're loading in the GAC or the current application's bin folder.Use LoadFile to load an arbitrary assembly file instead--but note that if you do this you'll need to load any dependencies yourself.
            return Assembly.LoadFile(dll.percorso + nomeDllDaCaricare);

            //IO.File.Delete(dll.nomePercorso & nomeDllDaCaricare)  'Le dll in uso non si possono cancellare
            //Return Reflection.Assembly.Load(dataArray)  'ATTENZIONE il caricamento delle dll direttamente in RAM con alcune dll può dare problemi(esempio: SevenZipBase.SetLibraryPath(dll.nomePercorso & nomeDll7Zip))            
        }

        private static bool ScriviDllEmbeddedSuDisco(string nomeDllDaScrivere, string percorso, bool terminaAppSeErr = true)
        {

            if (File.Exists(percorso + nomeDllDaScrivere) == true) return true;

            string errUte; Stream stream; byte[] dataArray;
            errUte = "";

            IEnumerable<String> nomeRisorsa = from tmp in Assembly.GetExecutingAssembly().GetManifestResourceNames() where tmp.Contains(nomeDllDaScrivere) select tmp;

            if (terminaAppSeErr == true) errUte = "L'applicazione verrà terminata consultare il log";

            if (nomeRisorsa.Count() == 0)
            {
                Log.main.Add(new Mess(Tipi.ERR, errUte, "nomeDllDaCaricare:<" + nomeDllDaScrivere + "> non trovato"));
                if (terminaAppSeErr == true) App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                return false;
            }
            else if (nomeRisorsa.Count() > 1)
            {
                Log.main.Add(new Mess(Tipi.ERR, errUte, "trovati " + nomeRisorsa.Count() + " di nomeDllDaCaricare:<" + nomeDllDaScrivere + ">"));
                if (terminaAppSeErr == true) App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
                return false;
            }

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(nomeRisorsa.ElementAt(0));
            dataArray = new byte[stream.Length];
            stream.Read(dataArray, 0, Convert.ToInt32(stream.Length));
            stream.Close();
            stream.Dispose();

            File.WriteAllBytes(percorso + nomeDllDaScrivere, dataArray);

            return true;
        }

        /// <summary>
        /// Load Assembly, DLL from Embedded Resources into memory.
        /// </summary>
        /// <param name="nomeRisorsaEmbedded">Embedded Resource string. Example: WindowsFormsApplication1.SomeTools.dll</param>
        /// <param name="nomeDll">File Name. Example: SomeTools.dll</param>
        public static bool CaricaDllEmbeddedInRam(string nomeRisorsaEmbedded, string nomeDll)
        {
            byte[] arrayByte; Assembly assembly;

            using (Stream flusso = Assembly.GetExecutingAssembly().GetManifestResourceStream(nomeRisorsaEmbedded))
            {
                if (flusso == null)
                {
                    Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, nomeRisorsaEmbedded + " is not found in Embedded Resources."));
                    return false;
                }
                arrayByte = new byte[flusso.Length];
                flusso.Read(arrayByte, 0, Convert.ToInt32(flusso.Length));
                try
                {
                    assembly = Assembly.Load(arrayByte);
                    listaDllInRam.Add(assembly.FullName, assembly);
                    return true;
                }
                catch  { } //Se fallisce eseguo il codice che segue
            }

            bool fileOk = false;
            string tempFile = "";

            using (System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider()) {
                string fileHash = BitConverter.ToString(sha1.ComputeHash(arrayByte)).Replace("-", string.Empty);
                tempFile = Path.GetTempPath() + nomeDll;
                if (File.Exists(tempFile)) {
                    byte[] bb = File.ReadAllBytes(tempFile);
                    string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", String.Empty);
                    if (fileHash == fileHash2)
                    { fileOk = true; }
                    else
                    { fileOk = false; }
                } else {
                    fileOk = false;
                }
            }
            if (fileOk == false) System.IO.File.WriteAllBytes(tempFile, arrayByte);

            assembly = Assembly.LoadFile(tempFile);
            listaDllInRam.Add(assembly.FullName, assembly);

            return true;
        }

        //Private Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly
        //    Return GetAssembly(args.Name)
        //End Function

        //Public Function GetAssembly(ByVal assemblyFullName As String) As Assembly
        //    If IsNothing(assemblies) = True Then Return Nothing
        //    If assemblies.ContainsKey(assemblyFullName) Then Return assemblies(assemblyFullName)
        //    Return Nothing
        //End Function
    }
}
