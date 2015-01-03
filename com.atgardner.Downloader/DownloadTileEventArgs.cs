using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    public class DownloadTileEventArgs : EventArgs
    {
        public Tile Tile { get; private set; }
        public DownloadPhase Phase { get; private set; }

        public DownloadTileEventArgs(Tile tile, DownloadPhase phase)
        {
            this.Tile = tile;
            this.Phase = phase;
        }
    }
}
