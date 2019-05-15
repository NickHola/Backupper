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
    /// Interaction logic for ProgressBarFlat.xaml
    /// </summary>
    public partial class ProgressBarFlat : ProgressBarBase
    {

        public SolidColorBrush ColoreBarreVerticali
        {
            get { return (SolidColorBrush)this.GetValue(ColoreBarreVerticaliProperty); }
            set { this.SetValue(ColoreBarreVerticaliProperty, value); }
        }
        public static readonly DependencyProperty ColoreBarreVerticaliProperty = DependencyProperty.Register("ColoreBarreVerticali", typeof(SolidColorBrush), typeof(ProgressBarFlat), new PropertyMetadata(Util.DammiBrushDaEsadec("BFEEFF")));

        public ProgressBarFlat()
        {
            this.Initialized += new EventHandler(MyInitialized);
            InitializeComponent();
        }

        private void MyInitialized(object sender, EventArgs e)
        {
        }
    }
}
