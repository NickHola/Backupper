using System;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using Main.DBs;
using Main.Logs;
using Main.Thrs;

namespace Main.SQLes
{
    public class SqlObj
    {
        StrConnection connStr; OleDbCommand cmd; OleDbTransaction tra; QryOut @out; Exception thrEx;

        public OleDbDataReader ResOle { get; set; }
        public DataTable ResDt { get; set; }

        public SqlObj(StrConnection connStr = null, OleDbCommand cmd = null, OleDbTransaction tra = null)
        {
            if (connStr != null) this.connStr = connStr;
            if (cmd != null)
                this.cmd = cmd;
            else
                this.cmd = new OleDbCommand(); //Messo qui e non dentro ConnettiDB per riutilizzare lo stesso oggetto, ConnettiDB avrà sempre cmd inizializzato

            this.tra = tra;
        }

        private void DisconnettiDB()
        {
            if (cmd.Connection != null && cmd.Connection.State == ConnectionState.Open)
            { cmd.Connection.Close(); }
        }

        public bool ConnettiDB(Mess logMess = null) //StrConnection strConn = null, 
        {
            if (logMess == null) logMess = new Mess(LogType.Warn, Log.main.warnUserText);

            this.DisconnettiDB();  //Seve perchè se riutilizzo più volte ConnettiDB dentro una stessa funzione devo chiudere la connessione precedente
            bool esito; Thread thread; string testoTmp;
            esito = false;

            if (connStr == null)
            {
                logMess.testoDaLoggare = "Impossibile connettersi al DB poichè connStr è null";
                Log.main.Add(logMess);
                return false;
            }

            if (connStr.DbRemoto == false)
            {
                try
                {
                    cmd.Connection = new OleDbConnection(connStr.Completa);

                    thread = Thr.AvviaNuovo(() => esito = ThrCDB_Exec());

                    if (Thr.AttesaCompletamento(ref thread, connStr.TimeOutConnMs) == false)
                    {
                        logMess.testoDaLoggare = "query:<" + cmd.CommandText + "> andata in timeOut, strConn.timeOutConnMs:<" + connStr.TimeOutConnMs + ">";
                        Log.main.Add(logMess);
                        return false;
                    }

                    if (esito == false)
                    {
                        testoTmp = thrEx != null ? ", ex.mess:<" + thrEx.Message + ">" : "il thread ThrCDB_Exec è ritornato false ma senza eccezioni";

                        logMess.testoDaLoggare = "strConn.completa:<" + connStr.Completa + ">" + testoTmp;
                        Log.main.Add(logMess);
                        return false;
                    }

                }
                catch
                {
                    Log.main.Add(new Mess(LogType.ERR, logMess.testoDaVisual, "Non è stato possibile connettersi al DataBase locale, strConn.completa:<" + connStr.Completa + ">", visualMsgBox: false));
                    return false;
                }
            }
            else
            {
                //Dim idRndXConnRemote As String
                //idRndXConnRemote = GeneraStrRandom(5)
                //While stackConnRemoteCmd.TryAdd(idRndXConnRemote, sqlCmd) = False ' * **ATTENZIONE: usare sempre 'TryAdd' e mai la 'GetOrAdd' poichè quest'ultima non è detto che lo faccia l'add
                //End While
                //While stackConnRemoteTra.TryAdd(idRndXConnRemote, sqlTra) = False ' * **ATTENZIONE: usare sempre 'TryAdd' e mai la 'GetOrAdd' poichè quest'ultima non è detto che lo faccia l'add
                //End While
                //While stackConnRemoteStato.TryAdd(idRndXConnRemote, 2) = False  ' * **ATTENZIONE: usare sempre 'TryAdd' e mai la 'GetOrAdd' poichè quest'ultima non è detto che lo faccia l'add
                //End While
                //While stackConnRemoteStato(idRndXConnRemote) = 2
                //    Sleep(1)
                //End While
                //If stackConnRemoteStato(idRndXConnRemote) = 1 Then
                //    sqlCmd = stackConnRemoteCmd.GetOrAdd(idRndXConnRemote, sqlCmd)
                //    sqlTra = stackConnRemoteTra.GetOrAdd(idRndXConnRemote, sqlTra)
                //    EliminaIdRndXConnRemote(idRndXConnRemote)
                //    Return True
                //Else
                //    EliminaIdRndXConnRemote(idRndXConnRemote)
                //    Return False
                //End If
            }
            return true;
        }

