namespace com.atgardner.OMFG.sources
{
    using packagers;
    using tiles;
    using utils;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NLog;

    class TileServerSource : ITileSource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Regex subDomainRegExp = new Regex(@"\[(.*)\]");
        private static readonly Regex md5RegEx = new Regex(@"\*(.*)\*");
        private static int subDomainNum = 0;

        private readonly string address;
        private readonly IDataCache dataCache;

        public TileServerSource(string name, string address)
        {
            this.address = address;
            dataCache = new CachePackager(name);
        }

        public async Task<byte[]> GetTileDataAsync(Tile tile)
        {
            logger.Debug("Tile {0} - Getting tile data async", tile);
            var data = await dataCache.GetDataAsync(tile);
            if (data != null)
            {
                return data;
            }

            var address = CreateAddress(tile);
            data = await Utils.PerformDownloadAsync(address);
            if (data != null)
            {
                await dataCache.PutDataAsync(tile, data);
            }

            logger.Debug("Tile {0} - Done getting tile data async", tile);
            return data;
        }

        private string CreateAddress(Tile tile)
        {
            string address;
            var match = subDomainRegExp.Match(this.address);
            if (match.Success)
            {
                var subDomain = match.Groups[1].Value;
                var currentSubDomain = subDomain.Substring(subDomainNum, 1);
                subDomainNum = (subDomainNum + 1) % subDomain.Length;
                address = subDomainRegExp.Replace(this.address, currentSubDomain);
            }
            else
            {
                address = this.address;
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
