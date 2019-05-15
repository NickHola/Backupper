using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class InfoVM
    {
        static InfoVM instance;

        public static InfoVM Instance
        {
            get
            {
                if (instance == null)
                    instance = new InfoVM();
                return instance;
            }
        }

        private InfoVM() { }
    }
}
