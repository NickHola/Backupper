using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows;
using static Main.Util;
using Main.Concurs;
using Main.Logs;

//***********Questo è un esempio su come espandere sia il namespace Tg che il modulo TagMd nell'applicazione che importerà la dll Main***************
//Namespace Tg 
//    Partial Public Module TagMd
//        Public Function tgValoreGen2(Optional val As Object = Nothing) As KeyValueTuple(Of String, Object)
//            Return New KeyValueTuple(Of String, Object)("valoreGen", val)
//        End Function
//    End Module
//End Namespace

namespace Main
{

    public static partial class Tg
    {
        public static bool Scrivi(FrameworkElement ogg, KeyValueTuple<string, object> tag)
        {
            if (CtrlParametri(ogg, tag) == false) return false;
            if (ogg.Tag == null) ogg.Tag = new ConcurrentDictionary<string, object>();
            if (Concur.Dictionary_TryAddOrUpdate((ConcurrentDictionary<string, object>)ogg.Tag, tag) == false)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Non sono riuscito a scrivere la tag di nome:<" + tag.Key + "> sul controllo ogg.Name:<" + ogg.Name + ">"));
                return false;
            }
            return true;
        }
        public static bool Dammi(FrameworkElement ogg, KeyValueTuple<string, object> tag, ref object valore, bool errSeMancante = true)
        {
            if (CtrlParametri(ogg, tag) == false) return false;

            if (ogg.Tag == null)
            {
                if (errSeMancante == true) Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Nel controllo ogg.Name:<" + ogg.Name + "> manca la tag di nome:<" + tag.Key + ">, ogg.Tag è a nothing "));
                return false;
            }
            bool esiste;
            if (Concur.Dictionary_TryGet((ConcurrentDictionary<string, object>)ogg.Tag, tag.Key, ref valore, out esiste) == false)
            {
                if (esiste == true)
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Non sono riuscito a leggere la tag di nome:<" + tag.Key + "> sul controllo ogg.Name:<" + ogg.Name + ">"));
                else
                {
                    if (errSeMancante == true) Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Nel controllo ogg.Name:<" + ogg.Name + "> manca la tag di nome:<" + tag.Key + ">"));
                }
                return false;
            }
            return true;
        }
        public static bool Elimina(FrameworkElement ogg, KeyValueTuple<string, object> tag)
        {
            if (CtrlParametri(ogg, tag) == false) return false;

            if (Concur.Dictionary_TryRemove((ConcurrentDictionary<string, object>)ogg.Tag, tag) == false)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Non sono riuscito a rimuovere la tag di nome:<" + tag.Key + "> sul controllo ogg.Name:<" + ogg.Name + ">"));
                return false;
            }
            return true;
        }
        private static bool CtrlParametri(FrameworkElement ogg, KeyValueTuple<string, object> tag)
        {
            if (ogg == null)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Ricevuto parametro ogg a nothing"));
                return false;
            }
            //Una struttura non può essere null
            //if (tag == null) 
            //{ log.Acc(new Mess(Tipi.err, log.errUserText, "Ricevuto parametro tag a nothing"));
            //  return false; }

            if (tag.Key == null)
            {
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "La key del parametro tag è nothing"));
                return false;
            }

            return true;
        }

        //public static ConcurrentDictionary<string, string> Nome(string val = null) { return new ConcurrentDictionary<string, string> { ["nome"] = val }; }
        //public static ConcurrentDictionary<string, object> ValoreGen(object val = null) { return new ConcurrentDictionary<string, object> { ["valoreGen"] = val }; }
        public static KeyValueTuple<string, string> Nome(string val = null) { return new KeyValueTuple<string, string>("nome", val); }
        public static KeyValueTuple<string, object> Valore(object val = null) { return new KeyValueTuple<string, object>("valore", val); }
        public static KeyValueTuple<string, object> ValoreAtt(object val = null) { return new KeyValueTuple<string, object>("valoreAtt", val); }
        public static KeyValueTuple<string, object> ValorePrec(object val = null) { return new KeyValueTuple<string, object>("valorePrec", val); }
        public static KeyValueTuple<string, object> Format(object val = null) { return new KeyValueTuple<string, object>("format", val); }
    }
}