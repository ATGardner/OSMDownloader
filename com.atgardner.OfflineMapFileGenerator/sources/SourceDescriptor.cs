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
            this.Name = name;
            this.Address = address;
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
            this.Attribution = attribution;
            this.type = type;
        }

        public ITileSource GetSource()
        {
            switch (type)
            {
                case SourceType.TileServer:
                    return new TileServerSource(this.Name, this.Address);
                case SourceType.MBTiles:
                    return new MBTilesSource(this.Address);
                default:
                    return null;
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

            return this.Equals(other as SourceDescriptor);
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

            return this.Name == other.Name && this.Address == other.Address;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Address.GetHashCode();
        }

        public static async Task<SourceDescriptor[]> LoadSources(string path)
        {
            var json = File.ReadAllText(path);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<SourceDescriptor[]>(json));
        }
    }
}
