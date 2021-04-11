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
using Main.Controls;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for SrcAddNewDeviceV.xaml
    /// </summary>
    public partial class SrcAddNewDeviceV : UserControl
    {
        //public AddNewDeviceVM AddNewDeviceVM { get { return AddNewDeviceVM.Instance; } }
               
        public SrcAddNewDeviceV()
        {
            InitializeComponent();
        }

        private void TxbAddNew_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((AddNewDeviceVM)this.DataContext).IsSelectionNewDevice = true;
        }

        private void BtnBackupsType_Click(object sender, RoutedEventArgs e)
        {
            ButtonM button = (ButtonM)sender;
            ((AddNewDeviceVM)this.DataContext).Model.AddNewBackupAtList((string)button.Content);
            ((AddNewDeviceVM)this.DataContext).IsSelectionNewDevice = false;
        }

        private void BtnAbortSelection_Click(object sender, RoutedEventArgs e)
        {
            ((AddNewDeviceVM)this.DataContext).IsSelectionNewDevice = false;
        }
    }
}
