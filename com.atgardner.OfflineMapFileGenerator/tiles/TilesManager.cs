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
            var dict = new Dictionary<int, IEnumerable<Tile>>();
            var biggestZoom = zoomLevels[0];
            dict[biggestZoom] = GetTilesDefinitionsFromCoordinates(coordinates, biggestZoom);
            for (var i = 1; i < zoomLevels.Length; i++)
            {
                var zoom = zoomLevels[i];
                var prevZoom = zoomLevels[i - 1];
                var tiles = new HashSet<Tile>();
                var prevTiles = dict[prevZoom];
                foreach (var prevTile in prevTiles)
                {
                    var tile = GetTileDefinitionFromOtherTile(prevTile, prevZoom, zoom);
                    tiles.Add(tile);
                }

                dict[zoom] = tiles;
            }

            var result = dict[biggestZoom];
            for (var i = 1; i < zoomLevels.Length; i++)
            {
                var zoom = zoomLevels[i];
                result = result.Concat(dict[zoom]);
            }

            return result;
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

        private static IEnumerable<Tile> GetTilesDefinitionsFromCoordinates(IEnumerable<GlobalCoordinates> coordinates, int zoom)
        {
            var tiles = new HashSet<Tile>();
            foreach (var c in coordinates)
            {
                var tile = WorldToTilePos(c, zoom);
                tiles.Add(tile);

                if (zoom > 12)
                {
                    foreach (var c2 in GetCoordinatesAround(c, 1500))
                    {
                        tile = WorldToTilePos(c2, zoom);
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }

        private static Tile GetTileDefinitionFromOtherTile(Tile prevTile, int prevZoom, int zoom)
        {
            var x = prevTile.X % 2 == 0 ? prevTile.X : prevTile.X - 1;
            var y = prevTile.Y % 2 == 0 ? prevTile.Y : prevTile.Y - 1;
            var denominator = (int)Math.Pow(2, prevZoom - zoom);
            return new Tile(prevTile.X / denominator, prevTile.Y / denominator, zoom);
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
