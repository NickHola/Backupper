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

namespace Main.FSes
{
    /// <summary>
    /// Interaction logic for SrcFilesSelector.xaml
    /// </summary>
    public partial class SrcFilesSelectorV : UserControl
    {
        public SrcFilesSelectorV()
        {
            InitializeComponent();
        }

     

        private void BtnSelectFolderOrFile_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private void DtgPaths_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridM dataGrid = (DataGridM)sender;
            DataGridColumn dtgClmPath = (DataGridColumn)dataGrid.FindName("txtPath");
            if (object.Equals(dataGrid.CurrentColumn, dtgClmPath) && e.ChangedButton == MouseButton.Left)
            {
                if (dataGrid.SelectedItem.GetType() == typeof(PathWithFilters)) 
                    ((FilesSelectorVM)this.DataContext).FilesSelectorM.SetSelectedPath((PathWithFilters)dataGrid.SelectedItem);
            }

        }

        private void btnTestPaths_Click(object sender, RoutedEventArgs e)
        {
            ((FilesSelectorVM)this.DataContext).TestPaths();
        }

        private void btnTestPath_Click(object sender, RoutedEventArgs e)
        {
            ((FilesSelectorVM)this.DataContext).TestPath();
        }

        private void btnCloseTest_Click(object sender, RoutedEventArgs e)
        {
            ((FilesSelectorVM)this.DataContext).CloseTest();
        }
    }
}
