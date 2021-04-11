using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backupper

{
    public class SettingsVM : INotifyPropertyChanged //Singleton class
    {
        private static readonly Lazy<SettingsVM> instance = new Lazy<SettingsVM>(() => new SettingsVM()); //Thread-safe singleton

        public static SettingsVM Instance
        {
            get
            {
                return instance.Value;
            }
        }
        public SettingsM SettingsM { get { return SettingsM.Instance; } }


        private SettingsVM()
        { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
