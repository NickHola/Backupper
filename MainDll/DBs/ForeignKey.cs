using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.DBs
{
    class ForeignKey
    {
        public readonly string nome, tab, col, tabFk, colFk;

        public ForeignKey(string nome, string tab, string col, string tabFk, string colFk)
        {
            this.nome = nome;
            this.tab = tab;
            this.col = col;
            this.tabFk = tabFk;
            this.colFk = colFk;
        }
    }
}
