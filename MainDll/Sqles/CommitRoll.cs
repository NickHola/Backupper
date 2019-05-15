using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.SQLes
{
    [Serializable]
    public enum CommitRoll
    {
        commitRollback,
        soloRollback,
        nessuno
    }
}
