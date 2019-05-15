using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Main.Validations.Validation;

namespace Backupper
{
    public class RESTsettingVM : INotifyPropertyChanged
    {
        RESTsettingM restSettingM;

        public RESTsettingM RESTsettingM
        {
            get { return restSettingM; }
            set
            {
                CtrlValue(value);
                restSettingM = value;
                OnPropertyChanged();
            }
        }

        public RESTsettingVM(RESTsettingM restSettingM) 
        {
            this.RESTsettingM = restSettingM;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
