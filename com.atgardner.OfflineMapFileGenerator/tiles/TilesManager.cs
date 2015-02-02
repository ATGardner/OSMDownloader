namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.sources;
    using Gavaghan.Geodesy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    class TilesManager
    {
        private static readonly GeodeticCalculator calc = new GeodeticCalculator();
        private static readonly int[] degrees = new[] { 0, 90, 180, 270 };

        private readonly IDataCache dataCache;

        public TilesManager()
        {
            dataCache = new FileDataCache();
        }

        public IEnumerable<Tile> GetTileDefinitions(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            // reverse zoomLevels, to start with the most detailed tile
            zoomLevels = zoomLevels.OrderByDescending(c => c).ToArray();
            var uniqueTiles = new HashSet<Tile>();
            foreach (var c in coordinates)
            {
                var skipZoom = true;
                var lon = c.Longitude.Degrees;
                var lat = c.Latitude.Degrees;
                foreach (var zoom in zoomLevels)
                {
                    var tile = WorldToTilePos(c, zoom);
                    if (uniqueTiles.Add(tile))
                    {
                        skipZoom = false;
                        yield return tile;
                    }

                    if (zoom > 12)
                    {
                        foreach (var c2 in GetCoordinatesAround(c, 1500))
                        {
                            tile = WorldToTilePos(c2, zoom);
                            if (uniqueTiles.Add(tile))
                            {
                                skipZoom = false;
                                yield return tile;
                            }
                        }
                    }

                    if (skipZoom)
                    {
                        break;
                    }
                }
            }
        }

        public IEnumerable<Task<Tile>> GetTileData(MapSource source, IEnumerable<Tile> tiles, bool dryRun)
        {
            var tasks = new List<Task<Tile>>();
            foreach (var tile in tiles)
            {
                var task = GetTileData(source, tile, dryRun);
                tasks.Add(task);
            }

            return tasks;
        }

        private async Task<Tile> GetTileData(MapSource source, Tile tile, bool dryRun)
        {
            if (dataCache.HasData(source, tile))
            {
                tile.Image = dataCache.GetData(source, tile);
                tile.FromCache = true;
            }
            else if (!dryRun)
            {
                var address = source.CreateAddress(tile);
                tile.Image = await PerformDownload(address);
                dataCache.PutData(source, tile);
            }

            return tile;
        }

        private static Tile WorldToTilePos(GlobalCoordinates coordinate, int zoom)
        {
            var lon = coordinate.Longitude.Degrees;
            var lat = coordinate.Latitude.Degrees;
            var x = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            var y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return new Tile(x, y, zoom);
        }

        private static IEnumerable<GlobalCoordinates> GetCoordinatesAround(GlobalCoordinates origin, double distance)
        {
            for (var i = 500; i <= distance; i += 500)
            {

                foreach (var startBearing in degrees)
                {
                    yield return calc.CalculateEndingGlobalCoordinates(Ellipsoid.WGS84, origin, startBearing, i);
                }
            }
        }

        private async Task<byte[]> PerformDownload(string address)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.Headers.Add("user-agent", "Offline Map File Generator");
                    return await webClient.DownloadDataTaskAsync(address);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed downloading tile, address: {0}, exception: {1}", address, e);
                    return null;
                }
            }
        }
    }
}
