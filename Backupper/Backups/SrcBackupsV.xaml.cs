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

namespace Backupper
{
    /// <summary>
    /// Interaction logic for SrcBackupsV.xaml
    /// </summary>
    public partial class SrcBackupsV : UserControl
    {
        public BackupsVM BackupsVM { get { return BackupsVM.Instance; } }
  
        public SrcBackupsV()
        {
            InitializeComponent();
        }

    }
}
