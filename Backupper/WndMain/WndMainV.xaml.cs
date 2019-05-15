using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Main;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for WndMainV.xaml
    /// </summary>
    public partial class WndMainV : Window
    {

        public WndMainVM WndMainVM { get { return WndMainVM.Instance; } }
       
      
        public WndMainV()
        {
            InitializeComponent();

            //SrcMsgBxV.SrcMsgBxVM.
        }

        private void BtnBackups_Click(object sender, RoutedEventArgs e)
        {
            WndMainVM.SelectedContent = BackupsVM.Instance;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            WndMainVM.SelectedContent = SettingsVM.Instance;
        }

        private void BtnDefaultZoomLevel_Click(object sender, RoutedEventArgs e)
        {
            WndMainVM.WndMainM.ZoomLevel = 1.3;
        }

        private void BtnZoom_Click(object sender, RoutedEventArgs e)
        {
            WndMainVM.ShowZoomLevel = !WndMainVM.ShowZoomLevel;
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            WndMainVM.SelectedContent = InfoVM.Instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
