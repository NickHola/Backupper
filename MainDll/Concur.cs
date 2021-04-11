using System;
using System.Collections.Concurrent;
using Main.DataOre;
using Main.Logs;

namespace Main.Concurs
{
    static class Concur
    {

        #region "Dictionary"

        public static bool Dictionary_TryAddOrUpdate<T1, T2>(ConcurrentDictionary<T1, T2> dizionario, object keyValue, UInt32 timeOutMs = 150, bool noUpadate = false, Mess logMess = null)
        { //Concurrent.ConcurrentDictionary(Of Object, Object)
            DateTime oraInizio = DateTime.MinValue;
            T1 key; T2 value, currentValue;
            currentValue = default(T2);

            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (dizionario == null)
            {
                logMess.testoDaLoggare = "ricevuto dizionario a null";
                Log.main.Add(logMess);
                return false;
            }

            if (keyValue == null)
            {
                logMess.testoDaLoggare = "ricevuto keyValue a null";
                Log.main.Add(logMess);
                return false;
            }

            if (Util.GetPropertyOrFieldValue(keyValue, "Key", out key) == false)
            {
                logMess.testoDaLoggare = "ricevuto keyValue senza proprietà o campo key";
                Log.main.Add(logMess);
                return false;
            }

            if (Util.GetPropertyOrFieldValue(keyValue, "Value", out value) == false)
            {
                logMess.testoDaLoggare = "ricevuto keyValue senza proprietà o campo value";
                Log.main.Add(logMess);
                return false;
            }


            if (dizionario.ContainsKey(key) == true)
            {
                if (noUpadate == true)
                {
                    logMess.testoDaLoggare = "il dizionario contiene già la chiave:<" + key.ToString() + ">";
                    Log.main.Add(logMess);
                    return false;
                }

                if (Dictionary_TryGet(dizionario, key, ref currentValue, out _) == false)
                {
                    logMess.testoDaLoggare = "il dizionario contiene la chiave:<" + key.ToString() + ">, ma non riesco a recuperare il valore";
                    Log.main.Add(logMess);
                    return false;
                }

                while (dizionario.TryUpdate(key, value, currentValue) == false)
                {
                    if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now; //Messo dentro il while per eseguire meno istruzioni possibili fuori (che è percorso normale)

                    if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                    {
                        logMess.testoDaLoggare = "TryUpdate raggiunto timeOutMs:<" + timeOutMs + ">, per la chiave:<" + key.ToString() + ">";
                        Log.main.Add(logMess);
                        return false;
                    }
                }
            }
            else
            {
                while (dizionario.TryAdd(key, value) == false)
                {
                    if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now; //Messo dentro il while per eseguire meno istruzioni possibili fuori (che è percorso normale)

                    if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                    {
                        logMess.testoDaLoggare = "TryAdd raggiunto timeOutMs:<" + timeOutMs + ">, per la chiave:<" + key.ToString() + ">";
                        Log.main.Add(logMess);
                        return false;
                    }
                }

            }

            return true;
        }

        public static bool Dictionary_TryGet<T1, T2>(ConcurrentDictionary<T1, T2> dizionario, T1 chiave, ref T2 valore, out bool esiste, UInt32 timeOutMs = 150, Mess logMess = null)
        { //Concurrent.ConcurrentDictionary(Of Object, Object)
            DateTime oraInizio = DateTime.MinValue;
            esiste = false;

            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (dizionario == null)
            {
                logMess.testoDaLoggare = "ricevuto dizionario a nothing";
                Log.main.Add(logMess);
                return false;
            }

            if (chiave == null)
            {
                logMess.testoDaLoggare = "ricevuto chiave a nothing";
                Log.main.Add(logMess);
                return false;
            }

            if (dizionario.ContainsKey(chiave) == true)
            {
                esiste = true;
                while (dizionario.TryGetValue(chiave, out valore) == false)
                {
                    if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now; //Inizializzazione dentro poichè normalmente qui non ci entrerà e quindi vado più veloce

                    if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                    {
                        logMess.testoDaLoggare = "TryGetValue raggiunto timeOutMs:<" + timeOutMs + ">, per la chiave:<" + chiave.ToString() + ">";
                        Log.main.Add(logMess);
                        return false;
                    }
                }
            }
            else
            { return false; }

            return true;
        }

