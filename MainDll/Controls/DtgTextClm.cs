using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Controls
{
    public class DtgTextClm : DataGridTextColumn
    {
        public string Format
        {
            get { return (string)this.GetValue(FormatProperty); }
            set { this.SetValue(FormatProperty, value); }
        }
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(DtgTextClm), new PropertyMetadata(""));


        public DtgTextClm()
        { }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs) {
            TextBox txtCell;

            txtCell = (TextBox)editingElement;
            txtCell.TextChanged += MyTextChanged;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }


        private void MyTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtCell = (TextBox)sender;
            try
            {
                txtCell.TextChanged -= MyTextChanged; //Poichè Formats.Format(..) cambia a sua volta la proprietà text
                Formats.Format(txtCell, Format);
            }
            finally
            {
                txtCell.TextChanged += MyTextChanged;
            }
        }
    }
}
