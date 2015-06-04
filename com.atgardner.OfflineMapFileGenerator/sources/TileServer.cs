namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.tiles;
    using com.atgardner.OMFG.utils;
    using Newtonsoft.Json;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    class TileServer : ITileSource
    {
        private static readonly Regex subDomainRegExp = new Regex(@"\[(.*)\]");
        private static readonly Regex md5RegEx = new Regex(@"\*(.*)\*");
        private static int subDomainNum = 0;

        private readonly SourceDescriptor source;
        private readonly IDataCache dataCache;

        public TileServer(SourceDescriptor source)
        {
            this.source = source;
            dataCache = new CachePackager(source.Name);
        }

        public async Task<Tile> GetTileData(Tile tile)
        {
            await dataCache.GetData(tile);
            if (!tile.HasData)
            {
                var address = CreateAddress(tile);
                tile.Image = await Utils.PerformDownload(address);
                await dataCache.PutData(tile);
            }

            return tile;
        }

        private string CreateAddress(Tile tile)
        {
            string address;
            var match = subDomainRegExp.Match(source.Address);
            if (match.Success)
            {
                var subDomain = match.Groups[1].Value;
                var currentSubDomain = subDomain.Substring(subDomainNum, 1);
                subDomainNum = (subDomainNum + 1) % subDomain.Length;
                address = subDomainRegExp.Replace(source.Address, currentSubDomain);
            }
            else
            {
                address = source.Address;
            }

            address = address.Replace("{z}", "{zoom}")
                .Replace("{zoom}", tile.Zoom.ToString())
                .Replace("{x}", tile.X.ToString())
                .Replace("{y}", tile.Y.ToString());
            match = md5RegEx.Match(address);
            if (match.Success)
            {
                var md5Section = match.Groups[1].Value;
                var md5Value = Utils.ComputeHash(md5Section);
                address = md5RegEx.Replace(address, md5Value);
            }

            return address;
        }
    }
}
