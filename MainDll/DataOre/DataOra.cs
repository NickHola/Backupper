using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Main.DataOre
{
    static public class DataOra //.ToString("yyyy/MM/dd HH:mm:ss.fff")
    {
        public static bool AttesaTempo(ref DateTime oraPrec, UInt64 attesaMs ) {
            DateTime oraAtt; double tTrascorso; //Deve essere Int64 poichè potrebbe essere negativo
            oraAtt = DateTime.Now;
            tTrascorso = (oraAtt - oraPrec).TotalMilliseconds;

            if (tTrascorso >= attesaMs || tTrascorso < -1) { //diff < -1 Significa che ho portato indietro la data
                oraPrec = oraAtt;
                return true;
            }
            Thread.Sleep(1);  //ATTENZIONE non si può mettere un tempo più elevato di un 1ms poichè questa funzione può essere usata su di un IF in OrElse con altre condizioni quindi deve essere veloce
            //C'è anche il discorso che se il PC va in standby la sleep allunga il tempo di dormita poichè il tempo dello standby del PC non viene considerato
            return false;
        }
        public static void SleepConDoEvents(UInt64 attesaMs) {
            DateTime oraAtt = DateTime.Now;
            while (AttesaTempo(ref oraAtt, attesaMs) == false) {
               Util.DoEvents();
            }
        }
        
        //Public Function AttesaTempoDormiente(ByRef oraPrec As DateTime, attesaMs As UInt64) As Boolean
        //    Dim oraAtt As DateTime, tTrascorso, dormiPer As Int64 'Deve essere Int64 poichè potrebbe essere negativo
        //    oraAtt = DateTime.Now
        //    tTrascorso = (oraAtt - oraPrec).TotalMilliseconds
        //    If tTrascorso >= attesaMs OrElse tTrascorso < -1 Then 'diff< -1 Significa che ho portato indietro la data
        //        oraPrec = oraAtt
        //        Return True
        //    End If
        //    dormiPer = (attesaMs - tTrascorso) * 0.9
        //    Threading.Thread.Sleep(dormiPer)
        //    Return False
        //End Function
    }

}
