using Main.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Main.Binds
{
    //Utilizzo: 
    //Scrivere "[TypeConverter(Typeof(EnumToStringa))]" prima di Public Enum
    //Se si vogliono modificare i paramtri di default scrive: "[ParamEnumToStringa(lett1Maiu: false)]" prima di Public Enum
    //Se si vuole associare un scritta diversa ad una singola voce dell'enum scrivere : <Description("XX XXX...")> prima della singola voce
    public class EnumToString : EnumConverter
    {
        public EnumToString(Type type) : base(type) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            string valore, stringaFormattata; bool letteraPrecMaiu;
            stringaFormattata = "";
            ParamEnumToStringa attributo = new ParamEnumToStringa(); //La new serve così ce l'ho inizializzato
            if (value.GetType().IsEnum == false) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "value non è di tipo IEnumerable")));

            if (destinationType != typeof(string) && destinationType != typeof(object)) throw new InvalidOperationException(Excep.ScriviLogInEx(new Mess(LogType.ERR, Log.main.errUserText, "The target must be a boolean")));

            object[] tmpAttr = value.GetType().GetCustomAttributes(typeof(ParamEnumToStringa), false);
            if (tmpAttr.Count() > 0) attributo = (ParamEnumToStringa)tmpAttr[0];

            if (LeggiDescrizioneVoceEnum(value, destinationType, ref stringaFormattata) == true) return stringaFormattata; //Se c'è una descrizione dell'enum ritorno quello altrimenti ritorno la scritta dell'enum formattata

            valore = value.ToString(); //Bisogna fare il ToString altrimenti l'enum restituisce il numero e non la scritta

            //****1° lettera
            if (attributo.Lett1Maiu == false) {
                stringaFormattata = Convert.ToString(System.Char.ToLower(valore[0]));
                letteraPrecMaiu = false;
            } else {
                stringaFormattata = Convert.ToString(System.Char.ToUpper(valore[0]));
                letteraPrecMaiu = true;
            }

            //****Dalla 2° lettera in poi, verifico anche gli acronimi come WPF che devono rimanere maiuscoli
            for (int i = 1; i < valore.Length; i++) {
                if (System.Char.IsUpper(valore[i])) {

                    if (i + 1 <= valore.Length - 1) { //Se dopo c'è ancora un carattere(lo verifico poichè se la scritta è "CiaoWPFVaiAllaGrande" deve diventare "Ciao WPF vai alla grande")
                        if (System.Char.IsUpper(valore[i + 1]) || valore[i + 1].IsNumeric())
                            stringaFormattata += letteraPrecMaiu == true ? Convert.ToString(valore[i]) : " " + valore[i];
                        else
                            stringaFormattata += " " + Char.ToLower(valore[i]);

                    } else { //Dopo non c'è alcuna lettera
                        stringaFormattata += letteraPrecMaiu == true ? Convert.ToString(valore[i]) : " " + Char.ToLower(valore[i]);
                    }
                    letteraPrecMaiu = true;
                } else {
                    stringaFormattata += valore[i];
                    letteraPrecMaiu = false;
                }
            }

            return stringaFormattata;
        }

        private bool LeggiDescrizioneVoceEnum(object value, Type destinationType, ref string descr) {

            if (value == null) return false;

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null) return false;

            DescriptionAttribute[] attributes = (DescriptionAttribute[])Convert.ChangeType(fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false), typeof(DescriptionAttribute[]));

            if (attributes.Length == 0 || attributes[0].Description == null || attributes[0].Description == "") return false;

            descr = attributes[0].Description;
            return true;
        }

        
#region "Metodo originale DammiDescrizioneVoceEnum da cui sono partito, uso e definizione"
        //****Uso: scrivere '<TypeConverter(GetType(EnumDescriptionTypeConverter))> //prima di Public Enum XXXXXX
        //****scrivere <Description("XXXXXX")>  prima di ogni valore di enum

        //Public Class EnumDescriptionTypeConverter
        //    Inherits EnumConverter

        //    Public Sub New(ByVal type As Type)
        //        MyBase.New(type)
        //    End Sub

        //    Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
        //        If destinationType = GetType(String) Then
        //            If IsNothing(value) = False Then
        //                Dim fi As FieldInfo = value.GetType.GetField(value.ToString) 'serve Imports System.Reflection
        //                If IsNothing(fi) = False Then
        //                    Dim attributes = CType(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
        //                    Return If(attributes.Length > 0 AndAlso String.IsNullOrEmpty(attributes(0).Description) = False, attributes(0).Description, value.ToString())
        //                End If
        //            End If

        //            Return String.Empty
        //        End If

        //        Return MyBase.ConvertTo(context, culture, value, destinationType)
        //    End Function
        //End Class
#endregion
    }
}
