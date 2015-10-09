namespace com.atgardner.OMFG.tiles
{
    using Gavaghan.Geodesy;
    using System;

    public class Tile : IEquatable<Tile>
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Zoom { get; private set; }
        public byte[] Image { get; set; }
        public bool HasData
        {
            get
            {
                return Image != null;
            }
        }

        public Tile(GlobalCoordinates coordinates, int zoom)
        {
            var lon = coordinates.Longitude.Degrees;
            var lat = coordinates.Latitude.Degrees;
            X = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            Y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            Zoom = zoom;
        }

        public Tile(int x, int y, int zoom)
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
    }
}
