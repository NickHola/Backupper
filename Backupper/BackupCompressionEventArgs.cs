using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backupper
{
    public class BackupCompressionResult : EventArgs
    {
        public bool compressionResult, filesAreChanged;
        public int filesToBackup;

        public BackupCompressionResult(bool compressionResult = false, bool filesAreChanged = false, int filesToBackup = 0)
        {
            this.compressionResult = compressionResult;
            this.filesAreChanged = filesAreChanged;
            this.filesToBackup = filesToBackup;
        }
    }
}