        private bool ThrCDB_Exec()
        {
            Thr.SbloccaThrPadre();
            thrEx = null;

            try
            { //Il Try Catch serve quà poichè essendo un altro thread rispetto al chiamante, il chiamante non può intercettare le eccezioni
                cmd.Connection.Open();
                tra = cmd.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = tra;
                return true;
            }
            catch (Exception ex)
            {
                thrEx = ex;
                return false;
            }
        }

        private bool PreExec(bool isQuery, string query, ref StrConnection strConn, NuovaConn nuovaConn, ref Int32 timeOutQuery, Mess logMess)
        {
            if (strConn != null) this.connStr = strConn;
            if (this.connStr == null)
            {
                if (App.Db.ConnString != null)
                { this.connStr = App.Db.ConnString; }
                else
                {
                    logMess.testoDaLoggare = "ricevuto strConn a nothing, Me.strConn è nothing e App.Db.ConnString è nothing, nessuna stringa di connessione disponibile";
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare));  //Qui ci vuole sempre errore indipendentemente da tipoLog, per questo faccio un nuovo mess
                    return false;
                }
            }

            if (this.connStr.IsInizializzata == false)
            { if (this.connStr.InizializzaSingoliCampi() == false) return false; }


            if (timeOutQuery == 0)
            { //se timeOutQuery=0 Carica timeout della config a meno che non sia locale (in questo caso attesa di tempo sempre infinita)
                if (this.connStr.DbRemoto == false)
                { timeOutQuery = -1; } //Attesa di tempo infinita
                else if (isQuery == true)
                { timeOutQuery = Sql.config.timeOutQuery; }
                else
                { timeOutQuery = Sql.config.timeOutNonQuery; }
            }
            else if (timeOutQuery < -1)
            {
                logMess.testoDaLoggare = "ricevuto timeOutQuery inferiore a -1";
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui ci vuole sempre errore indipendentemente da tipoLog, per questo faccio un nuovo mess
                return false;
            }

            switch (nuovaConn)
            { //Sono state messe in ordine di probabilità
                case NuovaConn.seNecessario:
                    if (cmd.Connection == null || cmd.Connection.State != ConnectionState.Open)
                    { if (ConnettiDB(logMess) == false) return false; }
                    break;
                case NuovaConn.no: //Controllo che sia tutto ok per una nuova query, ma non devo fare altro
                    if (cmd.Connection == null)
                    {
                        logMess.testoDaLoggare = "nuovaConn = no ma cmd.Connection.State è nothing";
                        Log.main.Add(new Mess(LogType.ERR, "", logMess.testoDaLoggare));
                        return false;
                    }

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        logMess.testoDaLoggare = "nuovaConn = no ma cmd.Connection.State diverso da open, state:<" + cmd.Connection.State.ToString() + ">";
                        Log.main.Add(new Mess(LogType.Warn, "", logMess.testoDaLoggare));
                        return false;
                    }
                    break;
                case NuovaConn.si:
                    if (ConnettiDB(logMess) == false) return false;
                    if (this.connStr.DbRemoto == true)
                    { if (cmd.Connection == null || cmd.Connection.State != ConnectionState.Open) return false; }
                    break;

