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

namespace Main.MsgBxes
{
    /// <summary>
    /// Interaction logic for WndCustomMsgBxV.xaml
    /// </summary>
    public partial class WndCustomMsgBxV : Window, INotifyPropertyChanged
    {
        WndCustomMsgBxVM wndCustomMsgBxVM;
        public WndCustomMsgBxVM WndCustomMsgBxVM
        {
            get { return wndCustomMsgBxVM; }
            set
            {
                wndCustomMsgBxVM = value;
                OnPropertyChanged();
            }
        }


        public WndCustomMsgBxV(WndCustomMsgBxVM wndMsgBxVM)
        {
            InitializeComponent();
            WndCustomMsgBxVM = wndMsgBxVM;
        }


        private void TxbAction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WndCustomMsgBxVM.SetResult((TextBlock)sender);
            this.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
