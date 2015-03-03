namespace com.atgardner.OMFG.tiles
{
    using Gavaghan.Geodesy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Tile : IEquatable<Tile>
    {
        private GlobalCoordinates? tl;
        private GlobalCoordinates? br;

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Zoom { get; private set; }
        public byte[] Image { get; set; }
        public bool FromCache { get; set; }

        public GlobalCoordinates TL
        {
            get
            {
                return tl.HasValue ? tl.Value : (tl = ToCoordinates(X, Y, Zoom)).Value;
            }
        }

        public GlobalCoordinates BR
        {
            get
            {
                return br.HasValue ? br.Value : (br = ToCoordinates(X + 1, Y + 1, Zoom)).Value;
            }
        }

        public Tile(GlobalCoordinates coordinate, int zoom)
        {
            var lon = coordinate.Longitude.Degrees;
            var lat = coordinate.Latitude.Degrees;
            X = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            Y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            Zoom = zoom;
        }

        public Tile(Tile other, int zoom)
        {
            var denominator = 2 * (int)Math.Pow(2, other.Zoom - zoom);
            X = other.X / denominator;
            Y = other.Y / denominator;
            Zoom = zoom;
        }

        public Tile(int x, int y, int zoom)
        {
            this.X = x;
            this.Y = y;
            this.Zoom = zoom;
            this.FromCache = false;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Zoom, X, Y);
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Equals(other as Tile);
        }

        public bool Equals(Tile other)
        {
            if (other == null)
            {
                return false;
            }

            return this.X == other.X && this.Y == other.Y && this.Zoom == other.Zoom;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Zoom.GetHashCode();
        }

        public static Tile FromTile(Tile prevTile, int prevZoom, int zoom)
        {
            var x = prevTile.X % 2 == 0 ? prevTile.X : prevTile.X - 1;
            var y = prevTile.Y % 2 == 0 ? prevTile.Y : prevTile.Y - 1;
            var denominator = (int)Math.Pow(2, prevZoom - zoom);
            return new Tile(prevTile.X / denominator, prevTile.Y / denominator, zoom);
        }

        public static Tile FromCoordinates(GlobalCoordinates coordinate, int zoom)
        {
            var lon = coordinate.Longitude.Degrees;
            var lat = coordinate.Latitude.Degrees;
            var x = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            var y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return new Tile(x, y, zoom);
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
