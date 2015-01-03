using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    public class DownloadTileEventArgs : EventArgs
    {
        public int ProgressPercentage { get; private set; }

        public DownloadTileEventArgs(int progressPercentage)
        {
            this.ProgressPercentage = progressPercentage;
        }
    }
}
