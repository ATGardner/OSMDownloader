namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using com.atgardner.OMFG.utils;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class MapSource : IEquatable<MapSource>
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public string Attribution { get; private set; }

        public MapSource(string name, string address, int minZoom, int maxZoom, string attribution)
        {
            this.Name = name;
            this.Address = address;
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
            this.Attribution = attribution;
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

        public override string ToString()
        {
            return Name;
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

        public static async Task<MapSource[]> LoadSources(string path)
        {
            var json = File.ReadAllText(path);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MapSource[]>(json));
        }
    }
}
