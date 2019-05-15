using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Main.Controls
{
    public class GroupBoxCheck : GroupBox
    {
        Style defaultStyle = (Style)App.UIResource["stlGpbChkMain"];
        System.Windows.Shapes.Rectangle rctOverText;
        bool loaded = false;

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GroupBoxCheck), new PropertyMetadata(""));

        public bool GpbIsEnabled
        {
            get { return (bool)this.GetValue(GpbIsEnabledProperty); }
            set { this.SetValue(GpbIsEnabledProperty, value); }
        }
        public static readonly DependencyProperty GpbIsEnabledProperty = DependencyProperty.Register("GpbIsEnabled", typeof(bool), typeof(GroupBoxCheck), new PropertyMetadata(true));


        public GroupBoxCheck()
        {
            this.Initialized += new EventHandler(MyInitialized);
            this.Loaded += new RoutedEventHandler(MyLoaded);
            if (Util.IsDesignTime == true) this.Style = defaultStyle;
            

            //var dpd = DependencyPropertyDescriptor.FromProperty(GroupBoxCheck.TextProperty, typeof(GroupBoxCheck));
            //if (dpd != null) { dpd.AddValueChanged(this, new EventHandler(TextPropertyChanged)); }
        }


        private void MyInitialized(object sender, EventArgs e)
        {
            if (Util.IsDesignTime == true) return;
            this.Style = Util.AddStylesToAnExistingOne(this.Style, new Style[] { }, this.GetType(), defaultStyle);
            Text = Text; //Per aggiungere gli spazi
        }


        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            if (loaded == true) return;
            //if (Util.IsDesignTime == true) this.Text = this.Text; //Forza la visualizzazione corretta della scritta per far posto alla checkbox. 
            try
            {
                rctOverText = (System.Windows.Shapes.Rectangle)this.Template.FindName("rctOverText", this);
                rctOverText.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(rctMouseHand_MouseLeftButtonUp);
            }
            finally
            { loaded = true; }
        }

        private void rctMouseHand_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GpbIsEnabled = !GpbIsEnabled;
            string name = this.Name;
        }
    }
}
