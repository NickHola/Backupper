using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Main.MsgBxes
{
    public class WndCustomMsgBxVM
    {
        string title, text;
        MsgBxPicture picture;
        BindingList<string> buttonSet;
        //SolidColorBrush onMouseOverColor = new SolidColorBrush(Color.FromArgb(255, 140, 212, 255));
        //bool isOnDisplay, setOpacity0Completed;



        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        public MsgBxPicture Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                OnPropertyChanged();
            }
        }
        public BindingList<string> ButtonSet
        {
            get { return buttonSet; }
            set
            {
                buttonSet = value;
                OnPropertyChanged();
            }
        }

        public string MsgBxResult { get; private set; }


        public WndCustomMsgBxVM(string title, string text, MsgBxPicture picture, BindingList<string> buttonSet)
        {
            Title = title;
            Text = text;
            Picture = picture;
            ButtonSet = buttonSet;
        }


        public void SetResult(TextBlock txbAction)
        {
            MsgBxResult = txbAction.Text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
