using Main.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Controls
{
    public static class Formats
    {
        public static void Format(FrameworkElement control, string format)
        {
            if (control.GetType() == typeof(TextBox))
            {
                TextBox txt = (TextBox)control;
                int cursorPos = txt.SelectionStart;
                switch (format)
                {
                    case "HH:mm":
                        txt.Text = HHdpmm(txt.Text, ref cursorPos, format);
                        break;
                    case "HH:mm:ss":
                        txt.Text = HHdpmmdpss(txt.Text, ref cursorPos, format);
                        break;
                    case "MM/dd":
                        txt.Text = MMsldd(txt.Text, ref cursorPos, format);
                        break;
                    default:
                        Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto valore disatteso per format:<" + format + ">"));
                        break;
                }
                txt.SelectionStart = cursorPos;
            }
        }

        private static string HHdpmm(string str, ref int posizioneCur, string format)
        {
            string strRicomposta; string[] oraSplit;
            bool esisteMinuti;
            esisteMinuti = false;
            strRicomposta = "";

            if (str.Trim() == "")
            { //serve per valorizzarlo la prima volta
                posizioneCur = 0;
                return "  :";
            }

            oraSplit = str.Split(':');

            if (oraSplit.Count() == 1)
            { //Serve quando cancello ed elimino i 2 punti

                if (posizioneCur == 0) posizioneCur = 1; //Per non far andare in errore l'istruzione sotto
                if (str.Length < posizioneCur) posizioneCur = str.Length; //Per non far andare in errore l'istruzione sotto
                str = str.Left(posizioneCur - 1) + " :" + str.Right(str.Length - posizioneCur);
                posizioneCur = posizioneCur - 1;
                oraSplit = str.Split(':');
            }

            if (oraSplit.Length > 2) //Tolgo eventuali 2 punti successivi se ce ne più di 1
                for (Int16 i = 2; i < oraSplit.Length; i++)
                    oraSplit[1] += oraSplit[i];

            if (oraSplit.Length > 1)
            {  //Minuti
                oraSplit[1] = oraSplit[1].TotalTrim();
                if (oraSplit[1].Length > 2) oraSplit[1] = oraSplit[1].Left(2);
                strRicomposta = oraSplit[1];

                if (oraSplit[1] != "") esisteMinuti = true;
            }

            if (oraSplit.Length > 0)
            {  //ora
                oraSplit[0] = oraSplit[0].TotalTrim();
                switch (oraSplit[0].Length)
                {
                    case 0:
                        strRicomposta = "  :" + strRicomposta;
                        break;
                    case 1:
                        if (esisteMinuti == true)
                            strRicomposta = oraSplit[0] + ":" + strRicomposta;
                        else
                            strRicomposta = oraSplit[0] + " :" + strRicomposta;
                        break;
                    case 2:
                        strRicomposta = oraSplit[0] + ":" + strRicomposta;
                        if (posizioneCur == 2) posizioneCur = 3;
                        break;
                    case 3:
                        strRicomposta = oraSplit[0].Left(2) + ":" + strRicomposta;
                        if (posizioneCur == 2) posizioneCur = 3;
                        break;
                }
            }
            return strRicomposta;
        }

        private static string HHdpmmdpss(string str, ref int posizioneCur, string format)
        {
            string strRicomposta; string[] oraSplit;
            bool esisteMinuti, esisteSecondi;
            esisteSecondi = false;
            esisteMinuti = false;
            strRicomposta = "";


            if (str.Trim() == "")
            { //Orelse stringa = " " Then 'serve per valorizzarlo la prima volta
                posizioneCur = 0;
                return "  :  :";
            }

            oraSplit = str.Split(':');

            if (oraSplit.Length == 1)
            {
                //If posizioneCur = 0 Then posizioneCur = 1 //Per non far andare in errore l'istruzione sotto
                if (str.Length < posizioneCur) posizioneCur = str.Length; //Per non far andare in errore l'istruzione sotto
                str = str.Left(posizioneCur) + " :" + "  :" + str.Right(str.Length - posizioneCur);
                //posizioneCur = posizioneCur - 1
                oraSplit = str.Split(':');
            }

            if (oraSplit.Count() == 2)
            { //Serve quando cancello ed elimino uno dei 2 punti 
                if (posizioneCur == 0) posizioneCur = 1; //Per non far andare in errore l'istruzione sotto
                if (str.Length < posizioneCur) posizioneCur = str.Length; //Per non far andare in errore l'istruzione sotto
                str = str.Left(posizioneCur - 1) + "  :" + str.Right(str.Length - posizioneCur);
                posizioneCur = posizioneCur - 1;
                oraSplit = str.Split(':');
            }

            if (oraSplit.Length > 3)
                for (Int16 i = 3; i < oraSplit.Length; i++)
                    oraSplit[2] += oraSplit[i];

            if (oraSplit.Length > 2)
            {  //Secondi
                oraSplit[2] = oraSplit[2].TotalTrim();
                if (oraSplit[2].Length > 2) oraSplit[2] = oraSplit[2].Left(2);
                strRicomposta = oraSplit[2];

                if (oraSplit[2] != "") esisteSecondi = true;
            }


            if (oraSplit.Length > 1)
            {  //Minuti
                oraSplit[1] = oraSplit[1].TotalTrim();
                if (oraSplit[1].Length > 2) oraSplit[1] = oraSplit[1].Left(2);

                if (oraSplit[1].Length == 2)
                {
                    strRicomposta = oraSplit[1] + ":" + strRicomposta;
                    if (posizioneCur == 5) posizioneCur = 6;
                }
                else
                    strRicomposta = oraSplit[1] + " :" + strRicomposta;

                if (oraSplit[1] != "") esisteMinuti = true;
            }


            if (oraSplit.Length > 0)
            { //ora
                oraSplit[0] = oraSplit[0].TotalTrim();
                switch (oraSplit[0].Length)
                {
                    case 0:
                        strRicomposta = "  :" + strRicomposta;
                        break;
                    case 1:
                        if (esisteSecondi == true || esisteMinuti == true)
                            strRicomposta = oraSplit[0] + ":" + strRicomposta;
                        else
                            strRicomposta = oraSplit[0] + " :" + strRicomposta;
                        break;
                    case 2:
                        strRicomposta = oraSplit[0] + ":" + strRicomposta;
                        if (posizioneCur == 2) posizioneCur = 3;
                        break;
                    case 3:
                        strRicomposta = oraSplit[0].Left(2) + ":" + strRicomposta;
                        if (posizioneCur == 2) posizioneCur = 3;
                        break;
                }
            }
            return strRicomposta;

        }

        private static string MMsldd(string str, ref int posizioneCur, string format)
        {
            string strRicomposta; string[] dataSplit;
            bool esisteGiorno, digitataBarra;
            esisteGiorno = false;
            digitataBarra = false;
            strRicomposta = "";

            if (str.Trim() == "")
            { //serve per valorizzarlo la prima volta
                posizioneCur = 0;
                return "  /";
            }
            str = str.Replace("\\", "/");

            dataSplit = str.Split('/');

            if (dataSplit.Count() == 1)
            { //Serve quando cancello la barra o nel caso in cui ho appena iniziato a scrivere
                if (posizioneCur == 0) posizioneCur = 1; //Per non far andare in errore l'istruzione sotto
                if (str.Length < posizioneCur) posizioneCur = str.Length; //Per non far andare in errore l'istruzione sotto
                if (str.Length <= 1) //Significa che ho appena iniziato a scrivere
                    str = str.Left(posizioneCur) + " /";
                else                 //Significa che ho cancellato la barra
                {
                    str = str.Left(posizioneCur - 1) + " /" + str.Right(str.Length - posizioneCur);
                    posizioneCur = posizioneCur - 1;
                }
                
                dataSplit = str.Split('/');
            }

            if (dataSplit.Length > 2) //Tolgo eventuali 2 slash successivi se ce ne più di 1
            {
                digitataBarra = true;
                for (Int16 i = 2; i < dataSplit.Length; i++)
                    dataSplit[1] += dataSplit[i];
            }

            if (dataSplit.Length > 1)
            {  //Minuti
                dataSplit[1] = dataSplit[1].TotalTrim();
                if (dataSplit[1].Length > 2) dataSplit[1] = dataSplit[1].Left(2);
                strRicomposta = dataSplit[1];

                if (dataSplit[1] != "") esisteGiorno = true;
            }

            //ora
            dataSplit[0] = dataSplit[0].TotalTrim();
            switch (dataSplit[0].Length)
            {
                case 0:
                    strRicomposta = "  /" + strRicomposta;
                    break;
                case 1:
                    if (esisteGiorno == true || digitataBarra == true)
                        strRicomposta = dataSplit[0] + "/" + strRicomposta;
                    else
                        strRicomposta = dataSplit[0] + " /" + strRicomposta;
                    break;
                case 2:
                    strRicomposta = dataSplit[0] + "/" + strRicomposta;
                    if (posizioneCur == 2) posizioneCur = 3;
                    break;
                case 3:
                    strRicomposta = dataSplit[0].Left(2) + "/" + strRicomposta;
                    if (posizioneCur == 2) posizioneCur = 3;
                    break;
            }

            return strRicomposta;
        }
    }
}
