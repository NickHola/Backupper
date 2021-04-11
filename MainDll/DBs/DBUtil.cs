using System;
using System.Threading;
using System.Diagnostics;
using Main.SQLes;
using Main.Logs;

namespace Main.DBs
{
    public static class DBUtil
    {
        
        public static void thrCycle_LockedQryKiller()
        {
            SqlObj sql = new SqlObj(); string query, queryBloccante; DateTime oraXatt; UInt64 procIdToKill, durataProc; ProcessStartInfo cmdShell;
            queryBloccante = "";
            oraXatt = DateTime.Now;

            //// QUERY COMPLETA
            //query = qrySel & "blocking_session_id AS BlockingSessionID, session_id AS VictimSessionID, " &
            //        " (SELECT [text] FROM sys.sysprocesses CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE spid = blocking_session_id) AS BlockingQuery, " &
            //        " [text] AS VictimQuery, wait_time AS WaitDurationMs, wait_type AS WaitType, percent_complete AS BlockingQueryCompletePercent " &
            //        " FROM sys.dm_exec_requests CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE blocking_session_id > 0 AND " &
            //        " blocking_session_id NOT IN (Select session_id FROM sys.dm_exec_requests CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE blocking_session_id > 0)"

            // QUERY CON I SOLI CAMPI CHE MI INTERESSANO
            query = Sql.sel + "blocking_session_id AS BlockingSessionID, (SELECT [text] FROM sys.sysprocesses CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE spid = blocking_session_id) AS BlockingQuery, " +
                " wait_time AS WaitDurationMs FROM sys.dm_exec_requests CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE blocking_session_id > 0 AND " +
                " blocking_session_id NOT IN (Select session_id FROM sys.dm_exec_requests CROSS APPLY sys.dm_exec_sql_text([sql_handle]) WHERE blocking_session_id > 0)";

            //la WHERE blocking_session_id NOT IN... è stata messa poichè se una query blocca una tabella le altre query saranno tutte vittime della prima, poichè è la prima che blocca tutte le altre in cascata
            //Come dimostra l'esempio sotto stante

            //BlockingSessionID()        VictimSessionID         BlockingQuery                                               VictimQuery
            //54	                            57	                INSERT INTO xxx (idRnd) VALUES ('12345')	                SELECT * FROM excDispositivi
            //57	                            62	                SELECT * FROM excDispositivi	                            SELECT * FROM [excDispositivi] WHERE idRnd=1

            while (true)
            {
#if DEBUG == false
                try { 
#endif

                //If AttesaTempoDormiente(oraXatt, 500) = False Then Continue While
                Thread.Sleep(500);

                if (App.Config.MainDbConnString.KillQryBloccanteMs < 1) continue;

                if (sql.ExecQuery(query, @out: QryOut.dataReader, strConn: App.Config.MainDbConnString) == false) continue; //qui posso usare il dataReader tanto sfrutto sempre la stessa connessione senza chiuderla

                while (sql.ResOle.Read())
                {

                    if (Convert.IsDBNull(sql.ResOle["BlockingSessionID"]) == true) continue;
                    procIdToKill = (UInt64)sql.ResOle["BlockingSessionID"];

                    if (Convert.IsDBNull(sql.ResOle["WaitDurationMs"]) == true) continue;
                    durataProc = (UInt64)sql.ResOle["WaitDurationMs"];

                    queryBloccante = "";
                    if (Convert.IsDBNull(sql.ResOle["BlockingQuery"]) == false) queryBloccante = (string)sql.ResOle["BlockingQuery"];

                    if (durataProc >= App.Config.MainDbConnString.KillQryBloccanteMs)
                    {

                        Log.main.Add(new Mess(LogType.Warn, "", "query bloccante sarà terminata, id:<" + procIdToKill + ">, query:<" + queryBloccante + ">"));

                        //è stato usato una shell CMD poichè l'istruzione kill se lanciata direttamente de questo programma tramite una semplice query o una stored procedure generava errore(come se non si avessero i diritti)
                        //invece se viene lanciata da un cmdShell(soluzione attuale) oppure da un .bat(soluzione adottata precedentemente) va a buon fine.

                        var mainDb = App.Config.MainDbConnString;

                        if (mainDb.TrustedConn == false)
                        {
                            cmdShell = new ProcessStartInfo("cmd", String.Format("/k {0} & {1}", "osql -S " + mainDb.IndirServer + " -U " + mainDb.UserId + " -P " + mainDb.Password + " -d " + mainDb.Database + @" -Q ""exec dbo.KILLSPID " + procIdToKill + @"""", "exit"));
                        }
                        else
                        {
                            cmdShell = new ProcessStartInfo("cmd", String.Format("/k {0} & {1}", "osql -S " + mainDb.IndirServer + " -E -d " + mainDb.Database + @" -Q ""exec dbo.KILLSPID " + procIdToKill + @"""", "exit"));
                        }

                        cmdShell.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(cmdShell);
                    }
                }


#if DEBUG == false
            } catch (Exception ex) {
                    Thrs.Thr.NotificaErrThrCiclo(ex, true);
                }
#endif
            }

        }
    }
}
