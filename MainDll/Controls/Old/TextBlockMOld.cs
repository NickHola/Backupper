using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Controls.Old
{
    public class TextBlockMOld : TextBlock
    {
    

        public TextBlockMOld() {
            this.Style = (Style)App.UIResource["stlTxbMain"];
        }

    }
}
