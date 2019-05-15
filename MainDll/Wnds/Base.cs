using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Main.Logs;
using Main.Salvable;

namespace Main.Wnds
{
    public class Base : Window
    {
        //private Configs.SaveLocation saveLocation;
        private object saveLocation;
        private ConfigWnd config;
        private ContextMenu cntMenuPrinc;
        internal bool salvaConfigInChiusura;

        //public Base(Configs.SaveLocation saveLocation = null) {
        public Base(object saveLocation = null)
        {
            this.saveLocation = saveLocation ?? App.Config;
            this.Initialized += Base_Initialized;
            this.Loaded += Base_Loaded;
            this.Closing += Base_Closing;

            salvaConfigInChiusura = true;

            inizMenuContestuale();
            this.ContextMenu = cntMenuPrinc;
        }

        #region "eventi wnd"

        private void Base_Initialized(object sender, EventArgs e) {
            config = new ConfigWnd(this.GetType().Name.ParoleMinuMaiu(Str.MinMai.minu, soloPrimaParola: true));
            config = (ConfigWnd)config.Load(Main.App.Config, out _); //TODO da modificare 
            //config = (ConfigWnd)Savable.Load(config);
            ApplicaImpostazioni();
        }

        private void Base_Loaded(object sender, RoutedEventArgs e) { }

        private void Base_Closing(object sender, CancelEventArgs e) {
            if (salvaConfigInChiusura == true) SalvaImpostazioni();
        }

        #endregion


        private void inizMenuContestuale() {
            cntMenuPrinc = new ContextMenu();

            MenuItem voceMenu = new MenuItem();
            voceMenu.Header = "Impostazioni";

            cntMenuPrinc.Items.Add(voceMenu);
            //cntMenuPrinc.Items.Add()
        }

        private bool ApplicaImpostazioni() {

            //ConfigWnd parentConfig = (ConfigWnd)config.SavableParent;
            ConfigWnd parentConfig = null; //TODO

            if (this.Content == null) {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "Me.Content è nothing, aggiungere il figlio alla window tramite la prop. .content"));
                return false;
            }

            this.Left = config.posizioneIniziale.X;
            this.Top = config.posizioneIniziale.Y;
            this.Width = config.dimensioni.Width;
            this.Height = config.dimensioni.Height;

            if (config.zoomDefault == true && (parentConfig != null)) {
                (this.Content as FrameworkElement).LayoutTransform = new ScaleTransform(parentConfig.zoom, parentConfig.zoom);
            } else {
                (this.Content as FrameworkElement).LayoutTransform = new ScaleTransform(config.zoom, config.zoom);
            }

            if (config.coloreSfondoDefault == true && parentConfig != null) {
                this.Background = parentConfig.coloreSfondo;
            } else {
                this.Background = config.coloreSfondo;
            }

            return true;
        }

        private bool SalvaImpostazioni() {

            if (this.Content == null) {
                Log.main.Add(new Mess(Tipi.ERR, Log.main.errUserText, "Me.Content è nothing"));
                return false;
            }

            config.posizioneIniziale = new Point(this.Left, this.Top);
            config.dimensioni = new Size(this.ActualWidth, this.ActualHeight);

            ScaleTransform scaler = (ScaleTransform)(this.Content as FrameworkElement).LayoutTransform;
            config.zoom = scaler.ScaleX;

            config.coloreSfondo = this.Background;
            if (config.Save(Main.App.Config)  == false) return false; //TODO da modificare

            return true;
        }

        public void RiportaInAreaVisibile() {
            //schermo.WorkingArea: il valore restituito tiene conto del fatto che l'area della barra delle applicazione non è di lavoro(al contrario della prop Bounds)
            //schermo.Bounds: da la risoluzione del monitor indipendentemente dall'area di lavoro

            List<Point> deltaPerSchermo = new List<Point>(); Point tmpDelta, deltaFinale; Int32 sommaDelta, tmpX, tmpY; bool ciclo1;
            sommaDelta = 0;
            deltaFinale = default(Point);

            //Rilevo i delta da applicare per riportare in area visibile in ogni monitor
            foreach (var schermo in System.Windows.Forms.Screen.AllScreens) {
                tmpDelta = new Point();

                //***tmpX per margine sinistro, tmpY per margine superiore
                tmpX = Convert.ToInt32(schermo.WorkingArea.Left - this.Left);
                tmpY = Convert.ToInt32(schermo.WorkingArea.Top - this.Top);

                if (tmpX > 0) tmpDelta.X = tmpX;
                if (tmpY > 0) tmpDelta.Y = tmpY;


                //***tmpX per margine destro, tmpY per margine inferiore
                tmpX = Convert.ToInt32((schermo.WorkingArea.Left + schermo.WorkingArea.Width) - (this.Left + this.Width));
                tmpY = Convert.ToInt32((schermo.WorkingArea.Top + schermo.WorkingArea.Height) - (this.Top + this.Height));


                //***Prendo il maggior spostamento
                if (tmpX < 0 && Math.Abs(tmpX) > tmpDelta.X) tmpDelta.X = tmpX;
                if (tmpY < 0 && Math.Abs(tmpY) > tmpDelta.Y) tmpDelta.Y = tmpY;


                //***Se le dimensioni della finestra superano quelle dello schermo comunque non devo spostare la finestra oltre il lato sinistro del monitor
                if ((this.Left + tmpDelta.X) < schermo.WorkingArea.Left) tmpDelta.X = schermo.WorkingArea.Left - this.Left;
                //***************************************************************************************************

                deltaPerSchermo.Add(tmpDelta);
            }

            //***Prendo il delta più basso di tutti i monitor 
            ciclo1 = true;
            foreach (Point deltaSchermo in deltaPerSchermo) {

                if (ciclo1 == true) { //Primo ciclo
                    ciclo1 = false;
                    deltaFinale = deltaSchermo;
                    sommaDelta = Convert.ToInt32(Math.Abs(deltaSchermo.X) + Math.Abs(deltaSchermo.Y));
                } else {
                    if (Math.Abs(deltaSchermo.X) + Math.Abs(deltaSchermo.Y) < sommaDelta) {
                        deltaFinale = deltaSchermo;
                        sommaDelta = Convert.ToInt32(Math.Abs(deltaSchermo.X) + Math.Abs(deltaSchermo.Y));
                    }
                }

            }

            if (deltaPerSchermo.Count == 0) { //Nessuno monitor disponibile
                Log.main.Add(new Mess(Tipi.Warn, "Nessuno schermo disponibile"));
                return;
            }
            this.Left += deltaFinale.X;
            this.Top += deltaFinale.Y;
        }

        [Serializable] private class ConfigWnd : Configs.ConfigBase //Ogni classe che eredita da config deve avere un nome univoco in tutta l'app poichè viene salvato
        {
            public Point posizioneIniziale;
            public Size dimensioni;
            public Double zoom;
            public Brush coloreSfondo;
            public bool zoomDefault, coloreSfondoDefault;
            [JsonConstructor] public ConfigWnd(string savableName) : base(savableName) { //JsonConstructor: I parametri obbligatori devono avere lo stesso nome delle variabili/proprietà
                posizioneIniziale = new Point(0, 0);
                dimensioni = new Size(300, 300);
                zoomDefault = true;
                zoom = 1.2;
                coloreSfondoDefault = true;
                coloreSfondo = Brushes.White;
            }
        }
    }
}
