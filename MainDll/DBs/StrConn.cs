using System;
using System.Linq;
using Main.Logs;
using Main.SQLes;

namespace Main.DBs
{
    [Serializable] public class StrConnection
    {
        string completa;
        public bool DbRemoto;
        [NonSerialized()] public bool IsInizializzata, TrustedConn;
        [NonSerialized()] public string Provider, IndirServer, Ip_Host, Database, UserId, Password, Porta;
        public Int16 TimeOutConnMs;
        public UInt16 KillQryBloccanteMs;

        public string Completa
        {
            get { return completa; }
            set
            {
                IsInizializzata = false;
                completa = value;
            }
        }

        internal StrConnection(string strCompleta = "", bool dbRemoto = false, Int16 timeOutConnMs = -1, UInt16 killQryBloccanteMs = 0)
        { //Prima di DB remoto
            if (strCompleta != "")
            {
                Completa = strCompleta;
                InizializzaSingoliCampi();
            }
            else
            { PulisciCampi(); }

            this.DbRemoto = dbRemoto;
            this.TimeOutConnMs = timeOutConnMs;
            this.KillQryBloccanteMs = killQryBloccanteMs;
        }

        public bool InizializzaSingoliCampi()
        {

            string[] parametriValori, parametroValore, ipHostENomeServer;
            string prefissoErrLog;
            this.IsInizializzata = false;
            prefissoErrLog = "Nella stringa di conn. strCompleta:<" + this.Completa + ">, ";

            parametriValori = this.Completa.Split(';');

            if (parametriValori.Count() < 4)
            { //Ci deve essere almeno: Provider=SQL Server Native Client XX.X;Server=XXX\XXX,XX;Database=XXX;Trusted_Connection=Yes;
                Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "ci sono meno di 4 parametri"));
                return false;
            }

            foreach (string elemento in parametriValori)
            {
                parametroValore = elemento.Trim().Split('=');

                if (parametroValore.Count() > 2)
                {
                    for (int i = 2; i <= parametroValore.Count() - 1; i++)
                    {
                        parametroValore[1] += parametroValore[i];
                    }
                }

                if (parametroValore.Count() == 0) continue;

                switch (parametroValore[0].ToLower().Trim())
                {
                    case "server":
                        this.IndirServer = parametroValore[1].Trim(); //NOME SERVER SQL COMPLETO PER ESEMPIO: 192.168.2.19\SQL2012EXP,1433

                        if (this.IndirServer == "")
                        {
                            Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "manca il nome del server sql"));
                            return false;
                        }
                        ipHostENomeServer = this.IndirServer.Split('\\');

                        if (ipHostENomeServer.Count() < 2)
                        {
                            Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "nella parte server manca il simbolo '\'"));
                            return false;
                        }

                        this.Ip_Host = ipHostENomeServer[0];

                        if (this.Ip_Host == "")
                        {
                            Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "manca il nome IP o Host"));
                            return false;
                        }

                        ipHostENomeServer = this.IndirServer.Split(',');

                        if (ipHostENomeServer[ipHostENomeServer.Count() - 1].IsNumeric() == true)
                        { this.Porta = ipHostENomeServer[ipHostENomeServer.Count() - 1]; }
                        else
                        { this.Porta = ""; }

                        break;

                    case "database":
                        this.Database = parametroValore[1].Trim();

                        if (this.Database == "")
                        {
                            Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "manca il nome del database sql"));
                            return false;
                        }
                        break;

                    case "user id":
                        this.UserId = parametroValore[1].Trim();
                        break;

                    case "password":
                        this.Password = parametroValore[1].Trim();
                        break;

                    case "trusted_connection":
                        if (parametroValore[1].ToLower().Trim() == "yes")
                        { this.TrustedConn = true; }
                        else
                        { this.TrustedConn = false; }
                        break;
                }
            }

            if (this.TrustedConn == false) {
                if (this.UserId == "") {
                    Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "manca lo userId per l'accesso al DB"));
                    return false;
                }

                if (this.Password == "") {
                    Log.main.Add(new Mess(LogType.ERR, "", prefissoErrLog + "manca la password per l'accesso al DB"));
                    return false;
                }
            }
            this.IsInizializzata = true;
            return true;
        }

        public void PulisciCampi() {
            IsInizializzata = false;
            Provider = "";
            IndirServer = "";
            Ip_Host = "";
            Database = "";
            UserId = "";
            Password = "";
            Porta = "";
            TrustedConn = false;
        }

        public bool ConnectionTest()
        {
            SqlObj sql = new SqlObj(this);
            return sql.ConnettiDB();
        }
    }
}
