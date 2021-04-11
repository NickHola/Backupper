using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Reflection;
using System;
using System.IO;
using System.Text;
using Main.FSes;
using Main.Logs;

//***ISTRUZIONI**************************************************************************************************
//<JsonProperty> per serializzare field e property con visibilità Private o Friend
//<NonSerialized> per non serializzare i field Public
//<JsonIgnore>: per non serializzare le property Public
//<JsonConstructor>: da usare nel caso in cui ci sia costruttore con parametri (i parametri si dovranno chiamare come i campi o proprietà della classe (sembra non essere case sensitive) e ***ATTENZIONE***: i parametri del costruttore non dovranno essere ByRef) 
//<JsonConverter(GetType(StringEnumConverter))>: serve per far in modo che la serializzazione di un enum sia convertita in testo e non in numero


namespace Main.Serializes
{
    public static class Serialize
    {
        //TypeNameHandling.Auto = quando serializzo, se nella definizione della classe c'è un campo di tipo object in questo modo si segna il tipo di oggetto all'atto della serializzazione
        public static JsonSerializerSettings jsonSett; //= New JsonSerializerSettings() With {.TypeNameHandling = TypeNameHandling.Auto} '.ContractResolver = New MyContractResolver(),

        //Public Sub Initialize()
        //    jsonSett = New JsonSerializerSettings() With {.TypeNameHandling = TypeNameHandling.Auto}
        //End Sub

        public static bool SerializeInText(object obj, ref string text, SerializerType serialType = SerializerType.ntsJson, bool visualErr = true)
        {
            MemoryStream flussoInRam = new MemoryStream();
            string prefissoErrLog, errUte;
            errUte = "";

            prefissoErrLog = "tipoSerial:<" + serialType.ToString() + ">, tipo oggetto:<" + obj.GetType().Name + ">";

            if (visualErr == true) errUte = Log.main.errUserText;

            try
            {
                switch (serialType)
                {
                    case SerializerType.ntsJson:
                        text = JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSett);
                        break;
                    case SerializerType.xml:
                        var xmlSerial = new XmlSerializer(obj.GetType());
                        xmlSerial.Serialize(flussoInRam, obj);
                        flussoInRam.Position = 0;
                        text = new StreamReader(flussoInRam).ReadToEnd();
                        break;
                    case SerializerType.json:
                        var jsonSerial = new DataContractJsonSerializer(obj.GetType());
                        jsonSerial.WriteObject(flussoInRam, obj);
                        flussoInRam.Position = 0;
                        text = new StreamReader(flussoInRam).ReadToEnd();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, errUte, "Eccezione " + prefissoErrLog + "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return false;
            }
            return true;
        }

        public static bool SerializeInFile(object obj, string fileFullPath, SerializerType serialType = SerializerType.ntsJson, bool visualErr = true)
        {
            string testo, errUte;
            testo = "";
            errUte = "";

            if (visualErr == true) errUte = Log.main.errUserText;

            if (FS.ValidaPercorsoFile(fileFullPath, true, out fileFullPath, verEsistenza: CheckExistenceOf.PathFolderOnly) == false) return false;

            if (SerializeInText(obj, ref testo, serialType) == false) return false;

            try
            {
                File.WriteAllText(fileFullPath, testo);
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, errUte, "Eccezione in sezione file, ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return false;
            }

            return true;
        }

        public static bool DeserializeFromText<T>(string textSerialized, ref T obj, SerializerType serialType = SerializerType.ntsJson, bool visualErr = true)
        {
            MemoryStream flussoInRam;
            string prefissoErrLog, errUte;
            errUte = "";

            if (visualErr == true) errUte = Log.main.errUserText;

            if (obj == null)
            {
                Log.main.Add(new Mess(LogType.ERR, errUte, "Ricevuto oggetto a nothing, impossibile determinarne il tipo", visualMsgBox: false));
                return false;
            }

            prefissoErrLog = "tipoSerial:<" + serialType.ToString() + ">, tipo oggetto:<" + obj.GetType().Name + ">";

            try
            {
                switch (serialType)
                {
                    case SerializerType.ntsJson:  //ATTENZIONE quando si deserializza si crea un oggetto e quindi prima usa il costruttore che ha l'attributo JsonConstructor e poi valorizza le proprietà, questo comporta il doppio...
                                        //passaggio nelle property del'oggetto, va comunque usato il costruttore "principale/standard" poichè se manca una property dalla config sul DB viene cmq valorizzata prima dal costruttore
                        obj = (T)JsonConvert.DeserializeObject(textSerialized, obj.GetType(), jsonSett);
                        break;
                    case SerializerType.xml:
                        flussoInRam = new MemoryStream(Encoding.UTF8.GetBytes(textSerialized));
                        flussoInRam.Position = 0;
                        XmlSerializer xmlDeserial = new XmlSerializer(obj.GetType());
                        obj = (T)xmlDeserial.Deserialize(flussoInRam);
                        break;
                    case SerializerType.json:
                        flussoInRam = new MemoryStream(Encoding.UTF8.GetBytes(textSerialized));
                        flussoInRam.Position = 0;
                        DataContractJsonSerializer jsonDeserial = new DataContractJsonSerializer(obj.GetType());
                        obj = (T)jsonDeserial.ReadObject(flussoInRam);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.main.Add(new Mess(LogType.ERR, errUte, "Eccezione " + prefissoErrLog + "ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return false;
            }
            return true;
        }

        public static bool DeserializeFromFile(string fileFullPath, ref object obj , SerializerType serialType = SerializerType.ntsJson, bool visualErr = true) {
            string testo, errUte;
            errUte = "";

            if (visualErr == true) errUte = Log.main.errUserText;

            if (FS.ValidaPercorsoFile(fileFullPath, true, out fileFullPath, verEsistenza: CheckExistenceOf.FolderAndFile) == false) return false;

            try {
                testo = File.ReadAllText(fileFullPath);
            } catch (Exception ex) {
                Log.main.Add(new Mess(LogType.ERR, errUte, "Eccezione in sezione file, ex.mess:<" + ex.Message + ">", visualMsgBox: false));
                return false;
            }

            if (DeserializeFromText(testo,ref obj, serialType) == false) return false;
            return true;
        }

        //Per serializzare un field o proprietà di una classe con visibilita private usare l'attributo<JsonProperty>
        //Public Class MyContractResolver 'Di default la libreria Newtonsoft per i Json non serializza campi e proprietà con visibilità private, MyContractResolver stabilisce cosa va serializzato
        //    Inherits Serialization.DefaultContractResolver
        //    Protected Overrides Function CreateProperties(type As Type, memberSerialization As MemberSerialization) As IList(Of Serialization.JsonProperty)
        //        Dim props = type.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance).Select(Function(p) MyBase.CreateProperty(p, memberSerialization)).Union(
        //            type.GetFields(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance).Select(Function(f) MyBase.CreateProperty(f, memberSerialization))).ToList()

        //        props.ForEach(Sub(p)
        //                          p.Writable = True
        //                          p.Readable = True
        //                      End Sub)
        //        Return props
        //    End Function
        //End Class

    }
}
