using System;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using Main.Logs;
using System.Windows.Media;

namespace Main.Controls
{
    public static class Control
    {
        //ATTENZIONE: Function Ricorsiva
        ///<summary>
        ///  Ritorna sempre una lista, vuota in caso di errore o nel caso in cui nessun figlio corrisponde 
        ///</summary>
        public static List<UIElement> DammiFigli(DependencyObject oggPadre, Type tipoDaCercare = null, string nomeDaCercare = "", Str.TipiRicerca tipoRicerca = Str.TipiRicerca.uguale,
                                                     bool piuDi1 = true, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            List<UIElement> listaFigli = new List<UIElement>();

            if (oggPadre == null)
            {
                logMess.testoDaLoggare = "Ricevuto parametro oggPadre a null";
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                return listaFigli;
            }

            string nomeOgg;

            foreach (object figlio in LogicalTreeHelper.GetChildren(oggPadre))
            { //ATTENZIONE: sembra che VisualTreeHelper.GetChild restituisce solo i figli visibili

                if (figlio == null || figlio.GetType().IsSubclassOf(typeof(DependencyObject)) == false) continue; //Sembra funzionare anche <Not TypeOf figlio Is DependencyObject>

                if (nomeDaCercare != null && nomeDaCercare != "")
                {
                    if (figlio.GetType().GetProperty("Name") != null)
                    {

                        nomeOgg = (string)figlio.GetType().GetProperty("Name").GetValue(figlio, BindingFlags.GetProperty, null, null, null);

                        switch (tipoRicerca)
                        {
                            case Str.TipiRicerca.uguale:
                                if (nomeOgg == nomeDaCercare)
                                {
                                    listaFigli.Add((UIElement)figlio);
                                    if (piuDi1 == false) return listaFigli;
                                }
                                break;
                            case Str.TipiRicerca.iniziaPer:
                                if (nomeOgg.Left(nomeDaCercare.Length) == nomeDaCercare.ToLower())
                                {
                                    listaFigli.Add((UIElement)figlio);
                                    if (piuDi1 == false) return listaFigli;
                                }
                                break;
                            case Str.TipiRicerca.finiscePer:
                                if (nomeOgg.Right(nomeDaCercare.Length) == nomeDaCercare)
                                {
                                    listaFigli.Add((UIElement)figlio);
                                    if (piuDi1 == false) return listaFigli;
                                }
                                break;
                            case Str.TipiRicerca.contiene:
                                if (nomeOgg.Contains(nomeDaCercare))
                                {
                                    listaFigli.Add((UIElement)figlio);
                                    if (piuDi1 == false) return listaFigli;
                                }
                                break;
                            default:
                                logMess.testoDaLoggare = "Ricevuto valore disatteso per il parametro tipoRicerca:<" + tipoRicerca.ToString() + ">";
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                                return listaFigli;
                        }
                    }
                }
                else if (tipoDaCercare != null)
                {
                    if (figlio.GetType() == tipoDaCercare)
                    {
                        listaFigli.Add((UIElement)figlio);
                        if (piuDi1 == false) return listaFigli;
                    }
                }
                else
                { //Non posso restituire tutti gli elementi 
                    logMess.testoDaLoggare = "Criterio di ricerca disatteso, parametro nome vuoto e tipo a nothing";
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                    return listaFigli;
                }

                foreach (UIElement figlioDelFiglio in DammiFigli((DependencyObject)figlio, tipoDaCercare, nomeDaCercare, tipoRicerca, piuDi1))
                {
                    listaFigli.Add(figlioDelFiglio);
                    if (piuDi1 == false) return listaFigli;
                }
            }
            return listaFigli;
        }



        ///<summary>
        ///  Ritorna sempre una lista, vuota in caso di errore o nel caso in cui nessun figlio corrisponde 
        ///</summary>
        public static List<UIElement> DammiPadri(DependencyObject oggFiglio, Type tipoDaCercare = null, string nomeDaCercare = "", Str.TipiRicerca tipoRicerca = Str.TipiRicerca.uguale,
                                                     bool piuDi1 = false, Mess logMess = null)
        {
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            List<UIElement> listaPadri = new List<UIElement>();

            if (oggFiglio == null)
            {
                logMess.testoDaLoggare = "Ricevuto parametro oggFiglio a null";
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                return listaPadri;
            }

            string nomeOgg;

            DependencyObject padre = VisualTreeHelper.GetParent(oggFiglio);
            //In questo caso meglio usare VisualTreeHelper rispetto a LogicalTreeHelper poichè i controlli non hanno sempre un parent nelle proprietà

            if (padre == null) return listaPadri;

            while (padre != null)
            {

                if (nomeDaCercare != null && nomeDaCercare != "" && padre.GetType().GetProperty("Name") != null)
                {

                    nomeOgg = (string)padre.GetType().GetProperty("Name").GetValue(padre, BindingFlags.GetProperty, null, null, null);

                    switch (tipoRicerca)
                    {
                        case Str.TipiRicerca.uguale:
                            if (nomeOgg == nomeDaCercare)
                            {
                                listaPadri.Add((UIElement)padre);
                                if (piuDi1 == false) return listaPadri;
                            }
                            break;
                        case Str.TipiRicerca.iniziaPer:
                            if (nomeOgg.Left(nomeDaCercare.Length) == nomeDaCercare.ToLower())
                            {
                                listaPadri.Add((UIElement)padre);
                                if (piuDi1 == false) return listaPadri;
                            }
                            break;
                        case Str.TipiRicerca.finiscePer:
                            if (nomeOgg.Right(nomeDaCercare.Length) == nomeDaCercare)
                            {
                                listaPadri.Add((UIElement)padre);
                                if (piuDi1 == false) return listaPadri;
                            }
                            break;
                        case Str.TipiRicerca.contiene:
                            if (nomeOgg.Contains(nomeDaCercare))
                            {
                                listaPadri.Add((UIElement)padre);
                                if (piuDi1 == false) return listaPadri;
                            }
                            break;
                        default:
                            logMess.testoDaLoggare = "Ricevuto valore disatteso per il parametro tipoRicerca:<" + tipoRicerca.ToString() + ">";
                            Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                            return listaPadri;
                    }
                }
                else if (tipoDaCercare != null)
                {
                    if (padre.GetType() == tipoDaCercare)
                    {
                        listaPadri.Add((UIElement)padre);
                        if (piuDi1 == false) return listaPadri;
                    }
                }
                else
                { //Non posso restituire tutti gli elementi 
                    logMess.testoDaLoggare = "Criterio di ricerca disatteso, parametro nome vuoto e tipo a null";
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui deve essere sempre errore
                    return listaPadri;
                }

                padre = VisualTreeHelper.GetParent(padre);

                //padre = .GetParent(padre);
            }

            return listaPadri;
        }



    }

}