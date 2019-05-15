using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Main.Controls.Old
{
    public class MenuButtonOld : Button
    {
        public bool Selected
        {
            get { return (bool)this.GetValue(SelectedProperty); }
            set { this.SetValue(SelectedProperty, value); }
        }
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(MenuButtonOld), new PropertyMetadata(false));

        public SolidColorBrush ForegroundSelectedColor
        {
            get { return (SolidColorBrush)this.GetValue(ForegroundSelectedColorProperty); }
            set { this.SetValue(ForegroundSelectedColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundSelectedColorProperty = DependencyProperty.Register("ForegroundSelectedColor", typeof(SolidColorBrush), typeof(MenuButtonOld), new PropertyMetadata(Util.DammiBrushDaEsadec("8BCFE0")));


        protected override void OnClick()
        {
            RimuoviSelectedAltriBtn();
            Selected = true;
            base.OnClick();
        }

        public MenuButtonOld()
        {
            this.Style = (Style)App.UIResource["stlBtnMenu"];
        }

        private void RimuoviSelectedAltriBtn()
        {
            List<UIElement> figli = Control.DammiFigli(this.Parent, tipoDaCercare: typeof(MenuButtonOld));

            foreach (MenuButtonOld figlio in figli)
            {
                if (figlio.Equals(figlio)) continue;
                figlio.Selected = false;
            }
        }
    }
}
