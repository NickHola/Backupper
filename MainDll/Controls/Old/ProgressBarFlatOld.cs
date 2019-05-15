using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Main.Controls.Old
{
    public class ProgressBarFlatOld : ProgressBarBase
    {
        public SolidColorBrush ColoreBarreVerticali
        {
            get { return (SolidColorBrush)this.GetValue(ColoreBarreVerticaliProperty); }
            set { this.SetValue(ColoreBarreVerticaliProperty, value); }
        }
        public static readonly DependencyProperty ColoreBarreVerticaliProperty = DependencyProperty.Register("ColoreBarreVerticali", typeof(SolidColorBrush), typeof(ProgressBarFlat), new PropertyMetadata(Util.DammiBrushDaEsadec("BFEEFF")));

        public ProgressBarFlatOld() {
            this.Style = (Style)App.UIResource["stlPrgFlat"];
            this.Initialized += new EventHandler(MyInitialized);
        }

        private void MyInitialized(object sender, EventArgs e) {
            this.Foreground = new SolidColorBrush(this.Colore0Perc); //Va qui e non sulla new, poichè sulla new ancora non ho letto il valore della proprietà assegnata nello xaml
        }
    }
}
