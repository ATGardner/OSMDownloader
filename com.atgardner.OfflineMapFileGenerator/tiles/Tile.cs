namespace com.atgardner.OMFG.tiles
{
    using Gavaghan.Geodesy;
    using System;

    public class Tile : IEquatable<Tile>
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Zoom { get; private set; }

        public Tile(GlobalCoordinates coordinates, int zoom)
        {
            var lon = coordinates.Longitude.Degrees;
            var lat = coordinates.Latitude.Degrees;
            X = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            Y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            Zoom = zoom;
        }

        private Tile(int x, int y, int zoom)
        {
            X = x;
            Y = y;
            Zoom = zoom;
        }

        public static Tile FromOtherTile(Tile other, int zoom)
        {
            if (other.Zoom < zoom)
            {
                throw new ArgumentException(string.Format("Can't create a tile of zoom {0}, from a tile of zoom {1}", zoom, other.Zoom));
            }

            if (other.Zoom == zoom)
            {
                return other;
            }

            var denominator = (int)Math.Pow(2, other.Zoom - zoom);
            var x = other.X / denominator;
            var y = other.Y / denominator;
            return new Tile(x, y, zoom);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Zoom, X, Y);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Equals(other as Tile);
        }

        public bool Equals(Tile other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X && Y == other.Y && Zoom == other.Zoom;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Zoom.GetHashCode();
        }
    }
}
