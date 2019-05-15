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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Main;
using Main.Controls;
using static Main.Util;

namespace Main.Schedulers
{
    /// <summary>
    /// Interaction logic for SrcScheduler.xaml
    /// </summary>
    public partial class SrcSchedulerV : UserControl
    {
        SchedulerVM schedulerVM;

        public SrcSchedulerV()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            schedulerVM = (SchedulerVM)this.DataContext;
        }


        private void btnTestScheduler_Click(object sender, RoutedEventArgs e)
        { schedulerVM.TestScheduler(); }

        private void btnCloseTest_Click(object sender, RoutedEventArgs e)
        { schedulerVM.CloseTest(); }


    }
}
