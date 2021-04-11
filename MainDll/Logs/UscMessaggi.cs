using Main.Controls;
using Main.Logs;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Main.Logs2
{

    internal class UscMessaggi : UserControl
    {
        private DataTable dtMessagi; DataGridM dataGrid;
        internal ConfigMess config;
        internal TipiUscMessaggi tipoUscMess;
        internal string nome;

        internal UscMessaggi(string nome, TipiUscMessaggi tipoUscMess, Configs.SaveLocation saveLocation = null)
        {
            this.nome = nome;
            this.tipoUscMess = tipoUscMess;

            dtMessagi = new DataTable();
            dtMessagi.Columns.Add("id", typeof(UInt64));
            dtMessagi.Columns.Add("ora", typeof(DateTime));
            dtMessagi.Columns.Add("tipo", typeof(string));
            dtMessagi.Columns.Add("testo", typeof(string));
            dtMessagi.Columns["id"].AutoIncrement = true;
            this.config = new ConfigMess(this.nome);

            dataGrid = new DataGridM { ValidaRighe = false };
            dataGrid.AutoGenerateColumns = false;

            DataGridTextColumn col;

            col = new DataGridTextColumn();
            col.Binding = new Binding("id");
            col.Visibility = Visibility.Hidden;
            dataGrid.Columns.Add(col);

            col = new DataGridTextColumn();
            col.Header = "Ora";
            col.Binding = new Binding("ora") { StringFormat = "{0:MM/dd  HH:mm:ss}" };
            dataGrid.Columns.Add(col);

            col = new DataGridTextColumn();
            col.Header = "Tipo";
            col.Binding = new Binding("tipo");
            dataGrid.Columns.Add(col);

            col = new DataGridTextColumn();
            col.Header = "Testo";
            col.Binding = new Binding("testo");
            dataGrid.Columns.Add(col);

            dataGrid.ItemsSource = dtMessagi.DefaultView;

            this.AddChild(dataGrid);
        }

        public bool accoda(Mess mess)
        {
            string testo, tipoMess; DataRow dataRow = null;

            switch (tipoUscMess)
            {
                case TipiUscMessaggi.Mess:
                    if (mess.testoDaVisual == "") return true;
                    testo = mess.testoDaVisual;
                    break;

                case TipiUscMessaggi.MessSoloLog:
                    if (mess.testoDaVisual != "") return true;
                    testo = mess.testoDaLoggare;
                    break;

                default:
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "tipo UscMessaggi disatteso, tipo:<" + tipoUscMess.ToString() + ">"));
                    return false;
            }

            switch (mess.tipo)
            {
                case LogType.info:
                    tipoMess = "info";
                    break;
                case LogType.Warn:
                    tipoMess = "Avvertenza";
                    break;
                case LogType.ERR:
                    tipoMess = "ERRORE";
                    break;
                default:
                    tipoMess = mess.tipo.ToString();
                    break;
            }

            //BeginInvoke Serve poichè il thrCicloGestoreMessaggi se aggiunge direttamente righe al dtMessagi dato che è collegato a dataGrid non creato da lui andrebbe in errore
            Application.Current.Dispatcher.Invoke(() => dataRow = dtMessagi.Rows.Add(null, mess.oraCreazione, tipoMess, testo));    //new Action(Sub() dataRow = dtMessagi.Rows.Add(Nothing, mess.oraCreazione, tipoMess, testo)))

            while (dtMessagi.Rows.Count > config.numMaxMessVisuali)  //Rimuovo i vecchi mess.
                Application.Current.Dispatcher.Invoke(() => dtMessagi.Rows.RemoveAt(0)); //a volte da problemi anche qui

            if (dtMessagi.Rows.Count > 0) Application.Current.Dispatcher.Invoke(() => dataGrid.ScrollIntoView(dataRow)); //Scorro alla fine 'a volte da problemi anche qui
            return true;
        }
        //Private Sub Me_Initialized() Handles MyBase.Initialized
        //End Sub
    }

}
