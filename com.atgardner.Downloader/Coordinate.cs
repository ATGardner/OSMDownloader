using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.Downloader
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public Coordinate(double longitude, double latitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
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

            return this.Equals(other as Coordinate);
        }

        public bool Equals(Coordinate other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Latitude == other.Latitude) &&
                   (this.Longitude == other.Longitude);
        }

        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
        }
    }
}
