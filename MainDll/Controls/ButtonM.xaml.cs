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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main.Controls
{
    /// <summary>
    /// Interaction logic for ButtonM.xaml
    /// </summary>
    public partial class ButtonM : Button
    {
        public TextWrapping TextWrapping  //Useful when use stlBtnMainNoHighlith style, because this style use textBlock as Template and binding the textBlock property with the button property
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ButtonM), new PropertyMetadata(TextWrapping.Wrap));

        public SolidColorBrush MouseOverBackground
        {
            get { return (SolidColorBrush)this.GetValue(MouseOverBackgroundProperty); }
            set { this.SetValue(MouseOverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(SolidColorBrush), typeof(ButtonM), new PropertyMetadata(Util.DammiBrushDaEsadec("BEE6FD")));

        public Style AddStyle
        {
            get { return (Style)this.GetValue(AddStyleProperty); }
            set { this.SetValue(AddStyleProperty, value); }
        }
        public static readonly DependencyProperty AddStyleProperty = DependencyProperty.Register("AddStyle", typeof(Style), typeof(ButtonM), new PropertyMetadata(null));

        public Style AddStyle2
        {
            get { return (Style)this.GetValue(AddStyleProperty2); }
            set { this.SetValue(AddStyleProperty2, value); }
        }
        public static readonly DependencyProperty AddStyleProperty2 = DependencyProperty.Register("AddStyle2", typeof(Style), typeof(ButtonM), new PropertyMetadata(null));

        public SolidColorBrush DisabledBackground
        {
            get { return (SolidColorBrush)this.GetValue(DisabledBackgroundProperty); }
            set { this.SetValue(DisabledBackgroundProperty, value); }
        }
        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(SolidColorBrush), typeof(ButtonM), new PropertyMetadata(Util.DammiBrushDaEsadec("FCFCFC")));

        public SolidColorBrush DisabledForegroundColor
        {
            get { return (SolidColorBrush)this.GetValue(DisabledForegroundColorProperty); }
            set { this.SetValue(DisabledForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty DisabledForegroundColorProperty = DependencyProperty.Register("DisabledForegroundColor", typeof(SolidColorBrush), typeof(ButtonM), new PropertyMetadata(Util.DammiBrushDaEsadec("C1C1C1")));

        public SolidColorBrush DisabledBorderBrush
        {
            get { return (SolidColorBrush)this.GetValue(DisabledBorderBrushProperty); }
            set { this.SetValue(DisabledBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.Register("DisabledBorderBrush", typeof(SolidColorBrush), typeof(ButtonM), new PropertyMetadata(Util.DammiBrushDaEsadec("D3D3D3")));


        public ButtonM()
        {
            //this.Initialized += new EventHandler(MyInitialized);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MyLoaded);
            if (Util.IsDesignTime == true) this.Style = (Style)this.Resources["stlDefault"];
        }

        //private void MyInitialized(object sender, EventArgs e)
        //{ }


        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            if (Util.IsDesignTime == true) return;
            this.Style = Util.AddStylesToAnExistingOne(this.Style, new Style[] { (Style)this.Resources["stlDefault"], AddStyle, AddStyle2 }, this.GetType());
        }

    }
}
