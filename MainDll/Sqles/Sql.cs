using System;
using System.Data;

namespace Main.SQLes
{
    public static class Sql
    {
        internal static ConfigSql config = new ConfigSql();
        public const string sel = "SELECT ";
        public const string ins = "INSERT INTO ";
        public const string upd = "UPDATE ";
        public const string del = "DELETE FROM ";

        public static string WhereFormat(object valore)
        {
            if (valore == null) return " IS NULL ";

            string valFormattato;

            if (valore.GetType().Equals(typeof(DateTime)))
                valore = ((DateTime)valore).ToString("yyyy/MM/dd HH:mm:ss.FFF");

            if (valore.GetType().Equals(typeof(string)))
            { valFormattato = "'" + ((string)valore).Replace("'", "''") + "'"; }
            else
            { valFormattato = (string)valore; }

            return "=" + valFormattato;
        }
    }


}
