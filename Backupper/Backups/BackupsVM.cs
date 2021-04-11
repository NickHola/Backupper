using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Main;
using Main.Logs;
using Main.Salvable;

namespace Backupper
{
    public class BackupsVM : INotifyPropertyChanged //Singleton class
    {
        private static readonly Lazy<BackupsVM> instance = new Lazy<BackupsVM>(() => new BackupsVM()); //Thread-safe singleton

        private object selectedSetting;
        private bool showSelectedSetting;
        List<AddNewDeviceVM> addNewDevice;

        public static BackupsVM Instance
        {
            get
            {
                return instance.Value;
            }
        }
        public BackupsM BackupsM { get { return BackupsM.Instance; } }
        public List<AddNewDeviceVM> AddNewDevice
        {
            get { return addNewDevice; }
            private set
            {
                addNewDevice = value;
                OnPropertyChanged();
            }
        } //For display SrcAddNewDeviceV in last place in ItemsControl.ItemsSource CompositeCollection

        public object SelectedSetting
        {
            get { return selectedSetting; }
            set
            {
                selectedSetting = value;
                OnPropertyChanged();
            }
        }
        public bool ShowSelectedSetting
        {
            get { return showSelectedSetting; }
            set
            {
                showSelectedSetting = value;
                OnPropertyChanged();
            }
        }

        private BackupsVM()
        {
            AddNewDevice = new List<AddNewDeviceVM>() { AddNewDeviceVM.Instance }; //Add one AddNewDeviceVM in list
        }

        public void ShowOrHideSelectedSetting(object setting)
        {
            if (setting.Equals(BackupsVM.Instance.SelectedSetting) == false || BackupsVM.Instance.ShowSelectedSetting == false)
            {
                BackupsVM.Instance.SelectedSetting = setting;
                BackupsVM.Instance.ShowSelectedSetting = true;
            }
            else
                BackupsVM.Instance.ShowSelectedSetting = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
