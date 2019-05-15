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
    /// Interaction logic for SrcBackupBaseFooter.xaml
    /// </summary>
    public partial class SrcBackupBaseFooter : UserControl
    {
        public SrcBackupBaseFooter()
        {
            InitializeComponent();
        }

        private void TxtNameValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TxtNameValue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) //e.ClickCount > 1 && 
            {
                BackupBaseVM backupBaseVm = (BackupBaseVM)this.DataContext;
                backupBaseVm.SetDestinationFolder();
            }
        }

        private void BtnFilesSelector_Click(object sender, RoutedEventArgs e)
        {
            ((BackupBaseVM)DataContext).ShowFilesSelector();
        }
    }
}
