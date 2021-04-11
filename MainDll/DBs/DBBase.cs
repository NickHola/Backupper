using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Main.SQLes;
using Main.Serializes;
using Main.Logs;

namespace Main.DBs
{
    public abstract class DBBase
    {
        public StrConnection ConnString { get; set; }

        //*****Dichiarazione oggetti tabella
        public DBTabs.Utenti Utenti { get; set; } = new DBTabs.Utenti();
        public DBTabs.ImpostApp ImpostApp { get; set; } = new DBTabs.ImpostApp();
        public DBTabs.GruppiUtente GruppiUtente { get; set; } = new DBTabs.GruppiUtente();
        public DBTabs.UtentiGruppi UtentiGruppi { get; set; } = new DBTabs.UtentiGruppi();

        protected internal DBBase() { }

        private class AnalisiTabella
        {
            public TabellaBase tabella;
            public List<AnalisiColonna> colonne;
            public List<AnalisiFK> foreignKeys;
            public bool controllato;

            public AnalisiTabella(TabellaBase tabella)
            {
                this.tabella = tabella;
                colonne = new List<AnalisiColonna>();
                foreignKeys = new List<AnalisiFK>();
                controllato = false;
            }
        }

        private class AnalisiColonna
        {
            public readonly Column colonna;
            public bool controllato;
            public AnalisiColonna(Column colonna)
            {
                this.colonna = colonna;
                controllato = false;
            }
        }

        private class AnalisiFK
        {  //Per controllare i FK non posso usare la lista interna a tab poichè non mi posso segnare se è stato controllato 
            public readonly ForeignKey foreignKey;
            public bool controllato;
            public AnalisiFK(ForeignKey foreignKey)
            {
                this.foreignKey = foreignKey;
                controllato = false;
            }
        }

