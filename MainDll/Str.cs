using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using Main.Logs;

namespace Main
{
    public static class Str //Alcune funzioni statiche non sono più in questo file poichè diventate Estensione del tipo string
    {
        public const string relativo = @".\"; //percorsi e nomi relativi, con lo \ sono sicuro che non lo va a scrivere direttamente sul disco
        public static string[] arrayRnd = { "P", "\"", "#", "$", "%", "&", "(", ")", "*", "+", ",", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?", "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "!", "O", "²", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_", "{", "|", "}", "~", "Ç", "â", "à", "ê", "ë", "è", "ï", "î", "ì", "Ä", "Å", "É", "Æ", "ô", "ò", "û", "ù", "ÿ", "Ö", "Ü", "£", "Ø", "×", "ƒ", "á", "í", "ó", "ú", "Ñ", "ª", "º", "¿", "®", "½", "¼", "¡", "«", "»", "©", "¢", "¥", "¤", "Ð", "¦", "ß", "Õ", "µ", "þ", "Ý", "¯", "´", "±", "¾", "¶", "•", "÷", "¸", "°", "¨", "·", "¹", "³", "N" };

        [Serializable]
        public enum MinMai
        {
            minu,
            maiu
        }

        [Serializable]
        public enum TipiRandom
        {
            tuttiCarattariStampabili,
            soloLettereNumeri,
            soloLettere,
            soloNumeri
        }

        [Serializable]
        public enum TipiRicerca
        {
            _Nothing,
            uguale,
            iniziaPer,
            finiscePer,
            contiene
        }
        
        public static string GeneraRandom(UInt16 lunghezza, TipiRandom TipoStrRandom = TipiRandom.tuttiCarattariStampabili)
        {
            if (lunghezza == 0) return "";
            string strRandom = "";
            switch (TipoStrRandom)
            {
                case TipiRandom.tuttiCarattariStampabili:
                    for (int i = 1; i <= lunghezza; i += 1)
                    { strRandom += arrayRnd[Util.rnd.Next(0, 129)]; }  //va da 0 a 128, arrayRnd è 
                    break;
                case TipiRandom.soloLettereNumeri:
                    for (int i = 1; i <= lunghezza; i += 1)
                    {
                        if (Util.rnd.Next(0, 2) == 0)  //va da 0 a 1
                            strRandom += (char)(Util.rnd.Next(48, 58)); //genera cifre (tabella ascii da 48 a 57) 
                        else
                            strRandom += (char)(Util.rnd.Next(65, 91)); //genera lettere (così il generatore va da 65 a 90)
                    }
                    break;
                case TipiRandom.soloLettere:
                    for (int i = 1; i <= lunghezza; i += 1)
                    { strRandom += (char)(Util.rnd.Next(65, 91)); } //genera lettere (così il generatore va da 65 a 90)
                    break;
                case TipiRandom.soloNumeri:
                    for (int i = 1; i <= lunghezza; i += 1)
                    { strRandom += (char)(Util.rnd.Next(48, 58)); } //genera cifre (tabella ascii da 48 a 57) 
                    break;
            }
            return strRandom;
        }

        public static bool CalcolaDimensioni(out double larghezza, out double altezza, string testo, FontFamily fontFamily, double dimFont, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStrech)
        {
            if (testo == "") testo = "Z";

            if (fontFamily == null) fontFamily = new FontFamily("Segoe UI");

            if (fontStyle == null) fontStyle = FontStyles.Normal;

            if (fontWeight == null) fontWeight = FontWeights.Normal;

            if (fontStrech == null) fontStrech = FontStretches.Normal;

            FormattedText formattedText;
            formattedText = new FormattedText(testo, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(fontFamily, fontStyle, fontWeight, fontStrech), dimFont, Brushes.Black);

            larghezza = formattedText.Width;
            altezza = formattedText.Height;

            return true;
        }

    }
}
