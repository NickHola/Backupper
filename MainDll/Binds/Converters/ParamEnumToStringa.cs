using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Binds
{
    //Utilizzo: 
    //Se si vogliono modificare i paramtri di default scrive: "<ParamEnumToStringa(lett1Maiu:=False)>" prima di Public Enum
    public class ParamEnumToStringa : Attribute
    {
        public bool Lett1Maiu;

        public ParamEnumToStringa(bool lett1Maiu = true) {
            this.Lett1Maiu = lett1Maiu;
        }
    }
}
