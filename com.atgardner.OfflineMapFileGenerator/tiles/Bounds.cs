namespace com.atgardner.OMFG.tiles
{
    using Gavaghan.Geodesy;
    using System;

    public class Bounds
    {
        private readonly int zoom;
        private Tile tl;
        private Tile br;
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;

        public int MinX
        {
            get { return minX; }
        }

        public int MinY
        {
            get { return minY; }
        }

        public int Height
        {
            get { return maxY - minY + 1; }
        }

        public int Width
        {
            get { return maxX - minX + 1; }
        }

        public GlobalCoordinates TL
        {
            get
            {
                if (tl == null)
                {
                    tl = new Tile(minX, minY, zoom);
                }

                return tl.TL;
            }
        }

        public GlobalCoordinates BR
        {
            get
            {
                if (br == null)
                {
                    br = new Tile(maxX, maxY, zoom);
                }

                return br.BR;
            }
        }

        public Bounds(int zoom)
        {
            this.zoom = zoom;
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
        }

        public void AddTile(Tile tile)
        {
            if (zoom != tile.Zoom)
            {
                var message = string.Format("Tile {0} must be of zoom {1}", tile, zoom);
                throw new ArgumentException(message, "tile");
            }

            minX = Math.Min(minX, tile.X);
            minY = Math.Min(minY, tile.Y);
            maxX = Math.Max(maxX, tile.X);
            maxY = Math.Max(maxY, tile.Y);
            tl = null;
            br = null;
        }
    }
}
