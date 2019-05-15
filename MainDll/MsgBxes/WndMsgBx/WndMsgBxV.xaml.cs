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
    /// Interaction logic for WndMsgBxV.xaml
    /// </summary>
    public partial class WndMsgBxV : Window, INotifyPropertyChanged
    {
        WndMsgBxVM wndMsgBxVM;
        public WndMsgBxVM WndMsgBxVM
        {
            get { return wndMsgBxVM; }
            set
            {
                wndMsgBxVM = value;
                OnPropertyChanged();
            }
        }


        public WndMsgBxV(WndMsgBxVM wndMsgBxVM)
        {
            InitializeComponent();
            WndMsgBxVM = wndMsgBxVM;
        }


        private void TxbAction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WndMsgBxVM.SetResult((TextBlock)sender);
            this.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
