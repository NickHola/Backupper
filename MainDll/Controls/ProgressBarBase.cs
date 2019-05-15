using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Main.Controls
{
    public class ProgressBarBase : System.Windows.Controls.UserControl //Non posso rendere la classe abstract altrimenti chi eredita non vede l'attributo Name che uso in xaml tipo: x:Name="uscMain"
    {
        protected Dictionary<Double, Color> passiColore = new Dictionary<Double, Color>();
        private static readonly SolidColorBrush defaultColor = Util.DammiBrushDaEsadec("008CFF");


        public SolidColorBrush DefaultColor
        {
            get { return defaultColor; }
        }


        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBarBase), new PropertyMetadata(Convert.ToDouble(0)));

        public string Testo
        {
            get { return (string)this.GetValue(TestoProperty); }
            set { this.SetValue(TestoProperty, value); }
        }
        public static readonly DependencyProperty TestoProperty = DependencyProperty.Register("Testo", typeof(string), typeof(ProgressBarBase), new PropertyMetadata(""));


        public Color Colore0Perc
        {
            get { return (Color)this.GetValue(Colore0PercProperty); }
            set { this.SetValue(Colore0PercProperty, value); }
        }
        public static readonly DependencyProperty Colore0PercProperty = DependencyProperty.Register("Colore0Perc", typeof(Color), typeof(ProgressBarBase), new PropertyMetadata(defaultColor.Color));

        public Color Colore100Perc
        {
            get { return (Color)this.GetValue(CColore100PercProperty); }
            set { this.SetValue(CColore100PercProperty, value); }
        }
        public static readonly DependencyProperty CColore100PercProperty = DependencyProperty.Register("Colore100Perc", typeof(Color), typeof(ProgressBarBase), new PropertyMetadata(defaultColor.Color));

        public ProgressBarBase()
        {
            //this.Initialized += new EventHandler(MyInitialized);
            this.Loaded += new RoutedEventHandler(MyLoaded);
        }

        private void MyInitialized(object sender, EventArgs e)
        {
            //Anim.ProgressColorAnimationPerPerc(Colore0Perc, Colore100Perc, ref passiColore);
        }

        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            if (Colore0Perc != DefaultColor.Color || Colore100Perc != DefaultColor.Color)
            {
                Anim.ProgressColorAnimationPerPerc(Colore0Perc, Colore100Perc, ref passiColore);
                UpdateForegroudColor(TimeSpan.FromMilliseconds(1)); //Aggiornamento rapido
            }
            this.TargetUpdated += MyTargetUpdated;
        }

        private void MyTargetUpdated(object sender, DataTransferEventArgs e) //Serve perchè quando aggiorno il value non passa per il set della property Value sopra dichiarata
        {
            //if (value < 0) value = 0;
            //if (value > 100) value = 100;
            //value = Math.Truncate(10 * value) / 10; //Serve poichè per es. se il valore che mi arriva è 99.96, lo xaml a cui ho detto di arrotondare a 1 cifra dec., visualizzerebbe 100%, invece voglio visualizzare 99.9 
            if (e.Property == ValueProperty) UpdateForegroudColor();
        }

        private void UpdateForegroudColor(TimeSpan time = default)
        {
            if (passiColore.Count == 0) return;
            this.Foreground = Anim.AnimaPropDaPassiColore(passiColore, this.Value, (SolidColorBrush)this.Foreground, tempo: time);
        }
    }
}
