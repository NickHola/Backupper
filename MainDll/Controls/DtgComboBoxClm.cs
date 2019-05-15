using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Controls
{
    public class DtgComboBoxClm : DataGridComboBoxColumn
    {
        public DtgComboBoxClm()
        { }
              

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            ComboBox comboBox = (ComboBox)editingElement;
            //comboBox.DropDownClosed -= ComboBox_DropDownClosed; //Sembra che non serve
            comboBox.DropDownClosed += ComboBox_DropDownClosed;
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }


        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            List<UIElement> padre = Controls.Control.DammiPadri(comboBox, typeof(DataGridM));
            if (padre.Count() == 0) return;
            DataGridM dataGrid = (DataGridM)padre[0];

            while (dataGrid.IsInEdit)
                dataGrid.CommitEdit();
        }

    }
}
