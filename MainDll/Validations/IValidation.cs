using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Main.Validations
{
    public interface IValidation
    {
        [JsonIgnore] bool IsValid { get; }

        /// <summary>
        /// </summary>
        /// <param name="nomeProp">se valorizzato con il nome della proprietà, viene effettuata la validazione solmente sul valore di quella proprietà</param>
        /// <returns></returns>
        ValidationResult ValidMySelf(string nomeProp = "");
        //Dim esito As Boolean, descErr As String
        //esito = True

        //If nomeProp = "" OrElse nomeProp = "prop1" Then
        //    If Me.prop1 = xxx Then
        //        esito = False
        //        descErr = "'Todo"
        //    End If
        //End If    

        ////If nomeProp = "" Then 'Validazione della riga nel caso in cui bisogna verificare il valore di più di una prorietà contemporaneamente
        ////    If Me.prop1 = xxx AndAlso Me.prop2 = xxx Then
        ////        esito = False
        ////        descErr = "'Todo"
        ////    End If
        ////End If

        //If nomeProp = "" Then
        //    Me.isValid = esito
        //Else
        //    If esito = False Then Me.isValid = esito
        //End If

        //Return New ValidationResult(esito, descErr)
    }

}