                default:
                    logMess.testoDaLoggare = "valore disatteso per nuovaConn:<" + nuovaConn.ToString() + ">";
                    Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare)); //Qui ci vuole sempre errore indipendentemente da tipoLog
                    return false;
            }


            //If nuovaConn = True Then
            //    If ConnettiDB(Me.strConn, visualErr) = False Then Return False
            //    If Me.strConn.dbRemoto = True Then
            //        If IsNothing(cmd.Connection) = True OrElse cmd.Connection.State <> ConnectionState.Open Then Return False
            //    End If
            //Else
            //    If IsNothing(cmd.Connection) = False AndAlso cmd.Connection.State <> ConnectionState.Open Then 'In questo caso comunque sia non posso fare connettiDB(poichè potrebbero essere una serie di insert con ...
            //        log.Acc(New Mess(Tipi.avv, "", "nuovaConn = false ma cmd.Connection.State diverso da open, state:<" & cmd.Connection.State.ToString & ">")) '...commit finale, quindi fallirà l'esecuzione della query)
            //    End If
            //End If

            cmd.CommandText = query + "  --***Stack sub: " + Util.GetCallStack();

            return true;
        }

        public bool ExecQuery(object p)
        {
            throw new NotImplementedException();
        }

        public bool ExecQuery(string query, QryOut @out = QryOut.dataTable, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, int timeOutQuery = 0, Mess logMess = null)
        {
            object res = null;
            return ExecQuery(query, ref res, @out: @out, nuovaConn: nuovaConn, strConn: strConn, timeOutQuery: timeOutQuery, logMess: logMess);
        }

        //ATTENZIONE res se non è un ref non funziona
        public bool ExecQuery<T1>(string query, ref T1 res, QryOut @out = QryOut.dataTable, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, int timeOutQuery = 0, Mess logMess = null)
        {   //Attenzione: l'out come Datatable costa di più in termini di cpu, dataReader costa meno
            if (logMess == null) logMess = new Mess(LogType.ERR, Log.main.errUserText);
            logMess.testoDaLoggare = "";

            Thread thread; Exception thrEx = null; bool esito; string testoTmp;

            if (PreExec(true, query, ref strConn, nuovaConn, ref timeOutQuery, logMess) == false) return false;

            if (res == null)
            { this.@out = @out; }
            else
            {
                if (res.GetType() == typeof(DataTable))
                { this.@out = QryOut.dataTable; }
                else if (res.GetType() == typeof(OleDbDataReader))
                { this.@out = QryOut.dataReader; }
                else
                {
                    logMess.testoDaLoggare = "ricevuto res con tipo disatteso, res.GetType.Name:<" + res.GetType().Name + ">";
                    Log.main.Add(logMess);
                    return false;
                }
            }

            esito = false;
            //se si perde la connessione prima di eseguire cmd.ExecuteReader() (può succede per DB remoto quando si è già connessi, quindi con nuovaConn=False) passano 47 secondi prima che ritorna l'errore data provider interno 30,
            thread = Thr.AvviaNuovo(() => esito = ThrEQ_Exec(ref thrEx), ApartmentState.MTA); //Attenzione se STA, alla 'resOle.Close()' c'è l'eccezione: Impossibile utilizzare oggetti COM separati dai relativi RCW sottostanti.

            if (Thr.AttesaCompletamento(ref thread, timeOutQuery) == false)
            {
                logMess.testoDaLoggare = "query:<" + cmd.CommandText + "> andata in timeOut:<" + timeOutQuery + ">";
                Log.main.Add(logMess);
                return false;
            }

            if (esito == false)
            {
                if (thrEx != null)
                {
                    if (FiltraEccezioniQuery(thrEx) == true)
                    {
                        logMess.testoDaVisual = "";
                        logMess.tipo = LogType.Warn;
                    }
                    testoTmp = "ex.mess:<" + thrEx.Message + ">";
                }
                else
                { testoTmp = "il thread ThrEQ_Exec è ritornato false ma senza eccezioni"; }

                logMess.testoDaLoggare = "query:<" + cmd.CommandText + ">, " + testoTmp;
                Log.main.Add(logMess);
                return false;
            }
            //Ho dovuto eseguire la resDt.Load(cmd.ExecuteReader()) nel delegato poichè se esegiuvo la resDt.Load(DataReader) qua da la seguente eccezione
            //Impossibile eseguire il cast di oggetti COM di tipo 'System.__ComObject' in tipi di interfaccia 'IRowset'. L'operazione non è stata completata perché la chiamata QueryInterface sul componente COM per l'interfaccia con IID '{ 0C733A7C - 2A1C - 11CE - ADE5 - 00AA0044773D}
            //non è riuscita a causa del seguente errore: Interfaccia non supportata. (Eccezione da HRESULT: 0x80004002 (E_NOINTERFACE)).

            if (res != null)
            {
                if (this.@out == QryOut.dataTable)
                { res = (T1)(dynamic)ResDt; }
                else if (this.@out == QryOut.dataReader)
                { res = (T1)(dynamic)ResOle; }
            }

            return true;
        }
        
        private bool ThrEQ_Exec(ref Exception thrEx)
        {
            Thr.SbloccaThrPadre();

            thrEx = null;

            try
            {  //Il Try Catch serve quà poichè essendo un altro thread rispetto al chiamante, il chiamante non può intercettare le eccezioni
                if (@out == QryOut.dataTable)
                {
                    ResDt = new DataTable();
                    ResDt.Load(cmd.ExecuteReader());

                }
                else if (@out == QryOut.dataReader)
                {
                    if (ResOle != null && ResOle.IsClosed == false) ResOle.Close();  //Se si riusa l'oggetto query con 'out' 'dataReader' senza una res specifica, bisogna chiudere l'OleDbDataReader

                    //resOle = cmd.ExecuteReader() 'Con cmd.ExecuteReader(CommandBehavior.CloseConnection) nel momento in cui su resOle eseguo il metodo.Close() si chiude anche il cmd.connection, meglio toglierlo poichè la connection mi serve aperta
                    ResOle = cmd.ExecuteReader();
                }
                return true;
            }
            catch (Exception ex)
            {
                thrEx = ex;
                return false;
            }
        }

        public bool ExecNoQuery(string query, CommitRoll commitRollback = CommitRoll.commitRollback, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, Int32 timeOutQuery = 0, Mess logMess = null)
        {

            if (logMess == null) logMess = new Mess(LogType.ERR, Log.main.errUserText);
            logMess.testoDaLoggare = "";

            Thread thread; Exception thrEx = null; bool esito; string testoTmp;

            if (PreExec(true, query, ref strConn, nuovaConn, ref timeOutQuery, logMess) == false) return false;

            esito = false;
            thread = Thr.AvviaNuovo(() => esito = ThrENQ_Exec(out thrEx));

            if (Thr.AttesaCompletamento(ref thread, timeOutQuery) == false)
            { //qua non faccio rollback poichè presumo che la connessione remota sia andata persa, se DB in locale non può andare in timeout
                logMess.testoDaLoggare = "query:<" + cmd.CommandText + "> andata in timeOut:<" + timeOutQuery + ">";
                Log.main.Add(logMess);
                return false;
            }

            if (esito == false)
            {
                try
                {
                    if (commitRollback == CommitRoll.commitRollback || commitRollback == CommitRoll.soloRollback)
                    {
                        tra.Rollback();
                        this.DisconnettiDB();
                    }
                }
                catch (Exception ex)
                {
                    logMess.testoDaVisual = ""; //Non visualizzo nulla poichè c'è il l'altro log sotto
                    logMess.testoDaLoggare = "eccezione durante rollback, ex.Mess:<" + ex.Message + ">, query:<" + query + ">";
                    Log.main.Add(logMess);
                }

                if (thrEx != null)
                {
                    if (FiltraEccezioniQuery(thrEx) == true)
                    {
                        logMess.testoDaVisual = "";
                        logMess.tipo = LogType.Warn;
                    }
                    testoTmp = "ex.mess:<" + thrEx.Message + ">";
                }
                else
                    testoTmp = "il thread ThrENQ_Exec è ritornato false ma senza eccezioni";


                logMess.testoDaLoggare = "query:<" + cmd.CommandText + ">, " + testoTmp;
                Log.main.Add(logMess);
                return false;
            }

            try
            {
                if (commitRollback == CommitRoll.commitRollback)
                {
                    tra.Commit();
                    this.DisconnettiDB();
                }
            }
            catch (Exception ex)
            {
                if (FiltraEccezioniCommit(ex) == true)
                {
                    logMess.testoDaVisual = "";
                    logMess.tipo = LogType.Warn;
                }

                logMess.testoDaLoggare = "errore durante 'sqlTra.Commit()', ex.Mess:<" + ex.Message + ">, query:<" + query + ">";
                Log.main.Add(logMess);
                return false;
            }

            return true;
        }

        private bool ThrENQ_Exec(out Exception thrEx)
        {
            Thr.SbloccaThrPadre();
            thrEx = null;

            try
            { //Il Try Catch serve quà poichè essendo un altro thread rispetto al chiamante, il chiamante non può intercettare le eccezioni
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                thrEx = ex;
                return false;
            }
        }

        internal static bool FiltraEccezioniQuery(Exception ex)
        {
            string mess, tipo; bool esito;
            mess = ex.Message;
            tipo = ex.GetType().ToString();
            esito = false;

            if (esito == false && mess.Contains("timeout") && mess.Contains("query")) esito = true;
            if (esito == false && mess.Contains(".net") && mess.Contains("framework") && mess.Contains("data") && mess.Contains("provider")) esito = true; //Sub:ThreadStart_Context->thrCicloSincronizzaOraConServer->ExecQuery |  - Query:<SELECT excOra FROM excDispositivi WHERE idRnd='OV359'  --***Sub: thrCicloSincronizzaOraConServer>, ErrDescr:<Errore .Net Framework Data Provider 30 interno.> , ErrNum:<5>
            if (esito == false && mess.Contains("provider") && mess.Contains("tcp")) esito = true;
            if (esito == false && mess.Contains("Errore") && mess.Contains("collegamento") && mess.Contains("durante") && mess.Contains("comunicazione")) esito = true; //Sub:ThreadStart_Context->thrCicloExchangeComandeEliminaTavoliChiusi->ExecQuery |  - Query:<SELECT idRiga FROM excComande  --***Sub: thrCicloExchangeComandeEliminaTavoliChiusi>, ErrDescr:<Errore di collegamento durante la comunicazione. Provider TCP: Il nome di rete specificato non è più disponibile.> , ErrNum:<5>
            if (esito == false && mess.Contains("communication") && mess.Contains("link") && mess.Contains("failure")) esito = true;
            if (esito == false && mess.Contains("Impossibile creare una nuova connessione perché è attivata la modalità di transazione manuale o distribuita")) esito = true; //<SELECT * FROM ExcCtrl WHERE nome='EventoInCorso'><Impossibile creare una nuova connessione perché è attivata la modalità di transazione manuale o distribuita.> lo ha generato poichè durante il runtime mi sono collegato ad un'altra rete

            if (esito == false && mess.Contains("Riferimento a un oggetto non impostato su un'istanza di oggetto")) esito = true; //ErrDescr:<Riferimento a un oggetto non impostato su un'istanza di oggetto.> , ErrNum:<91>
            if (esito == false && mess.Contains("Executereader richiede una oggetto connection aperto e disponibile. lo stato attuale della connessione è connessione in corso")) esito = true; //Sub:RunInternal->thrCicloSincronizzaOraConServer->ExecQuery |  - Query:<SELECT excOra FROM excDispositivi WHERE idRnd='6A>Õ0'  --***Sub: thrCicloSincronizzaOraConServer>, ErrDescr:<ExecuteReader richiede una oggetto Connection aperto e disponibile. Lo stato attuale della connessione è connessione in corso.> , ErrNum:<5>
            if (esito == false && mess.Contains("Executereader richiede una oggetto connection aperto e disponibile. lo stato attuale della connessione è chiuso")) esito = true;
            if (esito == false && mess.Contains("La transazione non è associata alla connessione corrente oppure è stata completata")) esito = true; //Sub:ThreadStart_Context->thrCicloExchangeComandeRicezione->ExecQuery |  - Query:<SELECT idRiga FROM excComande  --***Sub: thrCicloExchangeComandeRicezione>, ErrDescr:<La transazione non è associata alla connessione corrente oppure è stata completata.> , ErrNum:<5>

            if (esito == false && Thr.FiltraEccezioniThreadNonCiclanti(ex) == true) esito = true;

            return esito;
        }

        private bool FiltraEccezioniCommit(Exception ex)
        {
            string mess, tipo; bool esito;
            mess = ex.Message;
            tipo = ex.GetType().ToString();
            esito = false;

            if (esito == false && mess.Contains("riferimento a un oggetto non impostato su un'istanza di oggetto")) return true;
            if (esito == false && mess.Contains("la oledbtransaction è completata e non può più essere utilizzata")) return true;
            if (esito == false && mess.Contains(".net") && mess.Contains("framework") && mess.Contains("data") && mess.Contains("provider")) return true;
            if (esito == false && mess.Contains("errore") && mess.Contains("collegamento") && mess.Contains("durante") && mess.Contains("comunicazione")) return true;
            if (esito == false && mess.Contains("communication") && mess.Contains("link") && mess.Contains("failure")) return true;

            return esito;
        }
    }
}
