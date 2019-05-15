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
    /// Interaction logic for ProgressBarRing.xaml
    /// </summary>
    public partial class ProgressBarRing : ProgressBarBase
    {
        public Visibility PercVisibility
        {
            get { return (Visibility)this.GetValue(PercVisibilityProperty); }
            set { this.SetValue(PercVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PercVisibilityProperty = DependencyProperty.Register("PercVisibility", typeof(Visibility), typeof(ProgressBarRing), new PropertyMetadata(Visibility.Visible));


        public ProgressBarRing()
        {
            this.Initialized += new EventHandler(MyInitialized);
            InitializeComponent();
        }

        private void MyInitialized(object sender, EventArgs e)
        {
            
        }

    }
}
