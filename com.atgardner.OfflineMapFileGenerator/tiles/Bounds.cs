namespace com.atgardner.OMFG.tiles
{
    using utils;
    using Gavaghan.Geodesy;

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
                    tl = Utils.ToCoordinates(minX, minY, zoom);
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
                    br = Utils.ToCoordinates(maxX + 1, maxY + 1, zoom);
                }

                return br.Value;
            }
        }

        public Bounds(int minX, int minY, int maxX, int maxY, int zoom)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
            this.zoom = zoom;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", TL, BR);
        }
    }
}
