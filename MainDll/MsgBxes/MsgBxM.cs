using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Main.MsgBxes
{
    public class MsgBx
    {


        public static MsgBxButton Show(string title, string text, MsgBxPicture picture, MsgBxButtonSet button = MsgBxButtonSet.Ok)
        {
            WndMsgBxVM WndMsgBxVM = new WndMsgBxVM(title, text, picture, button);
            WndMsgBxV wndMsgBxV = new WndMsgBxV(WndMsgBxVM);
            wndMsgBxV.Topmost = true;
            wndMsgBxV.WindowStyle = System.Windows.WindowStyle.None;
            wndMsgBxV.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wndMsgBxV.ShowDialog();

            return WndMsgBxVM.MsgBxResult;
        }

        public static String Show(string title, string text, MsgBxPicture picture, List<String> buttons)
        {
            BindingList<string> bindingButtons = new BindingList<string>(buttons);
            WndCustomMsgBxVM WndMsgBxVM = new WndCustomMsgBxVM(title, text, picture, bindingButtons);
            WndCustomMsgBxV wndMsgBxV = new WndCustomMsgBxV(WndMsgBxVM);
            wndMsgBxV.Topmost = true;
            wndMsgBxV.WindowStyle = System.Windows.WindowStyle.None;
            wndMsgBxV.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wndMsgBxV.ShowDialog();

            return WndMsgBxVM.MsgBxResult;
        }
    }
}
