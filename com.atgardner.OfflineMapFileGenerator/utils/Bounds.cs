namespace com.atgardner.OMFG.utils
{
    using com.atgardner.OMFG.tiles;
    using System;

    class Bounds
    {
        private readonly int zoom;
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;

        public int Height
        {
            get
            {
                return maxY - minY + 1;
            }
        }

        public int Width
        {
            get
            {
                return maxX - minX + 1;
            }
        }

        public Tile TL
        {
            get
            {
                return new Tile(minX, minY, zoom);
            }
        }

        public Tile BL
        {
            get
            {
                return new Tile(minX, maxY + 1, zoom);
            }
        }

        public Tile TR
        {
            get
            {
                return new Tile(maxX + 1, minY, zoom);
            }
        }

        public Tile BR
        {
            get
            {
                return new Tile(maxX + 1, maxY + 1, zoom);
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
        }
    }
}
