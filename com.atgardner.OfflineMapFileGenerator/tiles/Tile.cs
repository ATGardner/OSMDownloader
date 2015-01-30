namespace com.atgardner.OMFG.tiles
{
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

        public Tile(int x, int y, int zoom)
        {
            this.X = x;
            this.Y = y;
            this.Zoom = zoom;
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
