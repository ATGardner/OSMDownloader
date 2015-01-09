using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    public class DownloadTileEventArgs : EventArgs
    {
        public int Current { get; private set; }
        public int Total { get; private set; }

        public DownloadTileEventArgs(int current, int total)
        {
            this.Current = current;
            this.Total = total;
        }
    }
}
