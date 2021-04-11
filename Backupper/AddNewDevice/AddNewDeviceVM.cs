using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class AddNewDeviceVM : INotifyPropertyChanged
    {
        private static readonly Lazy<AddNewDeviceVM> instance = new Lazy<AddNewDeviceVM>(() => new AddNewDeviceVM()); //Thread-safe singleton
        private AddNewDeviceM model;
        bool isSelectionNewDevice;

        public AddNewDeviceM Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public static AddNewDeviceVM Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public bool IsSelectionNewDevice
        {
            get
            {
                return isSelectionNewDevice;
            }
            set
            {
                isSelectionNewDevice = value;
                OnPropertyChanged();
            }
        }

        private AddNewDeviceVM()
        {
            Model = new AddNewDeviceM();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
