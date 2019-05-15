using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Main.Controls.Old
{
    public class ProgressBarRingOld : ProgressBarBase
    {
        public Visibility PercVisibility
        {
            get { return (Visibility)this.GetValue(PercVisibilityProperty); }
            set { this.SetValue(PercVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PercVisibilityProperty = DependencyProperty.Register("PercVisibility", typeof(Visibility), typeof(ProgressBarRingOld), new PropertyMetadata(Visibility.Visible));
        
        public ProgressBarRingOld() {
            this.Style = (Style)App.UIResource["stlPrgRing"];
            this.Initialized += new EventHandler(MyInitialized);
        }

        private void MyInitialized(object sender, EventArgs e) {
            if (PercVisibility == Visibility.Visible) this.Foreground = new SolidColorBrush(this.Colore0Perc); //Va qui e non sulla new, poichè sulla new ancora non ho letto il valore della proprietà assegnata nello xaml
        }
    }
}
