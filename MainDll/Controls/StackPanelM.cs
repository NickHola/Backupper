using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Controls
{
    //***ATTENZIONE non si può fare lo userControl che eredita da StackPanel poichè se all'interno si inseriscono altri ogg. con la prop. x:Name (es. <mainCtrl:ButtonM x:Name="btnTestRegex") si ha seguente errore:
    //Error Cannot set Name attribute value 'btnTestRegex' on element 'ButtonM'. 'ButtonM' is under the scope of element 'StackPanelM', which already had a name registered when it was defined in another scope.
    //Non ho ben capito il perchè, poiche se si fa uno userControl che eredita da userControl come GroupBoxCheck e lo si usa come contenitore di altri elemente che hanno x:Name non si ha nessun errore
    //***************************************************************************************************************************************************************************************************************
    public class StackPanelM : StackPanel
    {
        #region "DependencyProperty definition"
        public VerticalAlignment ChildVerticalAlignment
        {
            get { return (VerticalAlignment)this.GetValue(ChildVerticalAlignmentProperty); }
            set { this.SetValue(ChildVerticalAlignmentProperty, value); }
        }
        public static readonly DependencyProperty ChildVerticalAlignmentProperty = DependencyProperty.Register("ChildVerticalAlignment", typeof(VerticalAlignment), typeof(StackPanelM), new PropertyMetadata(VerticalAlignment.Center));

        public HorizontalAlignment ChildHorizontalAlignment
        {
            get { return (HorizontalAlignment)this.GetValue(ChildHorizontalAlignmentProperty); }
            set { this.SetValue(ChildHorizontalAlignmentProperty, value); }
        }
        public static readonly DependencyProperty ChildHorizontalAlignmentProperty = DependencyProperty.Register("ChildHorizontalAlignment", typeof(HorizontalAlignment), typeof(StackPanelM), new PropertyMetadata(HorizontalAlignment.Center));

        //public bool ChildMarginIsOn
        //{
        //    get { return (bool)this.GetValue(ChildMarginIsOnProperty); }
        //    set { this.SetValue(ChildMarginIsOnProperty, value); }
        //}
        //public static readonly DependencyProperty ChildMarginIsOnProperty = DependencyProperty.Register("ChildMarginIsOn", typeof(bool), typeof(StackPanelM), new PropertyMetadata(false));

        public Thickness ChildMargin
        {
            get { return (Thickness)this.GetValue(ChildMarginProperty); }
            set { this.SetValue(ChildMarginProperty, value); }
        }
        public static readonly DependencyProperty ChildMarginProperty = DependencyProperty.Register("ChildMargin", typeof(Thickness), typeof(StackPanelM), new PropertyMetadata(null));

        #endregion

        public StackPanelM()
        {

        }

        protected override void OnVisualChildrenChanged(DependencyObject objAdded, DependencyObject objRemoved)
        {

            FrameworkElement child = null; Style stile = null;

            if (objAdded != null && objAdded.GetType().IsSubclassOf(typeof(FrameworkElement)))
            {
                child = (FrameworkElement)objAdded;

                stile = new Style(child.GetType(), child.Style);

                if (ChildVerticalAlignment != VerticalAlignment.Center) stile.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, ChildVerticalAlignment));
                if (ChildHorizontalAlignment != HorizontalAlignment.Center) stile.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, ChildHorizontalAlignment));

                if (ChildMargin != new Thickness(0)) stile.Setters.Add(new Setter(FrameworkElement.MarginProperty, ChildMargin));

            }

            if (objRemoved != null)
            {
            }

            if (stile != null && child != null) { child.Style = stile; }

            base.OnVisualChildrenChanged(objAdded, objRemoved);
        }
    }
}