        public bool ControlloStrutturaDB(out bool esito, out List<string> listaErr)
        {
            esito = false;
            listaErr = new List<string>();

            //*****Popolo strutturaInDef
            List<AnalisiTabella> strutturaInDef = new List<AnalisiTabella>();
            AnalisiTabella analisiTabDef; AnalisiColonna analisiColDef; AnalisiFK analisiFK; Type tipoDB, tipoTabella;
            analisiTabDef = null;

            tipoDB = this.GetType(); //Prendo il tipo della classe che eredita da DB

            foreach (PropertyInfo proprietàTab in tipoDB.GetProperties())
            { //Scorro tutti i field della classe che eredita da DB alla ricerca delle tabelle
                if (proprietàTab.PropertyType.IsSubclassOf(typeof(TabellaBase)))
                {
                    //If proprietàTab.FieldType.BaseType = GetType(TabellaBase) Then

                    TabellaBase tab = (TabellaBase)proprietàTab.GetValue(this);
                    analisiTabDef = new AnalisiTabella(tab);
                    tipoTabella = tab.GetType();

                    foreach (ForeignKey FK in tab.listaFK)
                    {
                        analisiFK = new AnalisiFK(FK); //Per controllare i FK non posso usare la lista interna a tab poichè non mi posso segnare se è stato controllato 
                        analisiTabDef.foreignKeys.Add(analisiFK);
                    }

                    foreach (FieldInfo proprietaCampo in tipoTabella.GetFields())
                    {
                        if (proprietaCampo.FieldType == typeof(Column))
                        {
                            Column col = (Column)proprietaCampo.GetValue(tab);
                            if (col == null)
                            {
                                listaErr.Add("In definizione nella tab:<" + tab.Nm + "> è definita la colonna " + proprietaCampo.Name + " ma il valore è null, manca quindi l'instanza della colonna nel costruttore della tabella");
                                continue;
                            }
                            analisiColDef = new AnalisiColonna(col);
                            analisiTabDef.colonne.Add(analisiColDef);
                        }
                    }
                    strutturaInDef.Add(analisiTabDef);
                }
            }


            //*****Leggo le informazioni che mi servono da DB per poter fare il confronto
            SqlObj sql = new SqlObj();
            DataTable sqlTab = new DataTable();
            DataTable sqlCampi = new DataTable();
            DataTable sqlKeysPrim = new DataTable();
            DataTable sqlIdent = new DataTable();
            DataTable sqlFK = new DataTable();
            string nomeTabInSql, nomeColInSql, testoErrTab, testoErrCol;
            ColumnTypes.Base tipoColInSql;


            if (sql.ExecQuery(Sql.sel + "table_name FROM information_schema.tables", res: ref sqlTab) == false) return false; //Recupera sia tabelle che viste

            if (sql.ExecQuery(Sql.sel + "* FROM information_schema.columns", res: ref sqlCampi) == false) return false;

            if (sql.ExecQuery(Sql.sel + "ku.table_name AS tab, ku.column_name AS col " +
                                "FROM information_schema.table_constraints AS tc " +
                                "INNER JOIN information_schema.key_column_usage AS ku ON tc.constraint_type = 'PRIMARY KEY' AND tc.constraint_name = ku.constraint_name", res: ref sqlKeysPrim) == false) return false;

            if (sql.ExecQuery(Sql.sel + "obj.name AS tab, ident.name AS col FROM sys.identity_columns AS ident " +
                                "INNER JOIN sys.objects as obj ON obj.object_id = ident.object_id " +
                                "where obj.type_desc != 'INTERNAL_TABLE'", res: ref sqlIdent) == false) return false;

            if (sql.ExecQuery(Sql.sel + "obj.name AS nome, tab1.name AS tab, col1.name AS col, tab2.name AS tabFk, col2.name AS colFk " +
                                "FROM sys.foreign_key_columns fkc " +
                                "INNER JOIN sys.objects obj ON obj.object_id = fkc.constraint_object_id " +
                                "INNER JOIN sys.tables tab1 ON tab1.object_id = fkc.parent_object_id " +
                                "INNER JOIN sys.columns col1 ON col1.column_id = parent_column_id And col1.object_id = tab1.object_id " +
                                "INNER JOIN sys.tables tab2 ON tab2.object_id = fkc.referenced_object_id " +
                                "INNER JOIN sys.columns col2 ON col2.column_id = fkc.referenced_column_id AND col2.object_id = tab2.object_id ", res: ref sqlFK) == false) return false;


            foreach (DataRow tabInSql in sqlTab.Rows)
            {

                nomeTabInSql = (string)tabInSql["TABLE_NAME"];

                //*****verifica esistenza tabella in definizione
                var listaTabInDef = from tmp in strutturaInDef where tmp.tabella.Nm == nomeTabInSql select tmp;

                if (listaTabInDef.Count() == 0)
                {
                    listaErr.Add("In sql è presente la tabella:<" + nomeTabInSql + "> non presente in definizione");
                    continue;
                }
                else if (listaTabInDef.Count() > 1)
                {
                    listaErr.Add("In definizione c'è più di una tabella con il nome:<" + nomeTabInSql + ">");
                    continue;
                }
                else
                {
                    analisiTabDef = listaTabInDef.ElementAt(0);
                }

                testoErrTab = "per la tabella:<" + nomeTabInSql + ">";


                foreach (DataRow colInSql in sqlCampi.Select("TABLE_NAME='" + nomeTabInSql + "'"))
                {

                    nomeColInSql = (string)colInSql["COLUMN_NAME"];
                    testoErrCol = " la colonna:<" + nomeColInSql + ">";

                    //*****verifica esistenza colonne in definizione

                    var listaColInDef = from tmp in analisiTabDef.colonne where tmp.colonna.Nm == nomeColInSql select tmp;
                    Column colInDef;

                    if (listaColInDef.Count() == 0)
                    {
                        listaErr.Add("In sql " + testoErrTab + " è presente" + testoErrCol + ", non presente in definizione");
                        continue;
                    }
                    else if (listaTabInDef.Count() > 1)
                    {
                        listaErr.Add("In definizione " + testoErrTab + " c'è più di una colonna con il nome:<" + nomeColInSql + ">");
                        continue;
                    }
                    else
                    {
                        colInDef = listaColInDef.ElementAt(0).colonna;
                    }


                    switch (colInSql["IS_NULLABLE"])
                    {
                        case "NO":
                            if (colInDef.ValNull == true) listaErr.Add("In sql " + testoErrTab + testoErrCol + " non può essere null, in definizione si"); //Non serve Continue For cos' controllo anche altri errori
                            break;
                        case "YES":
                            if (colInDef.ValNull == false) listaErr.Add("In sql " + testoErrTab + testoErrCol + " può essere null, in definizione no"); //Non serve Continue For cos' controllo anche altri errori
                            break;
                        default:
                            Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " ha il valore IS_NULLABLE sconosciuto, valore:<" + colInSql["IS_NULLABLE"].ToString() + ">"));
                            continue;
                    }


                    if (Convert.IsDBNull(colInSql["COLUMN_DEFAULT"]))
                    {
                        if (colInDef.ValPred != null) listaErr.Add("In sql " + testoErrTab + testoErrCol + " non ha valore predefinito, in definizione si, colInDef.valPred:<" + colInDef.ValPred + ">"); //Non serve Continue For cos' controllo anche altri errori
                    }
                    else
                    {
                        if (colInDef.ValPred == null) listaErr.Add("In sql " + testoErrTab + testoErrCol + " ha valore predefinito, in definizione no, COLUMN_DEFAULT:<" + colInSql["COLUMN_DEFAULT"].ToString() + ">"); //Non serve Continue For cos' controllo anche altri errori

                        if (Regex.IsMatch((string)colInSql["COLUMN_DEFAULT"], @"^((\(\(){1}[0-9]+(\)\)){1}|(\('){1}[\s\S]*('\)){1})$"))
                        { //Regex: ((num)) or ('str') , str può essere 0 caratteri num no  
                            string valPredInSql = ((string)colInSql["COLUMN_DEFAULT"]).Right(-2).Left(-2);
                            if (valPredInSql != (string)colInDef.ValPred) listaErr.Add("In sql " + testoErrTab + testoErrCol + " ha valore predefinito:<" + valPredInSql + ">, in definizione:<" + colInDef.ValPred + ">"); //Non serve Continue For cos' controllo anche altri errori
                        }
                        else
                        {
                            Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " ha il valore COLUMN_DEFAULT con formato sconosciuto, valore:<" + colInSql["COLUMN_DEFAULT"].ToString() + ">"));
                            continue;
                        }
                    }

