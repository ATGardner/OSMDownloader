namespace com.atgardner.Downloader
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    public class MapSource : IEquatable<MapSource>
    {
        private static readonly int DailyLimit = 100;

        public string Name { get; private set; }
        public string Address { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public DateTime LastAccess { get; set; }
        public int Ammount { get; set; }

        public MapSource(string name, string address, int minZoom, int maxZoom, DateTime lastAccess, int ammount)
        {
            this.Name = name;
            this.Address = address;
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
            this.LastAccess = lastAccess;
            if (lastAccess != null && lastAccess.Date == DateTime.Today)
            {
                this.Ammount = ammount;
            }
            else
            {
                this.Ammount = DailyLimit;
            }
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

        public static MapSource[] LoadSources(string path)
        {
            var json = File.ReadAllText("sources.json");
            return JsonConvert.DeserializeObject<MapSource[]>(json);
        }

        public static void SaveSources(string path, MapSource[] sources)
        {
            var json = JsonConvert.SerializeObject(sources);
            File.WriteAllText("sources.json", json);
        }
    }
}
