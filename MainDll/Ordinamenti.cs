using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class Ordinamenti {
        public string nomeProprietà;
        public ListSortDirection direzione; 

        public Ordinamenti(string nomeProprietà, ListSortDirection direzione) {
            this.nomeProprietà = nomeProprietà;
            this.direzione = direzione;
        }
    }
}