                    if (sqlKeysPrim.Select("tab='" + nomeTabInSql + "' AND col='" + nomeColInSql + "'").Count() == 1)
                    {
                        if (colInDef.PrimKey == false) listaErr.Add("In sql " + testoErrTab + testoErrCol + " è chiave primaria, in definizione no"); //Non serve Continue For cos' controllo anche altri errori
                    }
                    else
                    {
                        if (colInDef.PrimKey == true) listaErr.Add("In sql " + testoErrTab + testoErrCol + " non è chiave primaria, in definizione si"); //Non serve Continue For cos' controllo anche altri errori
                    }


                    //*****verifica del tipo di colonna
                    switch (colInSql["DATA_TYPE"].ToString().ToLower())
                    { //I tipi che hanno un costruttore devono essere trattati singolarmente
                        case "nvarchar":
                            if (Convert.IsDBNull(colInSql["CHARACTER_MAXIMUM_LENGTH"]))
                            {   //se è NVarChar(MAX) la colonna CHARACTER_MAXIMUM_LENGTH vale -1 
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo NVarChar ma il valore di CHARACTER_MAXIMUM_LENGTH è null"));
                                continue;
                            }
                            else if (colInSql["CHARACTER_MAXIMUM_LENGTH"].IsNumeric() == false)
                            {
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo NVarChar ma il valore di CHARACTER_MAXIMUM_LENGTH non è numerico, valore:<" + colInSql["CHARACTER_MAXIMUM_LENGTH"].ToString() + ">"));
                                continue;
                            }
                            else if (Num.ContieneCaratNum(colInSql["CHARACTER_MAXIMUM_LENGTH"].ToString(), ctrlSegno: false))
                            {
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo NVarChar ma il valore di CHARACTER_MAXIMUM_LENGTH contiene caratteri disattesi, valore:<" + colInSql["CHARACTER_MAXIMUM_LENGTH"].ToString() + ">"));
                                continue;
                            }
                            else if (colInSql["CHARACTER_MAXIMUM_LENGTH"].ToString() == "-1")
                            { //-1 è NVarChar(MAX)
                                tipoColInSql = new ColumnTypes.NVarChar("MAX");
                            }
                            else
                            {
                                tipoColInSql = new ColumnTypes.NVarChar(colInSql["CHARACTER_MAXIMUM_LENGTH"].ToString());
                            }
                            break;

                        case "decimal":

                            if (Convert.IsDBNull(colInSql["NUMERIC_PRECISION"]) || Convert.IsDBNull(colInSql["NUMERIC_SCALE"]))
                            {
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo Decimal ma il valore di NUMERIC_PRECISION o NUMERIC_SCALE è null, NUMERIC_PRECISION:<" + colInSql["NUMERIC_PRECISION"].ToString() + ">, NUMERIC_SCALE:<" + colInSql["NUMERIC_SCALE"].ToString() + ">"));
                                continue;
                            }
                            else if (colInSql["NUMERIC_PRECISION"].IsNumeric() == false || colInSql["NUMERIC_SCALE"].IsNumeric() == false)
                            {
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo Decimal ma il valore di NUMERIC_PRECISION o NUMERIC_SCALE non è numerico, NUMERIC_PRECISION:<" + colInSql["NUMERIC_PRECISION"].ToString() + ">, NUMERIC_SCALE:<" + colInSql["NUMERIC_SCALE"].ToString() + ">"));
                                continue;
                            }
                            else if (Num.ContieneCaratNum(colInSql["NUMERIC_PRECISION"].ToString()) || Num.ContieneCaratNum(colInSql["NUMERIC_SCALE"].ToString()))
                            {
                                Log.main.Add(new Mess(LogType.ERR, Log.main.errUserText, "In sql " + testoErrTab + testoErrCol + " è di tipo Decimal ma il valore di NUMERIC_PRECISION o NUMERIC_SCALE contiene caratteri disattesi, NUMERIC_PRECISION:<" + colInSql["NUMERIC_PRECISION"].ToString() + ">, NUMERIC_SCALE:<" + colInSql["NUMERIC_SCALE"].ToString() + ">"));
                                continue;
                            }
                            else
                            {
                                tipoColInSql = new ColumnTypes.Decimal(Convert.ToUInt32(colInSql["NUMERIC_PRECISION"]), Convert.ToUInt32(colInSql["NUMERIC_SCALE"]));
                            }
                            break;

                        default:  //Tipo di colonna senza costruttore
                            var tipo = from tmp in Assembly.GetExecutingAssembly().GetTypes()
                                       where tmp.IsClass == true && tmp.BaseType == typeof(ColumnTypes.Base) && tmp.Name.ToLower() == (string)colInSql["DATA_TYPE"].ToString().ToLower()
                                       select tmp;

                            if (tipo.Count() == 0)
                            {
                                listaErr.Add("In sql " + testoErrTab + testoErrCol + " è di tipo sconoscito, DATA_TYPE:<" + colInSql["DATA_TYPE"].ToString() + ">");
                                continue;
                            }
                            tipoColInSql = (ColumnTypes.Base)Activator.CreateInstance(tipo.ElementAt(0));
                            break;
                    }