        public static bool Dictionary_TryRemove<T1, T2>(ConcurrentDictionary<T1, T2> dizionario, object keyOrKeyValue, UInt32 timeOutMs = 150, Mess logMess = null)
        { //Concurrent.ConcurrentDictionary(Of Object, Object)
            T1 key; DateTime oraInizio = DateTime.MinValue; T2 tmpObj;

            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (dizionario == null)
            {
                logMess.testoDaLoggare = "ricevuto dizionario a nothing";
                Log.main.Add(logMess);
                return false;
            }

            if (keyOrKeyValue == null)
            {
                logMess.testoDaLoggare = "ricevuto keyOrKeyValue a nothing";
                Log.main.Add(logMess);
                return false;
            }


            if (Util.GetPropertyOrFieldValue(keyOrKeyValue, "Key", out key) == true) {
                if (key == null)
                {
                    logMess.testoDaLoggare = "ricevuto keyOrKeyValue.key a null";
                    Log.main.Add(logMess);
                    return false;
                }
                //if (keyOrKeyValue.GetType().GetProperty("Key") != null || keyOrKeyValue.GetType().GetField("Key") != null)
                //{
                //    if (keyOrKeyValue.key == null)
                //    {
                //        logMess.testoDaLoggare = "ricevuto keyOrKeyValue.key a nothing";
                //        Log.main.Acc(logMess);
                //        return false;
                //    }
                //    key = keyOrKeyValue.key;
                //}
            }
            else
            { key = (T1)keyOrKeyValue; }


            if (dizionario.ContainsKey(key) == false) return true;

            while (dizionario.TryRemove(key, out tmpObj) == false)
            {
                if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now; //Inizializzazione dentro poichè normalmente qui non ci entrerà e quindi vado più veloce

                if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                {
                    logMess.testoDaLoggare = "TryRemove raggiunto timeOutMs:<" + timeOutMs + ">, per la keyOrKeyValue.key:<" + key.ToString() + ">";
                    Log.main.Add(logMess);
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region "Queue"

        public static bool Queue_TryPeek<T1>(ConcurrentQueue<T1> queue, ref T1 oggetto, UInt32 timeOutMs = 150, Mess logMess = null)
        { //queue As Concurrent.ConcurrentQueue(Of Object)
            DateTime oraInizio = DateTime.MinValue;

            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (queue == null)
            {
                logMess.testoDaLoggare = "ricevuto queue a nothing";
                Log.main.Add(logMess);
                return false;
            }

            if (queue.Count == 0) return false;

            while (queue.TryPeek(out oggetto) == false)
            {
                if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now;  //Inizializzazione dentro poichè normalmente qui non ci entrerà e quindi vado più veloce
                if (queue.Count == 0) return false;

                if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                {
                    logMess.testoDaLoggare = "TryPeek raggiunto timeOutMs:<" + timeOutMs + ">";
                    Log.main.Add(logMess);
                    return false;
                }
            }
            return true;
        }

        public static bool Queue_TryDequeue<T1>(ConcurrentQueue<T1> queue, ref T1 oggetto, UInt32 timeOutMs = 150, Mess logMess = null)
        { //queue As Concurrent.ConcurrentQueue(Of Object)
            DateTime oraInizio = DateTime.MinValue;
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (queue == null)
            {
                logMess.testoDaLoggare = "ricevuto queue a nothing";
                Log.main.Add(logMess);
                return false;
            }

            if (queue.Count == 0) return false;

            while (queue.TryDequeue(out oggetto) == false)
            {
                if (oraInizio == DateTime.MinValue) oraInizio = DateTime.Now;  //Inizializzazione dentro poichè normalmente qui non ci entrerà e quindi vado più veloce
                if (queue.Count == 0) return false;

                if (DataOra.AttesaTempo(ref oraInizio, timeOutMs) == true)
                {
                    logMess.testoDaLoggare = "TryDequeue raggiunto timeOutMs:<" + timeOutMs + ">";
                    Log.main.Add(logMess);
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
