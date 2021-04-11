using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Main.SQLes;
using Main.Logs;
using Main.Validations;

namespace Main.DBs
{
    public abstract class TabellaBase
    {
        public readonly string Nm, NmUte;  //I nomi dei field e delle property devono essere nomi che poi non verranno usati come nome di un DbCampo dalla classe che erediterà da DbTabella
        bool IsView; 
        internal List<ForeignKey> listaFK = new List<ForeignKey>();
        //internal DataTable datiQryPerValidaBL;
        string nomeColIdentity;

        protected static string DammiNomeTab(Type tipoTab)
        {
            Validation.CtrlValue(tipoTab, nomeVar: "tipoTab", ctrlVoid: false);
            return tipoTab.Name.ParoleMinuMaiu(Str.MinMai.minu, soloPrimaParola: true);
        }

        public string NomeColIdentity
        {
            get { return nomeColIdentity; }
            internal set
            {
                Validation.CtrlValue(value);
                if (nomeColIdentity != "")
                {
                    //descErr = "nomeColIdentity è già stato settato con il valore:<" & nomeColIdentity_ & ">"
                    //log.Acc(New Mess(Tipi.err, log.errUserText, "nomeColIdentity è già stato settato con il valore:<" & nomeColIdentity_ & ">"))
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "nomeColIdentity è già stato settato con il valore:<" + nomeColIdentity + ">")));
                }
                else { nomeColIdentity = value; }

            }
        }

        protected TabellaBase(string nomeUtente = "", bool isView = false)
        {
            this.Nm = DammiNomeTab(this.GetType());
            this.NmUte = nomeUtente == "" ? this.Nm : nomeUtente;
            this.IsView = isView;
            this.nomeColIdentity = "";
        }

        public void PulisciValoriColonne(bool ancheEsprSql = true)
        {
            Type tipoTabella; Column col;

            tipoTabella = this.GetType();

            foreach (FieldInfo proprietàCol in tipoTabella.GetFields())
            {  //Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
                if (proprietàCol.FieldType != typeof(Column)) continue;
                col = (Column)proprietàCol.GetValue(this);
                col.PulisciValori();
            }
        }

        internal bool AggiungiAListaFK(Column col, List<Column> colonneFk)
        {
            foreach (Column colFk in colonneFk)
            {
                if (AggiungiAListaFK(col, colFk.Padre.Nm, colFk.Nm) == false) return false;
            }
            return true;
        }

        internal bool AggiungiAListaFK(Column col, List<Type> tabsFk, string nomeCol = "id")
        {
            foreach (Type tabFk in tabsFk)
            {
                if (AggiungiAListaFK(col, TabellaBase.DammiNomeTab(tabFk), nomeCol) == false) return false;
            }
            return true;
        }

        private bool AggiungiAListaFK(Column col, string nomeTabFk, string nomeColFk)
        {
            string nomeFK; //, i As Byte

            if (col.Nm.Left(3) != "fk_")
            {
                Log.main.Add(new Mess(LogType.ERR, "", "la colonna:<" + col.Nm + ">, della tabella:<" + col.Padre.Nm + "> è una Fk ma il suo nome non inizia con fk_"));
                App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
            }

            nomeFK = "FK_" + col.Padre.Nm + "_" + col.Nm.Right(-3).ParoleMinuMaiu(Str.MinMai.minu, soloPrimaParola: true); //FK_ maiuscola poichè di default sql management lo scrive maiuscolo

            if ((from tmp in listaFK where tmp.nome == nomeFK select tmp).Count() > 0)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "per la tabella:<" + col.Padre.Nm + "> esistono già esiste una Fk di nome:<" + nomeFK + ">"));
                App.ClosingProcedure(salvaConfigApp: false, tSleepMs: Log.main.tStimatoPerLoggareMs);
            }

            //Do
            //    nomeFK = "fk_" & col.padre.nmTab & "_" & nomeTabFk
            //    i += 1
            //    If i > 1 Then nomeFK += "_" & i
            //Loop While (From tmp In listaFK Where tmp.nome = nomeFK).Count > 0

            listaFK.Add(new ForeignKey(nomeFK, col.Padre.Nm, col.Nm, nomeTabFk, nomeColFk));

            return true;
        }

        
        public bool Insert(string selecDeiValori = "", NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback, Int32 timeOutQuery = 0, Mess logMess = null)
        {
            SqlObj sql = null;
            return Insert(selecDeiValori, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);
        }

        public bool Insert(ref SqlObj sql, string selecDeiValori = "", NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback, Int32 timeOutQuery = 0,
            Mess logMess = null)
        { return Insert(selecDeiValori, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess); }

        public bool Insert(string selecDeiValori, ref SqlObj sql, NuovaConn nuovaConn, StrConnection strConn, CommitRoll commitRollback, Int32 timeOutQuery, Mess logMess)
        {
            if (logMess == null) logMess = new Mess(LogType.ERR, Log.main.errUserText);
            if (sql == null) sql = new SqlObj();

            string query;

            if (selecDeiValori == null || selecDeiValori == "")
            {

                Column col; string qryTestata, qryValori; List<string> valori = new List<String>(); bool primaCol; List<string> valXqry;
                primaCol = true;
                qryTestata = qryValori = "";

                foreach (FieldInfo proprietàCol in this.GetType().GetFields())
                {  //Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
                    if (proprietàCol.FieldType != typeof(Column)) continue;

                    col = (Column)proprietàCol.GetValue(this);
                    if (col.Ident == true) continue;  //Se è identity non va nella insert

                    qryTestata += col.Nm + ", ";

                    if (primaCol == true)
                    {
                        foreach (string tmpVal in col.ValXqryList)
                        {
                            valori.Add(tmpVal);
                        }

                        primaCol = false;

                    }
                    else
                    { //2° col in poi

                        valXqry = col.ValXqryList; //valXqryList al suo interno cicla i valori per renderli compatibili con sql, quindi la richiamo solamente una volta per le prestazioni 
                        if (valori.Count != valXqry.Count)
                        {
                            Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "per la tabella:<" + Nm + ">, la colonna:<" + col.Nm + "> ha un numero valori di:<" + valXqry.Count + "> mentre la prima colonna ne prevedeva:<" + valori.Count + "> la insert sarà annullata"));
                            return false;
                        }
                        for (int i = 0; i < valori.Count; i++)
                        {
                            valori[i] += ", " + valXqry[i];
                        }
                    }
                }

                if (ValidaBusinessLogic(Sql.ins, logMess: logMess) == false) return false;

                qryTestata = qryTestata.RemoveFinal(", ");

                foreach (string gruppoValore in valori)
                {
                    qryValori += "(" + gruppoValore + "), ";
                }

                qryValori = qryValori.RemoveFinal(", ");

                query = Sql.ins + Nm + " (" + qryTestata + ") VALUES " + qryValori;

            }
            else
            {
                if (ValidaBusinessLogic(Sql.upd, selecDeiValori, logMess: logMess) == false) return false;
                query = Sql.ins + Nm + " " + selecDeiValori;
            }

            if (sql.ExecNoQuery(query, commitRollback: commitRollback, nuovaConn: nuovaConn, strConn: strConn, timeOutQuery: timeOutQuery, logMess: logMess) == false) return false;

            if (commitRollback == CommitRoll.commitRollback) EventoEseguitaQuery(Sql.ins);

            return true;
        }

        //public bool Insert(Optional selecDeiValori As String = "", Optional ByRef sql As Ogg = Nothing, Optional nuovaConn As NuovaConn = NuovaConn.seNecessario, Optional strConn As StrConnessione = Nothing,
        //                       Optional commitRollback As CommitRoll = CommitRoll.commitRollback, Optional timeOutQuery As Int32 = 0, Optional logMess As Mess = Nothing) As Boolean

        //    If IsNothing(logMess) Then logMess = New Mess(Tipi.err, log.errUserText)
        //    If IsNothing(sql) Then sql = New Ogg

        //    Dim query As String

        //    If IsNothing(selecDeiValori) = True OrElse selecDeiValori = "" Then

        //        Dim col As Colonna, qryTestata, qryValori As String, valori As New List(Of String), primaCol As Boolean, valXqry As List(Of String)
        //        primaCol = True

        //        For Each proprietàCol As Reflection.FieldInfo In Me.GetType.GetFields  'Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
        //            If proprietàCol.FieldType<> GetType(Colonna) Then Continue For
        //            col = proprietàCol.GetValue(Me)
        //            If col.ident = True Then Continue For  'Se è identity non va nella insert

        //            qryTestata += col.nm & ", "

        //            If primaCol = True Then
        //                For Each tmpVal In col.valXqryList
        //                    valori.Add(tmpVal)
        //                Next

        //                primaCol = False

        //            Else '2° col in poi

        //                valXqry = col.valXqryList 'valXqryList al suo interno cicla i valori per renderli compatibili con sql, quindi la richiamo solamente una volta per le prestazioni 
        //                If valori.Count<> valXqry.Count Then
        //                    log.Acc(New Mess(Tipi.err, log.errUserText, "per la tabella:<" & nmTab & ">, la colonna:<" & col.nm & "> ha un numero valori di:<" & valXqry.Count & "> mentre la prima colonna ne prevedeva:<" & valori.Count & "> la insert sarà annullata"))
        //                    Return False
        //                End If

        //                For i = 0 To valori.Count - 1
        //                    valori(i) += ", " & valXqry(i)
        //                Next

        //            End If

        //        Next

        //        If ValidaBusinessLogic(qryIns, logMess:=logMess) = False Then Return False

        //        qryTestata = RimuoviFinale(qryTestata, ", ")

        //        For Each gruppoValore In valori
        //            qryValori += "(" & gruppoValore & "), "
        //        Next

        //        qryValori = RimuoviFinale(qryValori, ", ")

        //        query = qryIns & nmTab & " (" & qryTestata & ") VALUES " & qryValori

        //    Else
        //        If ValidaBusinessLogic(qryUpd, selecDeiValori, logMess:=logMess) = False Then Return False

        //        query = qryIns & nmTab & " " & selecDeiValori

        //    End If


        //    If sql.ExecNoQuery(query, commitRollback:=commitRollback, nuovaConn:=nuovaConn, strConn:=strConn, timeOutQuery:=timeOutQuery, logMess:=logMess) = False Then Return False

        //    If commitRollback = CommitRoll.commitRollback Then EventoEseguitaQuery(qryIns)

        //    Return True
        //End Function

        //Public Function Update(where As String, Optional wherePresente As Boolean = True, Optional ByRef sql As Ogg = Nothing, Optional nuovaConn As NuovaConn = NuovaConn.seNecessario, Optional strConn As StrConnessione = Nothing,
        //                       Optional commitRollback As CommitRoll = CommitRoll.commitRollback, Optional visualLog As Boolean = True, Optional timeOutQuery As Int32 = 0, Optional ByRef descErr As String = "") As Boolean

        public bool UpdateWithPKValue(NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
 Int32 timeOutQuery = 0, Mess logMess = null)
        {
            SqlObj sql = null;
            return UpdateWithPKValue(ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);
        }

        public bool UpdateWithPKValue(ref SqlObj sql, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
     Int32 timeOutQuery = 0, Mess logMess = null)
        {
            string where, val; Column col;
            where = "";

            foreach (FieldInfo proprietàCol in this.GetType().GetFields())
            {  //Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
                if (proprietàCol.FieldType != typeof(Column)) continue;
                col = (Column)proprietàCol.GetValue(this);

                if (col.PrimKey == false) continue;  //Se non è PrimKey la scarto
                val = col.ValXqry;

                if (val == "NULL" || val == "DEFAULT")
                {
                    Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "Non è stata valorizzata la colonna chiave:<" + col.Nm + ">"));
                    return false;
                }

                where += col.Nm + "=" + val + " AND ";
            }

            if (where == "")
            {
                Log.main.Add(new Mess(LogType.Warn, Log.main.warnUserText, "Nella tabella non sono presenti campi chiave"));
                return false;
            }

            where = where.RemoveFinal("AND ");

            return Update(where, true, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);
        }

        public bool Update(string where, bool wherePresente = true, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
            Int32 timeOutQuery = 0, Mess logMess = null)
        {
            SqlObj sql = null;
            return Update(where, wherePresente, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);
        }

        public bool Update(string where, ref SqlObj sql, bool wherePresente = true, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
            Int32 timeOutQuery = 0, Mess logMess = null)
        { return Update(where, wherePresente, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess); }

        public bool Update(string where, bool wherePresente, ref SqlObj sql, NuovaConn nuovaConn, StrConnection strConn, CommitRoll commitRollback, Int32 timeOutQuery, Mess logMess)
        {
            if (logMess == null) logMess = new Mess(LogType.ERR, Log.main.errUserText);
            if (where == null) where = "";

            if (wherePresente == true && where == "") { //Serve poichè inibisce la possibilità di eseguire la query senza la parte where 
                logMess.testoDaLoggare = "ricevuto 'wherePresente' a true, ma ricevuto 'where' a nothing o vuoto";
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, logMess.testoDaLoggare));
                return false;
            }

            if (sql == null) sql = new SqlObj();

            string query, qrySet, val; Column col;
            qrySet = "";

            foreach (FieldInfo proprietàCol in this.GetType().GetFields()) {  //Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
                if (proprietàCol.FieldType != typeof(Column)) continue;
                col = (Column)proprietàCol.GetValue(this);

                if (col.Ident == true) continue;  //Se è identity non va nella Update
                val = col.ValXqry;

                if ((val == "NULL" || val == "DEFAULT") && col.ValNull == false) continue;

                qrySet += col.Nm + "=" + val + ", ";
            }

            if (ValidaBusinessLogic(Sql.upd, logMess: logMess) == false) return false;

            qrySet = qrySet.RemoveFinal(", ");

            if (where != "") where = " WHERE " + where;

            query = Sql.upd + Nm + " SET " + qrySet + where;

            if (sql.ExecNoQuery(query, commitRollback: commitRollback, nuovaConn: nuovaConn, strConn: strConn, timeOutQuery: timeOutQuery, logMess: logMess) == false) return false;

            if (commitRollback == CommitRoll.commitRollback) EventoEseguitaQuery(Sql.upd);

            return true;
        }

        //Public Function Update(where As String, Optional wherePresente As Boolean = True, Optional ByRef sql As Ogg = Nothing, Optional nuovaConn As NuovaConn = NuovaConn.seNecessario, Optional strConn As StrConnessione = Nothing,
        //                       Optional commitRollback As CommitRoll = CommitRoll.commitRollback, Optional timeOutQuery As Int32 = 0, Optional logMess As Mess = Nothing) As Boolean

        //    If IsNothing(logMess) Then logMess = New Mess(Tipi.err, log.errUserText)
        //    If IsNothing(where) Then where = ""

        //    If wherePresente = True AndAlso where = "" Then 'Serve poichè inibisce la possibilità di eseguire la query senza la parte where 
        //        logMess.testoDaLoggare = "ricevuto 'wherePresente' a true, ma ricevuto 'where' a nothing o vuoto"
        //        log.Acc(New Mess(Tipi.err, log.errUserText, logMess.testoDaLoggare))
        //        Return False
        //    End If

        //    If IsNothing(sql) Then sql = New Ogg

        //    Dim query, qrySet, val As String, col As Colonna

        //    For Each proprietàCol As Reflection.FieldInfo In Me.GetType.GetFields  'Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
        //        If proprietàCol.FieldType<> GetType(Colonna) Then Continue For
        //        col = proprietàCol.GetValue(Me)
        //        If col.ident = True Then Continue For  'Se è identity non va nella Update
        //        val = col.valXqry

        //        If (val = "NULL" OrElse val = "DEFAULT") AndAlso col.valNull = False Then Continue For

        //        qrySet += col.nm & "=" & val & ", "
        //    Next

        //    If ValidaBusinessLogic(qryUpd, logMess:=logMess) = False Then Return False

        //    qrySet = RimuoviFinale(qrySet, ", ")

        //    If where<> "" Then where = " WHERE " & where

        //    query = qryUpd & nmTab & " SET " & qrySet & where

        //    If sql.ExecNoQuery(query, commitRollback:=commitRollback, nuovaConn:=nuovaConn, strConn:=strConn, timeOutQuery:=timeOutQuery, logMess:=logMess) = False Then Return False

        //    If commitRollback = CommitRoll.commitRollback Then EventoEseguitaQuery(qryUpd)

        //    Return True
        //End Function

        public bool Delete(string where, bool wherePresente = true, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
                            Int32 timeOutQuery = 0, Mess logMess = null)
        {
            SqlObj sql = null;
            return Delete(where, wherePresente, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);
        }

        public bool Delete(string where, ref SqlObj sql, bool wherePresente = true, NuovaConn nuovaConn = NuovaConn.seNecessario, StrConnection strConn = null, CommitRoll commitRollback = CommitRoll.commitRollback,
                            Int32 timeOutQuery = 0, Mess logMess = null)
        { return Delete(where, wherePresente, ref sql, nuovaConn, strConn, commitRollback, timeOutQuery, logMess);  }

        public bool Delete(string where, bool wherePresente, ref SqlObj sql, NuovaConn nuovaConn, StrConnection strConn, CommitRoll commitRollback, Int32 timeOutQuery, Mess logMess)
        {

            if (logMess == null) logMess = new Mess(LogType.ERR, Log.main.errUserText);
            if (where == null) where = "";

            if (wherePresente == true && where == "") { //Serve poichè inibisce la possibilità di eseguire la query senza la parte where 
                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "ricevuto 'wherePresente' a true, ma ricevuto 'where' a nothing o vuoto"));
                return false;
                }

            if (sql == null) sql = new SqlObj();

            string query;

            if (where != "") where = " WHERE " + where;

            if (ValidaBusinessLogic(Sql.del, where, logMess: logMess) == false) return false;

            query = Sql.del + Nm + where;

            if (sql.ExecNoQuery(query, commitRollback: commitRollback, nuovaConn: nuovaConn, strConn: strConn, timeOutQuery: timeOutQuery, logMess: logMess) == false) return false;

            if (commitRollback == CommitRoll.commitRollback) EventoEseguitaQuery(Sql.del);

            return true;
        }


        //Public Function Delete(where As String, Optional wherePresente As Boolean = True, Optional ByRef sql As Ogg = Nothing, Optional nuovaConn As NuovaConn = NuovaConn.seNecessario, Optional strConn As StrConnessione = Nothing,
        //                       Optional commitRollback As CommitRoll = CommitRoll.commitRollback, Optional timeOutQuery As Int32 = 0, Optional logMess As Mess = Nothing) As Boolean

        //    If IsNothing(logMess) Then logMess = New Mess(Tipi.err, log.errUserText)
        //    If IsNothing(where) Then where = ""

        //    If wherePresente = True AndAlso where = "" Then 'Serve poichè inibisce la possibilità di eseguire la query senza la parte where 
        //        log.Acc(New Mess(Tipi.err, log.errUserText, "ricevuto 'wherePresente' a true, ma ricevuto 'where' a nothing o vuoto"))
        //        Return False
        //    End If

        //    If IsNothing(sql) Then sql = New Ogg

        //    Dim query As String

        //    If where<> "" Then where = " WHERE " & where

        //    If ValidaBusinessLogic(qryDel, where, logMess:=logMess) = False Then Return False

        //    query = qryDel & nmTab & where

        //    If sql.ExecNoQuery(query, commitRollback:=commitRollback, nuovaConn:=nuovaConn, strConn:=strConn, timeOutQuery:=timeOutQuery, logMess:=logMess) = False Then Return False

        //    If commitRollback = CommitRoll.commitRollback Then EventoEseguitaQuery(qryDel)

        //    Return True
        //End Function

        public virtual bool ValidaBusinessLogic(string tipoQry, string datiDaQuery = "", Mess logMess = null) { //Da sviluppare nella classe della tabella che erediterà da Me (TabellaBase) 
            return true;
        }

        public virtual void EventoEseguitaQuery(string tipoQry) {  //Da sviluppare nella classe della tabella che erediterà da Me (TabellaBase) 
        }

        //Private Sub RicavaWhereDaColonne(ByRef where As String, Optional strConcatena As String = " AND ")
        //    Dim col As Colonna

        //    For Each proprietàCol As Reflection.FieldInfo In Me.GetType.GetFields  'Scorro tutti i field della classe che eredita da TabellaBase alla ricerca delle colonne
        //        If proprietàCol.FieldType <> GetType(Colonna) Then Continue For
        //        col = proprietàCol.GetValue(Me)

        //        If IsNothing(col.val) = True Then Continue For


        //        Select Case col.tipo.GetType
        //            Case GetType(TipiColonna.TinyInt), GetType(TipiColonna.SmallInt), GetType(TipiColonna.Int), GetType(TipiColonna.BigInt), GetType(TipiColonna.Decimal)
        //            Case GetType(TipiColonna.Bit)
        //                'If col.val Then
        //            Case Else
        //                descErr = "tipo valore sconosciuto, col.tipo.GetType:<" & col.tipo.GetType.Name & ">"
        //                log.Acc(New Mess(Tipi.err, log.errUserText, descErr))
        //                Throw New Exception(descErr)
        //        End Select


        //    Next

        //End Sub


    }
}
