using Main.Binds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Main.MsgBxes;

namespace Main.FSes
{
    public class FilesSelectorVM : INotifyPropertyChanged
    {
        FilesSelectorM filesSelectorM;
        PathWithFilters selectedPath;
        ViewStates viewState;
        string testResult;

        public FilesSelectorM FilesSelectorM
        {
            get { return filesSelectorM; }
            set
            {
                filesSelectorM = value;
                OnPropertyChanged();
            }
        }
        public PathWithFilters SelectedPath
        {
            get { return selectedPath; }
            set
            {
                selectedPath = value;
                OnPropertyChanged();
            }
        }
        public ViewStates ViewState
        {
            get { return viewState; }
            set
            {
                viewState = value;
                OnPropertyChanged();
            }
        }
        public string TestResult
        {
            get { return testResult; }
            set
            {
                testResult = value; //ctrlValue non serve, la imposto solo io da codice
                OnPropertyChanged();
            }
        }

        public FilesSelectorVM(FilesSelectorM filesSelectorM) //FilesSelectorM parameter constructor for CreateVmFromM Converter 
        {
            FilesSelectorM = filesSelectorM;
        }

        async internal void TestPath()
        {
            if (SelectedPath == null)
            {
                MsgBx.Show("", "There isn't a selected path, click on a path", MsgBxPicture.Info);
                return;
            }

            ViewState = ViewStates.InTestCalculation;
            StringBuilder result = new StringBuilder();

            Task<bool> taskThread = Task<bool>.Factory.StartNew(() => SelectedPath.CalculateFileList());
            await taskThread;

            if (taskThread.Result == false)
            {
                MsgBx.Show("", "Selected path is not valid", MsgBxPicture.Info);
                return;
            }

            foreach (string file in SelectedPath.FilesSelected)
                result.Append(file + Util.crLf);

            TestResult = result.ToString().RemoveFinal(Util.crLf);

            ViewState = ViewStates.InTestResultShowing;
        }

        async internal void TestPaths()
        {
            ViewState = ViewStates.InTestCalculation;
            StringBuilder result = new StringBuilder();

            Task<BindingList<string>> taskThread = Task<BindingList<string>>.Factory.StartNew(() => FilesSelectorM.TestFileList());
            await taskThread;

            foreach (string file in taskThread.Result)
                result.Append(file + Util.crLf);

            TestResult = result.ToString().RemoveFinal(Util.crLf);

            ViewState = ViewStates.InTestResultShowing;
        }

        internal void CloseTest()
        {
            ViewState = ViewStates.Normal;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
