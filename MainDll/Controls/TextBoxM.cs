using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace Main.Controls
{
    public class TextBoxM : TextBox
    {
        private Style defaultStyle = (Style)App.UIResource["stlTxtMain"];

        #region "DependencyProperty definition"
        public Style AddStyle
        {
            get { return (Style)this.GetValue(AddStyleProperty); }
            set { this.SetValue(AddStyleProperty, value); }
        }
        public static readonly DependencyProperty AddStyleProperty = DependencyProperty.Register("AddStyle", typeof(Style), typeof(TextBoxM), new PropertyMetadata(null));

        public Style AddStyle2
        {
            get { return (Style)this.GetValue(AddStyleProperty2); }
            set { this.SetValue(AddStyleProperty2, value); }
        }
        public static readonly DependencyProperty AddStyleProperty2 = DependencyProperty.Register("AddStyle2", typeof(Style), typeof(TextBoxM), new PropertyMetadata(null));

        public SolidColorBrush ColoreSfondoPopupErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreSfondoPopupErrProperty); }
            set { this.SetValue(ColoreSfondoPopupErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreSfondoPopupErrProperty = DependencyProperty.Register("ColoreSfondoPopupErr", typeof(SolidColorBrush), typeof(TextBoxM), new PropertyMetadata(Util.DammiBrushDaEsadec("E52914")));

        public SolidColorBrush ColoreScrittaPopupErr
        {
            get { return (SolidColorBrush)this.GetValue(ColoreScrittaPopupErrProperty); }
            set { this.SetValue(ColoreScrittaPopupErrProperty, value); }
        }
        public static readonly DependencyProperty ColoreScrittaPopupErrProperty = DependencyProperty.Register("ColoreScrittaPopupErr", typeof(SolidColorBrush), typeof(TextBoxM), new PropertyMetadata(Brushes.White));
        #endregion

        public TextBoxM()
        {
            this.Initialized += new EventHandler(MyInitialized);
            //this.Loaded += new RoutedEventHandler(MyLoaded);
            if (Util.IsDesignTime == true) this.Style = defaultStyle;
        }


        private void MyInitialized(object sender, EventArgs e)
        {
            if (Util.IsDesignTime == true) return;
            this.Style = Util.AddStylesToAnExistingOne(this.Style, new Style[] { AddStyle, AddStyle2 }, this.GetType(), defaultStyle);
        }


        //private void MyLoaded(object sender, RoutedEventArgs e)
        //{
        //    if (AddStyle != null )
        //        this.Style = Util.AggiungiStiliAdEsistente(this.Style, new Style[] { AddStyle });
        //}

    }

}
