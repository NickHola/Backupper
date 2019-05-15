using Main.Binds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.FSes
{
    class RegexPathFilterVM : INotifyPropertyChanged
    {
        SortBindList<RegexPathFilterM> regexPathFilterM;
        public SortBindList<RegexPathFilterM> RegexPathFilterM
        {
            get
            { return regexPathFilterM; }
            set
            {
                regexPathFilterM = value;
                OnPropertyChanged();
            }
        }

        public  RegexPathFilterVM(SortBindList<RegexPathFilterM> regexPathFilterM)
        {
            RegexPathFilterM = regexPathFilterM;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }


}
