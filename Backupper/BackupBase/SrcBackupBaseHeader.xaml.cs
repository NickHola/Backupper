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
    /// Interaction logic for SrcBackupBaseHeader.xaml
    /// </summary>
    public partial class SrcBackupBaseHeader : UserControl
    {
        public SrcBackupBaseHeader()
        {
            InitializeComponent();
        }

        private void LblNameValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((BackupBaseVM)this.DataContext).IsViewInEdit = true;
        }

        private void TxtNameValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ((BackupBaseVM)this.DataContext).IsViewInEdit = false;
        }

        private void TxtNameValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ((BackupBaseVM)this.DataContext).IsViewInEdit = false;
        }

        private void TxtNameValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BackupBaseVM bckBaseVm = (BackupBaseVM)this.DataContext;
            bckBaseVm.IsViewInEdit = true;
            txtNameValue.SelectAll();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ((BackupBaseVM)this.DataContext).Backup.Delete();
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            BackupBaseVM backupBaseVm = (BackupBaseVM)this.DataContext;
            backupBaseVm.StartStopCompression();
        }
    }
}