                    if (tipoColInSql.GetType() != colInDef.Tipo.GetType())
                    {
                        listaErr.Add("In sql " + testoErrTab + testoErrCol + " ha il tipo è diverso alla definizione, tipoColInSql:<" + tipoColInSql.GetType().Name + ">, colInDef:<" + colInDef.Tipo.GetType().Name + ">");
                    }
                    else
                    {  //Se sono dello stesso tipo controllo il contenuto (le variabili interne)
                        string serialColInSql, serialColInDef;
                        serialColInSql = serialColInDef = "";
                        if (Serialize.SerializeInText(tipoColInSql, ref serialColInSql) == false || Serialize.SerializeInText(colInDef.Tipo, ref serialColInDef) == false) continue;

                        if (serialColInSql.ToLower() != serialColInDef.ToLower()) listaErr.Add("In sql " + testoErrTab + testoErrCol + " ha il contenuto è diverso dalla definizione" + Util.crLf + Util.crLf +
                                                                            "serialColInSql:<" + serialColInSql + ">" + Util.crLf + Util.crLf +
                                                                            "serialColInDef:<" + serialColInDef + ">");
                    }
                    listaColInDef.ElementAt(0).controllato = true;
                }


                //*****Verifica del campo Identity della tabella
                DataRow[] colIdentInSql = sqlIdent.Select("tab='" + nomeTabInSql + "'");
                string colIdentInDef = analisiTabDef.tabella.NomeColIdentity;

                if (colIdentInSql.Count() == 1)
                {
                    if (colIdentInSql[0]["col"].ToString() != colIdentInDef) listaErr.Add("In sql " + testoErrTab + " la colonna identity è diversa da quella in definizione, colInSql:<" + colIdentInSql[0]["col"].ToString() + ">, colInDef:<" + colIdentInDef + ">");
                }
                else
                {
                    if (colIdentInDef != "") listaErr.Add("In sql " + testoErrTab + " non c'è la colonna identity, in definizione si, nome colonna:<" + colIdentInDef + ">");
                }


                //*****Verifica delle foreign key

                foreach (DataRow fkInSql in sqlFK.Select("tab='" + nomeTabInSql + "'"))
                {

                    var listaFkInDef = from tmp in analisiTabDef.foreignKeys where tmp.foreignKey.nome == fkInSql["nome"].ToString() select tmp;
                    ForeignKey fkInDef; string testoErrFk;

                    testoErrFk = " la foreign key:<" + fkInSql["nome"] + ">";

                    if (listaFkInDef.Count() == 0)
                    {
                        listaErr.Add("In sql " + testoErrTab + " c'è" + testoErrFk + ", in definizione no");
                        continue;
                    }

                    fkInDef = listaFkInDef.ElementAt(0).foreignKey;

                    if (fkInDef.tab != fkInSql["tab"].ToString()) listaErr.Add("In sql " + testoErrTab + testoErrFk + " ha come tabella:<" + fkInSql["tab"] + "> mentre in definizione è:<" + fkInDef.tab + ">");

                    if (fkInDef.col != fkInSql["col"].ToString()) listaErr.Add("In sql " + testoErrTab + testoErrFk + " ha come colonna:<" + fkInSql["col"] + "> mentre in definizione è:<" + fkInDef.col + ">");

                    if (fkInDef.tabFk != fkInSql["tabFk"].ToString()) listaErr.Add("In sql " + testoErrTab + testoErrFk + " ha come tabellaFk:<" + fkInSql["tabFk"] + "> mentre in definizione è:<" + fkInDef.tabFk + ">");

                    if (fkInDef.colFk != fkInSql["colFk"].ToString()) listaErr.Add("In sql " + testoErrTab + testoErrFk + " ha come colonnaFk:<" + fkInSql["colFk"] + "> mentre in definizione è:<" + fkInDef.colFk + ">");

                    listaFkInDef.ElementAt(0).controllato = true;
                }

                analisiTabDef.controllato = true;
            }

            //*****Ricerca dei non controllati (Significa presenti in definizione ma non presenti in sql)
            foreach (AnalisiTabella analisiTabInDef in strutturaInDef)
            {
                if (analisiTabInDef.controllato == false)
                {
                    listaErr.Add("In sql la tabella:<" + analisiTabInDef.tabella.Nm + "> non esiste");
                    continue;
                }

                foreach (AnalisiColonna analisiColInDef in analisiTabInDef.colonne)
                {
                    if (analisiColInDef.controllato == false) listaErr.Add("In sql nella tabella:<" + analisiTabInDef.tabella.Nm + "> la colonna:<" + analisiColInDef.colonna.Nm + "> non esiste");
                }

                foreach (AnalisiFK analisiFKInDef in analisiTabInDef.foreignKeys)
                {
                    if (analisiFKInDef.controllato == false) listaErr.Add("In sql nella tabella:<" + analisiTabInDef.tabella.Nm + "> il foreign key:<" + analisiFKInDef.foreignKey.nome + "> non esiste");
                }
            }
            if (listaErr.Count() == 0) esito = true;

            return true;
        }
    }
}
