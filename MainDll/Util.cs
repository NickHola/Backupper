using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;
using System.Security.Cryptography;
using Main.Logs;
using Main.Validations;
using System.Collections;
using System.Collections.Generic;

namespace Main
{
    public static partial class Util
    {
        public static readonly string crLf = Environment.NewLine;

        public static Random rnd = new Random();  //Non può stare dentro a Str.GeneraRandom altrimenti si rinizializza ogni volta e restituisce lo stesso numero
        public static MD5 md5;

        public static bool IsDesignTime
        { get { return LicenseManager.UsageMode == LicenseUsageMode.Designtime; } }

        public static object GetResource(string nome, Uri uriPath = null, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            try
            { return Application.Current.FindResource(nome); }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = "Risorsa non trovata per il nome:<" + nome + ">, ex.mess:<" + ex.Message + ">";
                Log.main.Add(logMess);
                return null;
            }
        }

        public static object GetResourceFromDictionary(string nome, string uriPath, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            try
            {
                var resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(uriPath, UriKind.Absolute);
                return (Style)resourceDictionary[nome];
            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = "Risorsa non trovata nel dizionario:<" + uriPath + "> per il nome:<" + nome + ">, ex.mess:<" + ex.Message + ">";
                Log.main.Add(logMess);
                return null;
            }
        }

