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
using Main.Schedulers;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for SrcAdvancedBackupV.xaml
    /// </summary>
    public partial class SrcAdvancedBackupV : UserControl
    {
        //AdvancedBackupVM advancedBackupVM;

        public SrcAdvancedBackupV()
        {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
        

        private void BtnModifyScheduler_Click(object sender, RoutedEventArgs e)
        {
            ((AdvancedBackupVM)DataContext).ShowScheduler();
        }

        private void BtnFilesSelector_Click(object sender, RoutedEventArgs e)
        {
            ((AdvancedBackupVM)DataContext).ShowFilesSelector();
        }
    }
}
