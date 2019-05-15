using System;
using System.Windows.Controls;

namespace Main.Logs2
{
    internal class WndMessaggi : Wnds.Base
    {
        internal WndMessaggi(UserControl uscMessaggi)
        {
            this.AddChild(uscMessaggi);
        }
    }
}
