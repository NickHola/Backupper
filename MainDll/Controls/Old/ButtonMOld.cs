using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Main.Controls.Old
{
    public class ButtonMOld : Button
    {
        public TextWrapping TextWrapping  //Useful when use stlBtnMainNoHighlith style, because this style use textBlock as Template and binding the textBlock property with the button property
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ButtonM), new PropertyMetadata(TextWrapping.Wrap));

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


        public SolidColorBrush DisabledForegroundColor
        {
            get { return (SolidColorBrush)this.GetValue(DisabledForegroundColorProperty); }
            set { this.SetValue(DisabledForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty DisabledForegroundColorProperty = DependencyProperty.Register("DisabledForegroundColor", typeof(SolidColorBrush), typeof(ButtonM), new PropertyMetadata(Util.DammiBrushDaEsadec("D3D3D3")));


        public ButtonMOld()
        {
            //this.Style = (Style)App.UIResource["stlBtnMain"];
            this.Initialized += new EventHandler(MyInitialized);
        }

        private void MyInitialized(object sender, EventArgs e)
        {
            this.Style = Util.AddStylesToAnExistingOne(this.Style, new Style[] { (Style)App.UIResource["stlBtnMain"], AddStyle, AddStyle2}, this.GetType());
        }
    }
}
