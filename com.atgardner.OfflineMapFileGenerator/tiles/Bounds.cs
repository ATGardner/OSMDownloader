namespace com.atgardner.OMFG.tiles
{
    using Gavaghan.Geodesy;
    using System;

    public class Bounds
    {
        private readonly int zoom;
        private GlobalCoordinates? tl;
        private GlobalCoordinates? br;
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
                    tl = ToCoordinates(minX, minY, zoom);
                }

                return tl.Value;
            }
        }

        public GlobalCoordinates BR
        {
            get
            {
                if (br == null)
                {
                    br = ToCoordinates(maxX + 1, maxY + 1, zoom);
                }

                return br.Value;
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

        public override string ToString()
        {
            return string.Format("{0}, {1}", TL, BR);
        }

        private static GlobalCoordinates ToCoordinates(int x, int y, int zoom)
        {
            double n = Math.PI - ((2.0 * Math.PI * y) / Math.Pow(2.0, zoom));
            var longitude = (x / Math.Pow(2.0, zoom) * 360.0) - 180.0;
            var latitude = 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
            return new GlobalCoordinates(latitude, longitude);
        }
    }
}
