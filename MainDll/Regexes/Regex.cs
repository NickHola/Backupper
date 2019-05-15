using System;
using System.Collections.Generic;
using Main.Logs;

namespace Main.Regexes
{
    public static class Regex
    {
        public static bool RegexMatch(string daValidare, object espressioni, out bool match, Mess logMess = null)
        {
            match = false;

            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

            if (daValidare == null)
            {
                logMess.testoDaLoggare = "ricevuto daValidare a nothing";
                Log.main.Add(logMess);
                return false;
            }
            if (espressioni == null)
            {
                logMess.testoDaLoggare = "ricevuto espressioni a nothing";
                Log.main.Add(logMess);
                return false;
            }
            if (espressioni.GetType() == typeof(string))
            { match = RM_Valida(daValidare, (string)espressioni, ref match, logMess); }
            else if (espressioni.GetType() == typeof(List<string>))
            {
                foreach (string espressione in (List<string>)espressioni)
                {
                    if (RM_Valida(daValidare, espressione, ref match, logMess) == false) return false;
                    if (match == true) break;
                }
            }
            else
            {
                logMess.testoDaLoggare = "ricevuto tipo sconosciuto per espressioni:<" + espressioni.GetType().Name + ">";
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }

        private static bool RM_Valida(string daValidare, string espressione, ref bool risultatoMatch, Mess logMess)
        {
            try
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(espressione);
                risultatoMatch = regex.Match(daValidare).Success;

            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = "eccezione ex.mess:<" + ex.Message + ">";
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }

        public static bool CheckRegexSyntax(string expression, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.errUserText);

            try
            {
                System.Text.RegularExpressions.Regex.Match("", expression);
            }
            catch (Exception ex)
            {
                logMess.testoDaLoggare = "sintassi non valida, espressione:<" + expression + ">, errore:<" + ex.Message + ">";
                Log.main.Add(logMess);
                return false;
            }
            return true;
        }
    }
}
