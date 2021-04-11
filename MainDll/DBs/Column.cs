using System;
using System.Collections.Generic;
using System.Linq;
using Main.Logs;

namespace Main.DBs
{
    public class Column
    {
        public readonly TabellaBase Padre;
        public readonly string Nm, NmUtente;
        public readonly object ValPred;
        public readonly ColumnTypes.Base Tipo;
        public readonly bool ValNull, PrimKey, Ident, Univoca; //, colFk As List(Of Colonna)
        private List<Object> value = new List<Object>(); //La val_ deve essere isolata da manipolazioni esterne (i valori devono passare per la 'property val')

        public object Value
        {
            get
            {
                if (value.Count == 0)
                { return null; }
                else
                { return value[0]; }
            }

            set
            { //La Set ha la funzione di rendere il valore ricevuto coerente con il tipo della colonna
                string testoErrTipo, testoErrValDis, testoEcc;

                if (value == null)
                {
                    this.value.Add(null);
                    return;
                }

                testoErrTipo = "per il tipo:<" + Tipo.GetType().Name + ">, ";
                testoErrValDis = "ricevuto valore disatteso, value:<" + value.ToString() + ">, value.GetType:<" + value.GetType().Name + ">";
                testoEcc = ", ex.mess:<{0}>";


                if (Tipo.GetType() == typeof(ColumnTypes.TinyInt))
                {
                    try { value = Convert.ToByte(value); }
                    catch (Exception ex)
                    { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis + String.Format(testoEcc, ex.Message)))); }
                }
                else if (Tipo.GetType() == typeof(ColumnTypes.SmallInt))
                {
                    try { value = Convert.ToInt16(value); }
                    catch (Exception ex)
                    { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis + String.Format(testoEcc, ex.Message)))); }
                }
                else if (Tipo.GetType() == typeof(ColumnTypes.Int))
                {
                    try { value = Convert.ToInt32(value); }
                    catch (Exception ex)
                    { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis + String.Format(testoEcc, ex.Message)))); }

                }
                else if (Tipo.GetType() == typeof(ColumnTypes.BigInt))
                {
                    try { value = Convert.ToInt64(value); }
                    catch (Exception ex)
                    { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis + String.Format(testoEcc, ex.Message)))); }
                }
                else if (Tipo.GetType() == typeof(ColumnTypes.Decimal))
                {
                    try { value = Convert.ToDecimal(value); }
                    catch (Exception ex)
                    { throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis + String.Format(testoEcc, ex.Message)))); }

                }
                else if (Tipo.GetType() == typeof(ColumnTypes.Bit))
                { //In questo caso normalizzo a booleano in true o false
                    if (value.GetType() != typeof(Boolean))
                    {
                        if (value.IsNumeric(ref value))
                        {
                            if ((double)value == 0)
                                value = false;
                            else if ((double)value == 1)
                                value = true;
                            else
                                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + "ricevuto valore numerico ma diverso da 0 o 1")));
                        }
                        else if (value.GetType() == typeof(string))
                        {
                            value = (value as string).Trim();
                            if ((string)value == "false" || (string)value == "0")
                                value = false;
                            else if ((string)value == "true" || (string)value == "1")
                                value = true;
                            else
                                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + "ricevuto valore di tipo stringa ma diverso da 'false', 'true', '0' o '1'")));
                        }
                        else
                        {
                            throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis)));
                        }
                    }
                }
                else if (Tipo.GetType() == typeof(ColumnTypes.DateTime) || Tipo.GetType() == typeof(ColumnTypes.Date))
                {
                    if (value.GetType() == typeof(DateTime)) { } //non faccio nulla, i tipi Date e DateTime sono la stessa identica cosa
                    else if (value.GetType() == typeof(string))
                    {
                        try
                        {
                            value = (DateTime)value; //Non usare TryParse, restituisce comunque eccezione
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + "non sono riuscito a convertire il valore stringa in datetime, value:<" + value.ToString() + ">" + String.Format(testoEcc, ex.Message))));
                        }
                    }
                    else
                    {
                        throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, testoErrTipo + testoErrValDis)));
                    }

                }
                else if (Tipo.GetType() == typeof(ColumnTypes.NVarChar))
                {
                    if (value.GetType() != typeof(string)) value = value.ToString();
                }
                else
                {
                    throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "tipo colonna sconosciuto, tipo.GetType:<" + Tipo.GetType().Name + ">")));
                }
                this.value.Add(value);
            }
        }

        public List<Object> ValList  //ATTENZIONE in questo Get non modificare mai la var val_
        {
            get { return value; }
        }

        public string ValXqry
        {  //ATTENZIONE in questo Get non modificare mai la var val_
            get
            {
                if (value.Count() == 0) return StrXQry(null);
                return StrXQry(value[0]);
            }
        }

        public List<string> ValXqryList
        {  //ATTENZIONE in questo Get non modificare mai la var val_
            get
            {
                List<string> valXqryList = new List<string>();

                if (value.Count() == 0) valXqryList.Add(StrXQry(null));

                foreach (object tmpVal in value)
                {
                    valXqryList.Add(StrXQry(tmpVal));
                }
                return valXqryList;
            }
        }

        public Column(TabellaBase padre, string nome, ColumnTypes.Base tipo, string nomeUtente = "", bool valNull = false, Object valPred = null, bool primKey = false, List<Column> colFk = null, List<Type> tabFk = null,
            bool ident = false, bool univoca = false)
        {
            this.Padre = padre;
            this.Nm = nome;
            this.NmUtente = nomeUtente == "" ? nome : nomeUtente;
            this.Tipo = tipo;
            this.ValPred = valPred;
            this.PrimKey = primKey;
            this.ValNull = valNull;
            //Me.colFk = colFk
            this.Ident = ident;
            this.Univoca = univoca;

            if (colFk != null && colFk.Count > 0) padre.AggiungiAListaFK(this, colFk);
            if (tabFk != null && tabFk.Count > 0) padre.AggiungiAListaFK(this, tabFk);

            try
            {
                if (ident == true) padre.NomeColIdentity = this.Nm;
            }
            catch
            {
                ident = false;
            }
        }

        private string StrXQry(Object valore)
        { //strXQry ha la funzione di trasormare il valore in modo da essere compatibile in una query o noQuery
            if (valore == null)
            {
                if (this.ValPred == null)
                    return "NULL";
                else
                    return "DEFAULT"; //Indica a sql di impostare il valore predefinito
            }
            string str;

            if (Tipo.GetType() == typeof(ColumnTypes.Bit))
                str = Convert.ToByte(valore).ToString(); //Usare Convert.ToByte e non cByte (poichè cByte converte True in 255 invece che 1), Non può andare in eccezione poichè sono sicuro che è un boolean
            else if (Tipo.GetType() == typeof(ColumnTypes.NVarChar))
                str = valore.ToString().Replace("'", "''");
            else if (Tipo.GetType() == typeof(ColumnTypes.TinyInt) || Tipo.GetType() == typeof(ColumnTypes.SmallInt) || Tipo.GetType() == typeof(ColumnTypes.Int) || Tipo.GetType() == typeof(ColumnTypes.BigInt) ||
                        Tipo.GetType() == typeof(ColumnTypes.Decimal) )
                str = valore.ToString();
            else if(Tipo.GetType() == typeof(ColumnTypes.DateTime))
                str = ((DateTime)valore).ToString("yyyy/MM/dd HH:mm:ss.FFF");
            else if (Tipo.GetType() == typeof(ColumnTypes.Date))
                str = ((DateTime)valore).ToString("yyyy/MM/dd");
            else
                throw new Exception(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "tipo valore sconosciuto, tipo.GetType:<" + Tipo.GetType().Name + ">")));
             
            return Tipo.valQryTraApici == true ? "'" + str + "'" : str;
        }

        public void PulisciValori() {
            value.Clear();
        }
    }
}
