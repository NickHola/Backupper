using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main.Controls
{
    /// <summary>
    /// Interaction logic for ToggleButton2.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        #region "DependencyProperty definition"

        public bool IsChecked
        {
            get { return (bool)this.GetValue(IsCheckedProperty); }
            set { this.SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false));

        public string TextOff
        {
            get { return (string)this.GetValue(TextOffProperty); }
            set { this.SetValue(TextOffProperty, value); }
        }
        public static readonly DependencyProperty TextOffProperty = DependencyProperty.Register("TextOff", typeof(string), typeof(ToggleButton), new PropertyMetadata(""));

        public string TextOn
        {
            get { return (string)this.GetValue(TextOnProperty); }
            set { this.SetValue(TextOnProperty, value); }
        }
        public static readonly DependencyProperty TextOnProperty = DependencyProperty.Register("TextOn", typeof(string), typeof(ToggleButton), new PropertyMetadata(""));

        public SolidColorBrush ColoreStelo
        {
            get { return (SolidColorBrush)this.GetValue(ColoreSteloProperty); }
            set { this.SetValue(ColoreSteloProperty, value); }
        }
        public static readonly DependencyProperty ColoreSteloProperty = DependencyProperty.Register("ColoreStelo", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("FAFAFB")));

        public SolidColorBrush ColoreSteloOn
        {
            get { return (SolidColorBrush)this.GetValue(ColoreSteloOnProperty); }
            set { this.SetValue(ColoreSteloOnProperty, value); }
        }
        public static readonly DependencyProperty ColoreSteloOnProperty = DependencyProperty.Register("ColoreSteloOn", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("52D468")));

        public SolidColorBrush ColoreSteloDisabled
        {
            get { return (SolidColorBrush)this.GetValue(ColoreSteloDisabledProperty); }
            set { this.SetValue(ColoreSteloDisabledProperty, value); }
        }
        public static readonly DependencyProperty ColoreSteloDisabledProperty = DependencyProperty.Register("ColoreSteloDisabled", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Brushes.LightGray));

        public SolidColorBrush ColoreToggle
        {
            get { return (SolidColorBrush)this.GetValue(ColoreToggleProperty); }
            set { this.SetValue(ColoreToggleProperty, value); }
        }
        public static readonly DependencyProperty ColoreToggleProperty = DependencyProperty.Register("ColoreToggle", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Brushes.White));

        public SolidColorBrush ColoreToggleOn
        {
            get { return (SolidColorBrush)this.GetValue(ColoreToggleOnProperty); }
            set { this.SetValue(ColoreToggleOnProperty, value); }
        }
        public static readonly DependencyProperty ColoreToggleOnProperty = DependencyProperty.Register("ColoreToggleOn", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("F7FFF7")));

        public SolidColorBrush ColoreToggleDisabled
        {
            get { return (SolidColorBrush)this.GetValue(ColoreToggleDisabledProperty); }
            set { this.SetValue(ColoreToggleDisabledProperty, value); }
        }
        public static readonly DependencyProperty ColoreToggleDisabledProperty = DependencyProperty.Register("ColoreToggleDisabled", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("F4F4F4")));

        public SolidColorBrush ColoreBordo
        {
            get { return (SolidColorBrush)this.GetValue(ColoreBordoProperty); }
            set { this.SetValue(ColoreBordoProperty, value); }
        }
        public static readonly DependencyProperty ColoreBordoProperty = DependencyProperty.Register("ColoreBordo", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("EAEAEB")));

        public SolidColorBrush ColoreBordoOn
        {
            get { return (SolidColorBrush)this.GetValue(ColoreBordoOnProperty); }
            set { this.SetValue(ColoreBordoOnProperty, value); }
        }
        public static readonly DependencyProperty ColoreBordoOnProperty = DependencyProperty.Register("ColoreBordoOn", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("41C955")));

        public SolidColorBrush ColoreBordoDisabled
        {
            get { return (SolidColorBrush)this.GetValue(ColoreBordoDisabledProperty); }
            set { this.SetValue(ColoreBordoDisabledProperty, value); }
        }
        public static readonly DependencyProperty ColoreBordoDisabledProperty = DependencyProperty.Register("ColoreBordoDisabled", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("E5E5E5")));

        public SolidColorBrush ColoreTestoOn
        {
            get { return (SolidColorBrush)this.GetValue(ColoreTestoOnProperty); }
            set { this.SetValue(ColoreTestoOnProperty, value); }
        }
        public static readonly DependencyProperty ColoreTestoOnProperty = DependencyProperty.Register("ColoreTestoOn", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Util.DammiBrushDaEsadec("00AA00")));

        public SolidColorBrush ColoreTestoDisabled
        {
            get { return (SolidColorBrush)this.GetValue(ColoreTestoDisabledProperty); }
            set { this.SetValue(ColoreTestoDisabledProperty, value); }
        }
        public static readonly DependencyProperty ColoreTestoDisabledProperty = DependencyProperty.Register("ColoreTestoDisabled", typeof(SolidColorBrush), typeof(ToggleButton), new PropertyMetadata(Brushes.LightGray));

        public Style AddStyle
        {
            get { return (Style)this.GetValue(AddStyleProperty); }
            set { this.SetValue(AddStyleProperty, value); }
        }
        public static readonly DependencyProperty AddStyleProperty = DependencyProperty.Register("AddStyle", typeof(Style), typeof(ToggleButton), new PropertyMetadata(null));

        public bool PrimaTesto
        {
            get { return (bool)this.GetValue(PrimaTestoProperty); }
            set { this.SetValue(PrimaTestoProperty, value); }
        }
        public static readonly DependencyProperty PrimaTestoProperty = DependencyProperty.Register("PrimaTesto", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false));


        #endregion

        private bool loaded;

        public ToggleButton()
        {
            this.Initialized += new EventHandler(MyInitialized);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MyLoaded);
        }

        private void MyInitialized(object sender, EventArgs e)
        {

        }

        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            if (loaded == true)
                return;

            loaded = true;

            if (String.IsNullOrEmpty(TextOff) && !String.IsNullOrEmpty(TextOn))
                TextOff = TextOn;
            else if (String.IsNullOrEmpty(TextOn) && !String.IsNullOrEmpty(TextOff))
                TextOn = TextOff;

            if (Util.IsDesignTime == true) return;
            Style finalStyle = Util.AddStylesToAnExistingOne(this.Style, new Style[] { AddStyle }, this.GetType());
            this.Style = InizializzaAnimazioni(finalStyle); //non mettere lo style che ritorna in AggiungiStiliAdEsistente poichè serve effettuare il RegisterName che sta all'interno della CreaBloccoAnimazioni
        }

        private Style InizializzaAnimazioni(Style newStyle)
        {
            if (newStyle == null) newStyle = new Style();

            //***Animazione colore OFF
            ColorAnimation claColoreStelo = new ColorAnimation(ColoreStelo.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreStelo, new PropertyPath("ColoreStelo.Color"));

            ColorAnimation claColoreToggle = new ColorAnimation(ColoreToggle.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreToggle, new PropertyPath("ColoreToggle.Color"));

            ColorAnimation claColoreBordo = new ColorAnimation(ColoreBordo.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreBordo, new PropertyPath("ColoreBordo.Color"));

            Anim.CreaBloccoAnimazioni(newStyle, new List<Condition> { new Condition(IsCheckedProperty, false), new Condition(IsEnabledProperty, true) },
                                        new List<Timeline> { claColoreStelo, claColoreToggle, claColoreBordo });


            //***Animazione colore ON
            claColoreStelo = new ColorAnimation(ColoreSteloOn.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreStelo, new PropertyPath("ColoreStelo.Color"));

            claColoreToggle = new ColorAnimation(ColoreToggleOn.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreToggle, new PropertyPath("ColoreToggle.Color"));

            claColoreBordo = new ColorAnimation(ColoreBordoOn.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreBordo, new PropertyPath("ColoreBordo.Color"));

            ColorAnimation claColoreTesto = new ColorAnimation(ColoreTestoOn.Color, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTargetProperty(claColoreTesto, new PropertyPath("Foreground.Color"));

            Anim.CreaBloccoAnimazioni(newStyle, new List<Condition> { new Condition(IsCheckedProperty, true), new Condition(IsEnabledProperty, true) },
                                        new List<Timeline> { claColoreStelo, claColoreToggle, claColoreBordo, claColoreTesto });


            //***Animazione colore DISABLED
            claColoreStelo = new ColorAnimation(ColoreSteloDisabled.Color, TimeSpan.FromSeconds(0.1));
            Storyboard.SetTargetProperty(claColoreStelo, new PropertyPath("ColoreStelo.Color"));

            claColoreToggle = new ColorAnimation(ColoreToggleDisabled.Color, TimeSpan.FromSeconds(0.1));
            Storyboard.SetTargetProperty(claColoreToggle, new PropertyPath("ColoreToggle.Color"));

            claColoreBordo = new ColorAnimation(ColoreBordoDisabled.Color, TimeSpan.FromSeconds(0.1));
            Storyboard.SetTargetProperty(claColoreBordo, new PropertyPath("ColoreBordo.Color"));

            claColoreTesto = new ColorAnimation(ColoreTestoDisabled.Color, TimeSpan.FromSeconds(0.1));
            Storyboard.SetTargetProperty(claColoreTesto, new PropertyPath("Foreground.Color"));

            Anim.CreaBloccoAnimazioni(newStyle, new List<Condition> { new Condition(IsEnabledProperty, false) },
                                        new List<Timeline> { claColoreStelo, claColoreToggle, claColoreBordo, claColoreTesto });

            //Dim ellips As Ellipse = Me.Template.FindName("llpToggle", Me)
            //ellips.BeginStoryboard(stbScorri)

            //this.Style = stile;
            return newStyle;
        }

        private void ToggleButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
        }
    }
}
