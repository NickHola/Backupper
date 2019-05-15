using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Main.Schedulers;
using Main.Zips;
using System.Threading;

namespace Backupper
{
    /// <summary>
    /// Interaction logic for wndTest.xaml
    /// </summary>
    public partial class wndTest : Window, INotifyPropertyChanged
    {


        private OnOffSeries onOffUnitSeries;  //BindingList(Of Tuple(Of UInt16, UInt16))

        public OnOffSeries aaaa
        { //BindingList(Of Tuple(Of UInt16, UInt16))
            get { return onOffUnitSeries; }
            set
            { //BindingList(Of Tuple(Of UInt16, UInt16)))
              //ValidazioneNs.Validation(value)
              //if (value != null)
              //{
              //    string descErr = "";
              //    foreach (var tupla in value)
              //    {
              //        if (ValidaTuplaOnOff(tupla, out descErr) == false)
              //            throw new Exception("Valore non valido: " + descErr);
              //    }
              //}
                onOffUnitSeries = value;
                OnPropertyChanged();
            }
        }


        private SchedulerVM scheduler;
        public SchedulerVM SchedulerVM
        {
            get { return scheduler; }
            set
            {
                scheduler = value;
                OnPropertyChanged();
            }
        }

        public wndTest()
        {
            InitializeComponent();
            //SchedulerVM = new SchedulerVM();
            //SchedulerVM.SchedulerM.WeekDays = new WeekDays(new List<WeekDayObj> { new WeekDayObj(Main.DataOre.WeekDay.Monday) });
            //aaaa = new OnOffSeries();
            //DataGridRow cia = new DataGridRow();
            //cia.Sele



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread zipThr = null;
            Main.Thrs.Thr.AvviaNuovo(() => thrCiao(ref zipThr));
            Thread.Sleep(5000);
            zipThr.Abort();
        }

        private void thrCiao(ref Thread zipThr)
        {
            Main.Thrs.Thr.SbloccaThrPadre();
            Zip.Comprimi(@"C:\Users\CIAO\Desktop\TestBackup", @"C:\Users\CIAO\Desktop\DestinationBackup\test1.7zip", out zipThr);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }


    }

    public class eccolo2 : BindingList<eccolo>
    {
        public eccolo2() { }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class eccolo : INotifyPropertyChanged
    {

        private string item1;  //BindingList(Of Tuple(Of UInt16, UInt16))

        public string Item1
        { //BindingList(Of Tuple(Of UInt16, UInt16))
            get { return item1; }
            set
            { //BindingList(Of Tuple(Of UInt16, UInt16)))
              //ValidazioneNs.Validation(value)
              //if (value != null)
              //{
              //    string descErr = "";
              //    foreach (var tupla in value)
              //    {
              //        if (ValidaTuplaOnOff(tupla, out descErr) == false)
              //            throw new Exception("Valore non valido: " + descErr);
              //    }
              //}
                item1 = value;
                OnPropertyChanged();
            }
        }

        private string item2;  //BindingList(Of Tuple(Of UInt16, UInt16))

        public string Item2
        { //BindingList(Of Tuple(Of UInt16, UInt16))
            get { return item2; }
            set
            { //BindingList(Of Tuple(Of UInt16, UInt16)))
              //ValidazioneNs.Validation(value)
              //if (value != null)
              //{
              //    string descErr = "";
              //    foreach (var tupla in value)
              //    {
              //        if (ValidaTuplaOnOff(tupla, out descErr) == false)
              //            throw new Exception("Valore non valido: " + descErr);
              //    }
              //}
                item2 = value;
                OnPropertyChanged();
            }
        }

        public eccolo() { }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

}
