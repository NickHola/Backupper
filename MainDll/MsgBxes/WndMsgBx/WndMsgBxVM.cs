using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using Main.Logs;
using System.Windows.Controls;

namespace Main.MsgBxes
{
    public class WndMsgBxVM : INotifyPropertyChanged
    {
        string title, text;
        MsgBxPicture picture;
        MsgBxButtonSet buttonSet;
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
        public MsgBxButtonSet ButtonSet
        {
            get { return buttonSet; }
            set
            {
                buttonSet = value;
                OnPropertyChanged();
            }
        }
        public MsgBxButton MsgBxResult { get; private set; }
        //public SolidColorBrush OnMouseOverColor
        //{
        //    get { return onMouseOverColor; }
        //    set
        //    {
        //        onMouseOverColor = value;
        //        OnPropertyChanged();
        //    }
        //}


        public WndMsgBxVM(string title, string text, MsgBxPicture picture, MsgBxButtonSet buttonSet)
        {
            Title = title;
            Text = text;
            Picture = picture;
            ButtonSet = buttonSet;
            //if (onMouseOverColor != default) OnMouseOverColor = onMouseOverColor;

        }

      
        public void SetResult(TextBlock txbAction)
        {
            switch (txbAction.Name)
            {
                case "txbYes":
                    MsgBxResult = MsgBxButton.Yes;
                    break;
                case "txbNo":
                    MsgBxResult = MsgBxButton.No;
                    break;
                case "txbOk":
                    MsgBxResult = MsgBxButton.Ok;
                    break;
                case "txbCancel":
                    MsgBxResult = MsgBxButton.Cancel;
                    break;
                default:
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "Ricevuto txbAction.Name disatteso:<" + txbAction.Name + ">")));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
