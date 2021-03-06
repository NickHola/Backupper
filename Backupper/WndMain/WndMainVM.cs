﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class WndMainVM : INotifyPropertyChanged
    {
        private static readonly Lazy<WndMainVM> instance = new Lazy<WndMainVM>(() => new WndMainVM()); //Thread-safe singleton
        object selectedContent;
        private bool showZoomLevel;

        [JsonIgnore]
        public static WndMainVM Instance
        {
            get
            {
                return instance.Value;
            }
        }

        [JsonIgnore]
        public object SelectedContent
        {
            get
            {
                return selectedContent;
            }
            set
            {
                selectedContent = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool ShowZoomLevel
        {
            get
            {
                return showZoomLevel;
            }
            set
            {
                showZoomLevel = value;
                OnPropertyChanged();
            }
        }

        public WndMainM WndMainM { get { return WndMainM.Instance; } }

        [JsonConstructor]
        private WndMainVM()
        {
            SelectedContent = BackupsVM.Instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
