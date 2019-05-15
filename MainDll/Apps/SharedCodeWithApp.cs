using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Apps
{
    public abstract class SharedCodeWithApp
    {
        public abstract string GetAppName();
        public abstract string GetAppVersion();
        public abstract void ExitProcedure(bool saveConfigurations);
        //public abstract void AppExit();
    }
}
