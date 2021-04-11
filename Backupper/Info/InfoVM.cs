using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class InfoVM
    {
        private static readonly Lazy<InfoVM> instance = new Lazy<InfoVM>(() => new InfoVM()); //Thread-safe singleton

        public static InfoVM Instance
        {
            get
            {
                return instance.Value;
            }
        }

        private InfoVM() { }
    }
}
