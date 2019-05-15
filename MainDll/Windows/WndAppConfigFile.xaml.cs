using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Main.MsgBxes;
using Main.Roots;
using Main.Serializes;
using Main.Logs;
using Main.Validations;
using Main.Salvable;
using Main;

namespace Main
{
    /// <summary>
    /// Logica di interazione per WndAppConfigFile.xaml
    /// </summary>
    public partial class WndAppConfigFile : Window, INotifyPropertyChanged
    {
        ConfigWndDiDiagnostica config;
        public ConfigWndDiDiagnostica Config
        {
            get { return config; }
            set
            { //Settabile solo all'interno della dll
                Validation.CtrlValue(value);
                config = value;
                OnPropertyChanged();
            }
        }

        public WndAppConfigFile()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Root.RichiestaAccesso() == false) this.Visibility = Visibility.Hidden;

            Config = new ConfigWndDiDiagnostica("WndAppConfigFile");
            //Config = (ConfigWndDiDiagnostica)Savable.Load(Config);
            Config = (ConfigWndDiDiagnostica)Config.Load(Main.App.Config, out _);
            //Config = (ConfigWndDiDiagnostica)Savable.Load(Config);

            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.7;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.7;

            this.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 0.15;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.15;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Root.AccessoRoot == false)
            {
                this.Close(); //TODO trovare un altro modo, così si riesce a vedere, forse con l'hidden nel Window_Initialized
            }
            btnCarica_Click(btnCarica, new RoutedEventArgs());
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Config != null) Config.Save(Main.App.Config); //ConfigWndAppConfigFile viene sempre memorizzato in file config dell'app
        }

        private void btnTerminaApp_Click(object sender, RoutedEventArgs e)
        {
            App.ClosingProcedure(salvaConfigApp: false);
        }

        private void BtnDBConnTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(App.Config.MainDbConnString.ConnectionTest().ToString());
        }

        private void btnCarica_Click(object sender, RoutedEventArgs e)
        {
            string tmpStr = txtAppConfig.Text;
            //Apps.AppConfigFile.LoadFromFile(ref App.config, testo: out tmpStr, encrypted: out _);
            App.config = (Apps.AppConfigFile)App.config.LoadFromFile(testo: ref tmpStr, encrypted: out _, inErr: out _);
            if (tmpStr != "") txtAppConfig.Text = tmpStr;


            if (txtAppConfig.Text == "")
            {
                MsgBx.Show("", "Non sono riuscito a leggere il file o il file è vuoto, consultare il log", MsgBxPicture.Alert);

                if (Serialize.SerializeInText(App.Config, ref tmpStr) == false)
                    txtAppConfig.Text = "";
                else
                    txtAppConfig.Text = tmpStr;
            }
        }

        private void btnSalva_Click(object sender, RoutedEventArgs e)
        {
            //Apps.AppConfigFile tmp = App.Config;
            if (Serialize.DeserializeFromText(txtAppConfig.Text, ref App.config) == false)
                MsgBx.Show("", "Non sono riuscito a deserializzare la configurazione nell'oggetto App.config, consultare il log", MsgBxPicture.Critical);

            if (App.Config.SaveOnFile(App.config.IsEncrypted) == true)
            {
                //if (Apps.AppConfigFile.LoadFromFile(ref App.config, out _) == true)
                App.config = (Apps.AppConfigFile)App.config.LoadFromFile(out bool inErr);
                if  (inErr == false)
                {
                    Log.main.Add(new Mess(Tipi.info, "Configurazione salvata correttamente"));
                    this.Close();
                }
                else
                { MsgBx.Show("", "La configurazione è stata salvata su file ma il successivo caricamento è fallito, consultare il log", MsgBxPicture.Critical); }
            }
            else
            { MsgBx.Show("", "Non sono riuscito a salvare la configurazione nel file, consultare il log", MsgBxPicture.Critical); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName()] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }


    }
}
