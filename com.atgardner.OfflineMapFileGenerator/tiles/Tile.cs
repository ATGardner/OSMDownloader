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
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Zoom { get; private set; }
        public byte[] Image { get; set; }
        public bool FromCache { get; set; }

        public Tile(GlobalCoordinates coordinates, int zoom)
        {
            var lon = coordinates.Longitude.Degrees;
            var lat = coordinates.Latitude.Degrees;
            X = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            Y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            Zoom = zoom;
        }

        public Tile(Tile other, int zoom)
        {
            var denominator = (int)Math.Pow(2, other.Zoom - zoom);
            X = other.X / denominator;
            Y = other.Y / denominator;
            Zoom = zoom;
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
