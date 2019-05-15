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

namespace Main.Controls
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    public partial class MenuButton : Button
    {
        private Style defaultStyle = (Style)App.UIResource["stlBtnMenu"];

        #region "DependencyProperty definition"
        public bool Selected
        {
            get { return (bool)this.GetValue(SelectedProperty); }
            set { this.SetValue(SelectedProperty, value); }
        }
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));

        public SolidColorBrush ForegroundSelectedColor
        {
            get { return (SolidColorBrush)this.GetValue(ForegroundSelectedColorProperty); }
            set { this.SetValue(ForegroundSelectedColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundSelectedColorProperty = DependencyProperty.Register("ForegroundSelectedColor", typeof(SolidColorBrush), typeof(MenuButton), new PropertyMetadata(Util.DammiBrushDaEsadec("8BCFE0")));

        public bool IndipendentBehavior
        {
            get { return (bool)this.GetValue(IndipendentBehaviorProperty); }
            set { this.SetValue(IndipendentBehaviorProperty, value); }
        }
        public static readonly DependencyProperty IndipendentBehaviorProperty = DependencyProperty.Register("IndipendentBehavior", typeof(bool), typeof(MenuButton), new PropertyMetadata(false));


        #endregion

        public MenuButton()
        {
            //this.Initialized += new EventHandler(MyInitialized);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MyLoaded);
            if (Util.IsDesignTime == true) this.Style = defaultStyle;
        }

        //private void MyInitialized(object sender, EventArgs e)
        //{

        //}
        private void MyLoaded(Object sender, RoutedEventArgs e)
        {
            if (Util.IsDesignTime == true) return;
            this.Style = Util.AddStylesToAnExistingOne(this.Style, new Style[] { }, this.GetType(), defaultStyle);
        }

        protected override void OnClick()
        {
            if (IndipendentBehavior == false)
            {
                RimuoviSelectedAltriBtn();
                Selected = true;
            }
            base.OnClick();
        }

        private void RimuoviSelectedAltriBtn()
        {
            List<UIElement> figli = Control.DammiFigli(this.Parent, tipoDaCercare: typeof(MenuButton));

            foreach (MenuButton figlio in figli)
            {
                if (this.Equals(figlio)) continue;
                figlio.Selected = false;
            }
        }
    }
}
