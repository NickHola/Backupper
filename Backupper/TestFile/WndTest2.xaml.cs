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
using System.Windows.Shapes;
using Main.MsgBxes;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for WndTest2.xaml
    /// </summary>
    public partial class WndTest2 : Window
    {
        public WndTest2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MsgBx.Show("", "Eccolooom", MsgBxPicture.Info, MsgBxButtonSet.YesNo) == MsgBxButton.Yes)
            {
                MessageBox.Show("yes");
            }
            else
            {
                MessageBox.Show("no");
            };
        }
    }
}
