using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.TilesDownloader
{
    public class MapSource : IEquatable<MapSource>
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }

        public MapSource(string name, string address, string zoomLevels)
        {
            this.Name = name;
            this.Address = address;
            var split = zoomLevels.Split('-');
            this.MinZoom = int.Parse(split[0]);
            this.MaxZoom = int.Parse(split[1]);
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

            return this.Equals(other as MapSource);
        }

        public bool Equals(MapSource other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name && this.Address == other.Address;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Address.GetHashCode();
        }
    }
}
