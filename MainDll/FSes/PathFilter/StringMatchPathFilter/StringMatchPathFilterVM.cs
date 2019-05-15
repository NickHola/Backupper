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
    public class StringMatchPathFilterVM : INotifyPropertyChanged
    {
        SortBindList<StringMatchPathFilterM> stringMatchPathFilterM;
        public SortBindList<StringMatchPathFilterM> StringMatchPathFilterM
        {
            get
            { return stringMatchPathFilterM; }
            set
            {
                stringMatchPathFilterM = value;
                OnPropertyChanged();
            }
        }

        public StringMatchPathFilterVM(SortBindList<StringMatchPathFilterM> stringMatchPathFiltersM)
        {
            StringMatchPathFilterM = stringMatchPathFiltersM;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
