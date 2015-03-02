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

        public Map GetTileDefinitions(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            // reverse zoomLevels, to start with the most detailed tile
            zoomLevels = zoomLevels.OrderByDescending(c => c).ToArray();
            var biggestZoom = zoomLevels[0];
            var map = GetTilesDefinitionsFromCoordinates(coordinates, biggestZoom);
            for (var i = 1; i < zoomLevels.Length; i++)
            {
                var zoom = zoomLevels[i];
                var prevZoom = zoomLevels[i - 1];
                var prevLayer = map[prevZoom];
                foreach (var prevTile in prevLayer)
                {
                    var tile = Tile.FromTile(prevTile, prevZoom, zoom);
                    map.AddTile(tile);
                }
            }

            return map;
        }

        public int CheckTileCache(MapSource source, IEnumerable<Tile> tiles)
        {
            var result = 0;
            foreach (var tile in tiles)
            {
                tile.FromCache = dataCache.HasData(source, tile);
                if (!tile.FromCache)
                {
                    result++;
                }
            }

            return result;
        }

        public IEnumerable<Task<Tile>> GetTileData(MapSource source, IEnumerable<Tile> tiles)
        {
            var tasks = new List<Task<Tile>>();
            foreach (var tile in tiles)
            {
                var task = GetTileData(source, tile);
                tasks.Add(task);
            }

            return tasks;
        }

        private static Map GetTilesDefinitionsFromCoordinates(IEnumerable<GlobalCoordinates> coordinates, int zoom)
        {
            var map = new Map();
            foreach (var c in coordinates)
            {
                var tile = Tile.FromCoordinates(c, zoom);
                map.AddTile(tile);

                if (zoom > 12)
                {
                    foreach (var c2 in GetCoordinatesAround(c, 1500))
                    {
                        tile = Tile.FromCoordinates(c2, zoom);
                        map.AddTile(tile);
                    }
                }
            }

            return map;
        }

        private async Task<Tile> GetTileData(MapSource source, Tile tile)
        {
            if (tile.FromCache)
            {
                tile.Image = dataCache.GetData(source, tile);
            }
            else
            {
                var address = source.CreateAddress(tile);
                tile.Image = await PerformDownload(address);
                dataCache.PutData(source, tile);
            }

            return tile;
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

        private static async Task<byte[]> PerformDownload(string address)
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
