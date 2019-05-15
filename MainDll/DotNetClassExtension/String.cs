using Main.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Main.Str;

namespace Main
{
    public static partial class DotNetClassExtension
    {
        public static string Left(this string str, int length) //Lasciare int32, anche se parametro length maggiore di str.Length non da eccezione
        {
            if (str == null) //Serve controllare che sia null
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "ricevuta str a nothing, sarà restituita a nothing"));
                return null;
            }

            if (length < 0)
            {
                length = length * -1;
                if (str.Length > length)
                    length = str.Length - length;
                else
                    return "";
            }
            return str.Substring(0, length);
        }

        public static string Right(this string str, int length) //Lasciare int32, anche se parametro length maggiore di str.Length non da eccezione
        {                       
            if (str == null)  //Serve controllare che sia null
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "ricevuta str a nothing, sarà restituita a nothing"));
                return null;
            }

            if (length < 0)
            {
                length = length * -1;
                if (str.Length > length)
                    length = str.Length - length;
                else
                    return "";
            }
            return str.Substring(str.Length - length, length);
        }

        public static string TotalTrim(this string str)
        {
            if (str == null) //Serve controllare che sia null
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "ricevuta str a nothing, sarà restituita a nothing"));
                return null;
            }

            Int64 lastLength = str.Length;
            str = str.Replace(" ", "");

            while (str.Length != lastLength)
            {
                lastLength = str.Length;
                str = str.Replace(" ", "");
            }

            return str;
        }

        public static string RemoveFinal(this string str, string strFinaleDaRim, bool ancheSpazi = false)
        {
            if (str == null) //Serve controllare che sia null
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "ricevuta str a nothing, sarà restituita a nothing"));
                return null;
            }

            if (str.Length < 1) return "";
            if (strFinaleDaRim.Length < 1) return str;

            if (ancheSpazi == true)
            {
                while (str.Right(1) == " ")
                {
                    if (str.Length < 2) return "";
                    str = str.Left(-1); //rimuovo l'ultimo carattere
                }
            }
            if (str.Length >= strFinaleDaRim.Length && str.Right(strFinaleDaRim.Length) == strFinaleDaRim)
                str = str.Left(-1 * strFinaleDaRim.Length); //rimuovo l'ultimo o gli ultimi caratteri

            return str;
        }

        public static string ParoleMinuMaiu(this string str, MinMai minuMaiu, bool soloPrimaParola = false)
        {
            if (str == null) //Serve controllare che sia null
            {
                Log.main.Add(new Mess(Tipi.ERR, "", "ricevuta str a nothing, sarà restituita a nothing"));
                return null;
            }

            if (str.Length == 0) return "";

            string[] parole; bool isPrimaParola; string tmpParola;
            isPrimaParola = true;
            parole = str.Split(' ');
            tmpParola = "";
            str = "";

            foreach (string parola in parole)
            {
                if (parola.Length > 0 && (soloPrimaParola == false || isPrimaParola == true))
                {
                    if (minuMaiu == MinMai.minu) { tmpParola = parola.Left(1).ToLower() + parola.Right(-1); }
                    if (minuMaiu == MinMai.maiu) { tmpParola = parola.Left(1).ToUpper() + parola.Right(-1); }
                    isPrimaParola = false;
                }
                str += tmpParola + " ";
            }
            return str.RemoveFinal(" ");
        }

        public static bool ContainsIngnoreCase(this string str, string compare)
        {
            return str.IndexOf(compare, StringComparison.OrdinalIgnoreCase) >= 0 ? true : false;
        }
    }
}
