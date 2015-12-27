namespace com.atgardner.OMFG.sources
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class SourceDescriptor : IEquatable<SourceDescriptor>
    {
        public enum SourceType
        {
            TileServer,
            ZipFile,
            LocalFolder,
            MBTiles,
            Maperitive
        }

        public string Name { get; private set; }
        public string Address { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public string Attribution { get; private set; }
        private SourceType type;

        public SourceDescriptor(string name, string address, int minZoom, int maxZoom, string attribution, SourceType type)
        {
            Name = name;
            Address = address;
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            Attribution = attribution;
            this.type = type;
        }

        public ITileSource GetSource()
        {
            switch (type)
            {
                case SourceType.TileServer:
                    return new TileServerSource(Name, Address);
                case SourceType.MBTiles:
                    return new MBTilesSource(Address);
                case SourceType.Maperitive:
                    return new MaperitiveSource();
                default:
                    return null;
            }
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

            return Equals(other as SourceDescriptor);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(SourceDescriptor other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name && Address == other.Address;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Address.GetHashCode();
        }

        public static async Task<SourceDescriptor[]> LoadSourcesAsync(string path)
        {
            var json = File.ReadAllText(path);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<SourceDescriptor[]>(json));
        }
    }
}
