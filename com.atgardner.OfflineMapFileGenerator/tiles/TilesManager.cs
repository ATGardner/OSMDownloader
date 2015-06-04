namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.utils;
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

        private readonly ITileSource source;

        public TilesManager(SourceDescriptor descriptor)
        {
            this.source = descriptor.GetSource();
        }

        public Map GetTileDefinitions(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            // reverse zoomLevels, to start with the most detailed tile
            zoomLevels = zoomLevels.OrderByDescending(c => c).ToArray();
            var biggestZoom = zoomLevels[0];
            var map = new Map();
            var tiles = GetTilesDefinitionsFromCoordinates(coordinates, biggestZoom);
            map.AddAll(tiles);
            for (var i = 1; i < zoomLevels.Length; i++)
            {
                var zoom = zoomLevels[i];
                var prevZoom = zoomLevels[i - 1];
                var prevLayer = map[prevZoom];
                foreach (var prevTile in prevLayer)
                {
                    var tile = new Tile(prevTile, zoom);
                    map.AddTile(tile);
                }
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