        public static Color DammiColoreDaEsadec(string esaDec)
        {
            if (esaDec == null)
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Ricevuto esaDec a nothing")));
            try
            {
                Color colore = Color.FromRgb(Convert.ToByte(esaDec.Substring(0, 2), 16), Convert.ToByte(esaDec.Substring(2, 2), 16), Convert.ToByte(esaDec.Substring(4, 2), 16));
                return colore;
            }
            catch (Exception ex)
            { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">"))); }
        }

        public static SolidColorBrush DammiBrushDaEsadec(string esaDec)
        {
            try
            {
                return new SolidColorBrush(DammiColoreDaEsadec(esaDec));
            }
            catch (Exception ex)
            { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Eccezione ex.mess:<" + ex.Message + ">"))); }
        }

        public static void DoEvents(int sleep = 1)
        {
            if (sleep > 0)
                Thread.Sleep(sleep);
            System.Windows.Forms.Application.DoEvents();

            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(Sub()  'Non funziona per i binding dell'interfaccia
            //                                                                                End Sub))
        }

        public static string GetCallStack(byte numMetodoPiùLontano = 6, byte numMetodoPiùVicino = 2, byte dammiSoloSubLiv = 0)
        {
            string strOut;
            if (dammiSoloSubLiv == 0)
            {
                strOut = "|Sub:";
                for (byte i = numMetodoPiùLontano; i >= numMetodoPiùVicino; i -= 1) //numMetodoPiùVicino a 2 poichè 1 e lo 0 sono rispettivamente SubName e Accoda
                {
                    strOut += new StackFrame(i, true).GetMethod().Name + "->";
                }

                strOut = strOut.RemoveFinal("->");
                strOut += " | ";
                return strOut;  //"|Sub:" & subPrec4 & "->" & subPrec3 & "->" & subPrec2 & "->" & subPrec1 & " | "
            }
            else
            {
                strOut = new StackFrame(dammiSoloSubLiv, true).GetMethod().Name;
                return strOut;
            }
        }

        public struct KeyValueTuple<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            public KeyValueTuple(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }
        }


        //TODO commentare AggiungiStiliAdEsistente vecchio e rinominare AddStyleToAnExistingOne in nuovo

        //public static Style AddStyleToAnExistingOneOld(Style stileAttuale, Style[] stiliAggiuntivi, Mess logMess = null)
        //{
        //    if (Util.IsDesignTime == true) return stileAttuale; //Xaml rendering goes in error with code line: "(Style)XamlReader.Parse(XamlWriter.Save(stileAttuale.BasedOn));", to avoid this check IsDesignTime

        //    if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

        //    if (stiliAggiuntivi == null)
        //    {
        //        logMess.testoDaLoggare = "Ricevuto stileAggiuntivo a null";
        //        Log.main.Add(logMess);
        //        return stileAttuale;
        //    }

        //    stiliAggiuntivi = stiliAggiuntivi.Where(item => item != null).ToArray();

        //    Style finalStyle;

        //    try
        //    {
        //        if (stileAttuale != null)
        //        {
        //            finalStyle = (Style)XamlReader.Parse(XamlWriter.Save(stileAttuale));

        //            if (stileAttuale.BasedOn != null)
        //                finalStyle.BasedOn = (Style)XamlReader.Parse(XamlWriter.Save(stileAttuale.BasedOn));
        //        }
        //        else
        //        { finalStyle = new Style(); }

        //        if (finalStyle.BasedOn == null)
        //            finalStyle.BasedOn = new Style();

        //        foreach (Style styleToCopy in stiliAggiuntivi)
        //        {
        //            CopiaStile(finalStyle.BasedOn, styleToCopy); //TODO
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("designTime error final step");
        //        MessageBox.Show("errore1" + ex.Message); //TODO errore
        //        return stileAttuale;
        //    }

        //    return finalStyle;
        //}

        ///summary///
        ///Questo metodo consente il merge di più stili anche se lo stile attuale o gli stiliAggiuntivi sono IsSaled, partendo da uno stile vergine si copiano all'interno tutte le proprietà degli altri stili
        ///summary///
        ///<param name="typeToSet">Serve per sapere quale targetType impostare a tutti gli stili nel caso in cui lo stile attuale fosse null</param>  
        public static Style AddStylesToAnExistingOne(Style currentStyle, Style[] stylesToAdd, Type typeToSet, Style defaultStyle = null, Mess logMess = null)
        {
            if (Util.IsDesignTime == true) return currentStyle; //Xaml rendering goes in error with code line: "(Style)XamlReader.Parse(XamlWriter.Save(stileAttuale.BasedOn));", to avoid this check IsDesignTime

            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            if (stylesToAdd == null)
            {
                logMess.testoDaLoggare = "Ricevuto stileAggiuntivo a null";
                Log.main.Add(logMess);
                return currentStyle;
            }

            stylesToAdd = stylesToAdd.Where(item => item != null).ToArray();

            if (currentStyle != null)
                typeToSet = currentStyle.TargetType;
            else
            {
                if (typeToSet == null)
                {
                    logMess.testoDaLoggare = "Impossobile impostare targetType agli stili poichè sia stileAttuale che typeToSet sono a null";
                    Log.main.Add(logMess);
                    return currentStyle;
                }
            }

            Style finalStyle = new Style(typeToSet);

            try
            {
                foreach (Style styleToCopy in stylesToAdd)
                {
                    StyleCopy(finalStyle, styleToCopy);
                }
                if (currentStyle != null) StyleCopy(finalStyle, currentStyle);
                if (defaultStyle != null) finalStyle.BasedOn = defaultStyle;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("designTime error final step");
                MessageBox.Show("errore1" + ex.Message); //TODO errore
                return currentStyle;
            }

            return finalStyle;
        }

        private static void StyleCopy(Style currentStyle, Style styleToAdd)
        {
            if (styleToAdd.BasedOn != null)
                StyleCopy(currentStyle, styleToAdd.BasedOn);


            //foreach (object key in stileAggiuntivo.Resources.Keys)
            //    stileAttuale.Resources[key] = stileAggiuntivo.Resources[key];
            foreach (var resorces in styleToAdd.Resources)
            {
                if (resorces.GetType() == typeof(DictionaryEntry))
                    currentStyle.Resources.Add(((DictionaryEntry)resorces).Key, ((DictionaryEntry)resorces).Value);

                else if (resorces.GetType() == typeof(ResourceDictionary))
                    currentStyle.Resources.MergedDictionaries.Add((ResourceDictionary)resorces);

                else
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "Trovato nuovo resorces.GetType:<" + resorces.GetType().ToString() + ">"));
            }
            foreach (TriggerBase trigger in styleToAdd.Triggers)
            {
                //string text = "";
                //Serializes.Serializ.SerializzaInTesto(trigger, ref text);
                //TriggerBase trgDeserial = (TriggerBase)Activator.CreateInstance(trigger.GetType());
                //Serializes.Serializ.DeserializzaDaTesto(text, ref trgDeserial);
                currentStyle.Triggers.Add(trigger);
            }
            foreach (SetterBase setter in styleToAdd.Setters)
            {
                currentStyle.Setters.Add(setter);
            }

        }

        public static bool GetPropertyOrFieldValue<T1>(object obj, string propertyName, out T1 returnValue)
        {
            returnValue = default(T1);
            try
            {
                Type myType = obj.GetType();
                PropertyInfo propInfo = myType.GetProperty(propertyName);
                returnValue = (T1)propInfo.GetValue(obj, null);
                return true;
            }
            catch
            { }

            try
            {
                Type myType = obj.GetType();
                FieldInfo fieldInfo = myType.GetField(propertyName);
                returnValue = (T1)fieldInfo.GetValue(obj);
                return true;
            }
            catch
            { }
            return false;
        }

        //public static void ExposeProperty<T>(Expression<Func<T>> property)
        //{
        //    var expression = GetMemberInfo(property);
        //    string path = string.Concat(expression.Member.DeclaringType.FullName,
        //        ".", expression.Member.Name);
        //    // Do ExposeProperty work here...
        //}
    }

    public static class PerformWatch
    {
        private static DateTime start;
        //private static SortedList perform;
        private static Queue<WatchObj> perform;
        public static void Inizia()
        {
            if (perform == null) perform = new Queue<WatchObj>();
            perform.Clear();
            start = DateTime.Now;
        }
        public static void Segna(string step = "")
        {
            if (perform == null) return;
            perform.Enqueue(new WatchObj(step, (DateTime.Now - start).TotalMilliseconds));
            //perform.Add(step, (DateTime.Now - start).TotalMilliseconds);
        }

        public static void Fine()
        {
            perform.Enqueue(new WatchObj("End", (DateTime.Now - start).TotalMilliseconds));
            string totale = "";
            //foreach (DictionaryEntry step in perform)
            //{
            //    totale += step.Key + ": " + Math.Round((double)step.Value, 0) + "ms" + Util.crLf;
            //}
            foreach (WatchObj watchObj in perform)
                totale += watchObj.Step + ": " + Math.Round((double)watchObj.TotMs, 0) + "ms" + Util.crLf;

            MessageBox.Show(totale);
        }

        private class WatchObj
        {
            public string Step { get; set; }
            public double TotMs { get; set; }

            public WatchObj(string step, double totMs)
            {
                Step = step;
                TotMs = totMs;
            }
        }
    }

    public class GenericEventArgs : EventArgs
    {
        public string nome, valore, descErr;
        public bool inErr;

        public GenericEventArgs(string nome = "", string valore = "", string descErr = "", bool inErr = false)
        {
            this.nome = nome;
            this.valore = valore;
            this.descErr = descErr;
            this.inErr = inErr;
        }
    }

    public class Progressione : INotifyPropertyChanged
    {

        private Double percentage;
        private bool isComplete;

        public event EventHandler Completed;
        public event EventHandler EndedWithErrors;

        public Double Percentage
        {
            get { return percentage; }
            set
            {
                if (value > 100) value = 100;
                percentage = value;
                OnPropertyChanged();
            }
        }

        public bool IsComplete
        {
            get { return isComplete; }
            set
            {
                bool scatenaEvento = false;
                if (value == true && isComplete == false) scatenaEvento = true; //Serve per non far scatenare l'evento più volte nel caso in sui si setta completa a true più volte
                isComplete = value;
                OnPropertyChanged();
                if (scatenaEvento == true) Completed?.Invoke(this, new EventArgs()); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
            }
        }

        public Progressione()
        {
            IsComplete = false;
            Percentage = 0;
        }

        public void ScatenaEventoTerminataConErrori(String descErr = "")
        {
            GenericEventArgs parametri = new GenericEventArgs(descErr: descErr);
            EndedWithErrors?.Invoke(this, parametri); //?.Invoke invice della parentesi tonda diretta poichè se nessuno ha sottoscritto l'evento non va in eccezione
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    [Serializable]
    public class ConfigWndDiDiagnostica : Configs.ConfigBase, INotifyPropertyChanged //Ogni classe che eredita da config deve avere un nome univoco in tutta l'app poichè viene salvato
    {
        private double zoom;
        public double Zoom
        {
            get { return zoom; }
            set //TODO internal da errore sullo xaml poichè dice che è di sola luettura - Settabile solo all'interno della dll
            {
                Validation.CtrlValue(value);
                zoom = value;
                OnPropertyChanged();
            }
        }


        [JsonConstructor]
        public ConfigWndDiDiagnostica(string savableName) : base(savableName, distinguiUtente: false, distinguiNomePc: false)  //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
        {
            Zoom = 1.2;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

}