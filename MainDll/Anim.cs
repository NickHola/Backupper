using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Main.Logs;

namespace Main
{
    public static class Anim
    {
        public static bool CreaBloccoAnimazioni(Style stile, List<Condition> condizioni, List<Timeline> animazioniEntrata, List<Timeline> animazioniUscita = null,
                                     bool RemoveStbInUscita = true, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(Tipi.Warn, Log.main.warnUserText);

            if (stile == null)
            {
                logMess.testoDaLoggare = "ricevuto stile a null";
                Log.main.Add(logMess);
                return false;
            }

            if (condizioni == null)
            {
                logMess.testoDaLoggare = "ricevuto condizioni a null";
                Log.main.Add(logMess);
                return false;
            }

            if (animazioniEntrata == null)
            {
                logMess.testoDaLoggare = "ricevuto animazioniEntrata a null";
                Log.main.Add(logMess);
                return false;
            }

            if (animazioniEntrata.Count == 0)
            {
                logMess.testoDaLoggare = "ricevuta lista animazioniEntrata vuota";
                Log.main.Add(logMess);
                return false;
            }


            object trgMain = null;
            Storyboard stbMain = new Storyboard();

            switch (condizioni.Count) {
                case 0:
                    logMess.testoDaLoggare = "ricevuta lista condizioni vuota";
                    Log.main.Add(logMess);
                    return false;
                case 1:
                    trgMain = new Trigger();
                    (trgMain as Trigger).Property = condizioni[0].Property;
                    (trgMain as Trigger).Value = condizioni[0].Value;
                    break;
                case int n when (n > 1):
                    trgMain = new MultiTrigger();
                    foreach (Condition condizione in condizioni) { 
                        (trgMain as MultiTrigger).Conditions.Add(condizione);
                    }
                    break;
            }

            //Animazioni in trigger EnterActions
            Storyboard stbEnterAction = new Storyboard();

            foreach (Timeline animazione in animazioniEntrata) {
                stbEnterAction.Children.Add(animazione);
            }

            //Animazioni in trigger ExitActions
            Storyboard stbExitAction;

            if (animazioniUscita != null && animazioniUscita.Count == 0) animazioniUscita = null;

            if (animazioniUscita != null) {
                stbExitAction = new Storyboard();
                foreach (Timeline animazione in animazioniEntrata) {
                    stbEnterAction.Children.Add(animazione);
                }
            }

            string nomeBeginStoryboard = Str.GeneraRandom(5, Str.TipiRandom.soloLettere);

            (trgMain as TriggerBase).EnterActions.Add(new BeginStoryboard() { Name = nomeBeginStoryboard, Storyboard = stbEnterAction });
            if (animazioniUscita == null && RemoveStbInUscita == true) (trgMain as TriggerBase).ExitActions.Add(new RemoveStoryboard() { BeginStoryboardName = nomeBeginStoryboard });

            stile.RegisterName(nomeBeginStoryboard, (trgMain as TriggerBase).EnterActions[0]);
            stile.Triggers.Add((trgMain as TriggerBase));

            return true;
        }

        public static void ProgressColorAnimationPerPerc(Color inizio, Color fine, ref Dictionary<Double, Color> passiColore) { 
            Double[] stepsRGB = new double[3];

            stepsRGB[0] = (fine.R - inizio.R) / 100;
            stepsRGB[1] = (fine.G - inizio.G) / 100;
            stepsRGB[2] = (fine.B - inizio.B) / 100;

            passiColore.Add(0, inizio);

            for (int i=1; i<=100; i++) { 
                passiColore.Add(i, Color.FromRgb(Convert.ToByte(inizio.R + (stepsRGB[0] * i)), Convert.ToByte(inizio.G + (stepsRGB[1] * i)), Convert.ToByte(inizio.B + (stepsRGB[2] * i))));
            }
        }

        public static SolidColorBrush AnimaPropDaPassiColore(Dictionary<Double, Color> passiColore, Double valore, SolidColorBrush propDaAnimare, TimeSpan tempo = default) {
            byte intero = Convert.ToByte(Math.Round(valore, 0));
            if (passiColore.ContainsKey(intero) == false) return propDaAnimare;
            Color colore = passiColore[intero];

            if (colore == propDaAnimare.Color) return propDaAnimare;
            if (tempo == TimeSpan.FromSeconds(0)) tempo = TimeSpan.FromSeconds(0.4);
            ColorAnimation colorAnim = new ColorAnimation(colore, tempo);

            //Dim eccoloo As New SolidColorBrush(CType(Me.Foreground, SolidColorBrush).Color)
            SolidColorBrush colorBrush = new SolidColorBrush(propDaAnimare.Color);
            colorBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            //Me.Foreground = eccoloo
            //propDaAnimare = colorBrush;
            return colorBrush;
            //Me.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ciao)
        }

    }
}
