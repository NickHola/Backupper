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
    /// Interaction logic for TextBlockM.xaml
    /// </summary>
    public partial class TextBlockM : TextBlock
    {
        public Style AddStyle
        {
            get { return (Style)this.GetValue(AddStyleProperty); }
            set { this.SetValue(AddStyleProperty, value); }
        }
        public static readonly DependencyProperty AddStyleProperty = DependencyProperty.Register("AddStyle", typeof(Style), typeof(TextBlockM), new PropertyMetadata(null));

        public Style AddStyle2
        {
            get { return (Style)this.GetValue(AddStyle2Property); }
            set { this.SetValue(AddStyle2Property, value); }
        }
        public static readonly DependencyProperty AddStyle2Property = DependencyProperty.Register("AddStyle2", typeof(Style), typeof(TextBlockM), new PropertyMetadata(null));

        private Style defaultStyle = (Style)App.UIResource["stlTxbMain"];

        public TextBlockM()
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


        //private void MyLoaded(Object sender, RoutedEventArgs e)
        //{  }
    }
}
