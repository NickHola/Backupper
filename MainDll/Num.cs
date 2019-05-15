using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public static class Num
    {
        public static bool ContieneCaratNum(string numero, bool ctrlVirgola = true, bool ctrlSegno = true) {
            if (ctrlVirgola == true && (numero.Contains(".") || numero.Contains(","))) return true;
            if (ctrlSegno == true && numero.Contains("-")) return true;
            return false;
        }
    }
}
