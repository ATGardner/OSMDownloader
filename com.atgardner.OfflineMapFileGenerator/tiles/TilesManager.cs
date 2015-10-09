namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.sources;
    using Gavaghan.Geodesy;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    class TilesManager
    {
        private static readonly GeodeticCalculator calc = new GeodeticCalculator();
        private static readonly int[] degrees = new[] { 0, 90, 180, 270 };

        private readonly ITileSource source;

        public TilesManager(ITileSource source)
        {
            this.source = source;
        }

        public Map GetTileDefinitions(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            // reverse zoomLevels, to start with the most detailed tile
            var biggestZoom = zoomLevels.Max();
            var map = new Map(zoomLevels);
            var tiles = GetTilesDefinitionsFromCoordinates(coordinates, biggestZoom).Distinct();
            foreach (var tile in tiles)
            {
                map.AddTile(tile);
            }

            return map;
        }

        public List<Task<Tile>> GetTileData(IEnumerable<Tile> tiles)
        {
            var tasks = new List<Task<Tile>>();
            foreach (var tile in tiles)
            {
                var task = source.GetTileData(tile);
                tasks.Add(task);
            }

            return tasks;
        }

        private static IEnumerable<Tile> GetTilesDefinitionsFromCoordinates(IEnumerable<GlobalCoordinates> coordinates, int zoom)
        {
            foreach (var c in coordinates)
            {
                var tile = new Tile(c, zoom);
                yield return tile;
                foreach (var t in GetTilesAround(c, zoom, 1500))
                {
                    yield return t;
                }
            }
        }

        private static IEnumerable<Tile> GetTilesAround(GlobalCoordinates origin, int zoom, double distance)
        {
            for (var i = 500; i <= distance; i += 500)
            {
                foreach (var startBearing in degrees)
                {
                    var c = calc.CalculateEndingGlobalCoordinates(Ellipsoid.WGS84, origin, startBearing, i);
                    yield return new Tile(c, zoom);
                }
            }
        }
    }
}
