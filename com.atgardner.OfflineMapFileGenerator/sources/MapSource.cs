namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
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
        private static readonly Regex subDomainRegExp = new Regex(@"\[(.*)\]");
        private static readonly Regex md5RegEx = new Regex(@"\*(.*)\*");
        private static int subDomainNum = 0;

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

        public static void SaveSources(string path, MapSource[] sources)
        {
            var json = JsonConvert.SerializeObject(sources);
            File.WriteAllText(path, json);
        }

        public string CreateAddress(Tile tile)
        {
            string address;
            var match = subDomainRegExp.Match(this.Address);
            if (match.Success)
            {
                var subDomain = match.Groups[1].Value;
                var currentSubDomain = subDomain.Substring(subDomainNum, 1);
                subDomainNum = (subDomainNum + 1) % subDomain.Length;
                address = subDomainRegExp.Replace(this.Address, currentSubDomain);
            }
            else
            {
                address = this.Address;
            }

            address = address.Replace("{z}", "{zoom}")
                .Replace("{zoom}", tile.Zoom.ToString())
                .Replace("{x}", tile.X.ToString())
                .Replace("{y}", tile.Y.ToString());
            match = md5RegEx.Match(address);
            if (match.Success)
            {
                var md5Section = match.Groups[1].Value;
                var md5Value = ComputeHash(md5Section);
                address = md5RegEx.Replace(address, md5Value);
            }

            return address;
        }

        private static string ComputeHash(string str)
        {
            var md5 = MD5.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(str);
            var hash = md5.ComputeHash(bytes);
            var hashStr = BitConverter.ToString(hash);
            return hashStr.Replace("-", string.Empty).ToLower();
        }
    }
}
