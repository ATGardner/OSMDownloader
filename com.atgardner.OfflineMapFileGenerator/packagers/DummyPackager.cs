namespace com.atgardner.OMFG.packagers
{
    using com.atgardner.OMFG.tiles;
    using System;
    using System.Threading.Tasks;
using System.Windows.Forms;

    class DummyPackager : IPackager
    {
        private readonly ToolStripStatusLabel lblStatus;
        private int total;
        private int fromCache;

        public DummyPackager(ToolStripStatusLabel lblStatus)
        {
            total = 0;
            fromCache = 0;
            this.lblStatus = lblStatus;
        }

        public async Task Init()
        {
            // do nothing
        }

        public async Task AddTile(Tile tile)
        {
            //Console.WriteLine(tile);
            total++;
            if (tile.FromCache)
            {
                fromCache++;
            }
        }

        public void Dispose()
        {
            UpdateStatus(string.Format("Tiles in cache: {0}, Tiles to download: {1}, Total tiles: {2}", fromCache, total - fromCache, total));
        }

        private void UpdateStatus(string status)
        {
            lblStatus.Owner.BeginInvoke((MethodInvoker)(() => lblStatus.Text = status));
        }
    }
}
