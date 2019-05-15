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
        static AddNewDeviceVM instance;
        private AddNewDeviceM addNewDeviceM;
        bool isSelectionNewDevice;

        public AddNewDeviceM AddNewDeviceM
        {
            get { return addNewDeviceM; }
            set
            {
                addNewDeviceM = value;
                OnPropertyChanged();
            }
        }

        public static AddNewDeviceVM Instance
        {
            get
            {
                if (instance == null)
                    instance = new AddNewDeviceVM();
                return instance;
            }
        }

        public bool IsSelectionNewDevice
        {
            get { return isSelectionNewDevice; }
            set
            {
                isSelectionNewDevice = value;
                OnPropertyChanged();
            }
        }

        private AddNewDeviceVM()
        {
            AddNewDeviceM = new AddNewDeviceM();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
