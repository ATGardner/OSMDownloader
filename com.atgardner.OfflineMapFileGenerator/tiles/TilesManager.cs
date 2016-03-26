namespace com.atgardner.OMFG.tiles
{
    using sources;
    using Gavaghan.Geodesy;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    class TilesManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly GeodeticCalculator calc = new GeodeticCalculator();
        private static readonly int[] degrees = new[] { 0, 90, 180, 270 };

        private readonly ITileSource source;

        public TilesManager(ITileSource source)
        {
            this.source = source;
        }

        public IEnumerable<Tile> GetTileDefinitions(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            zoomLevels = zoomLevels.OrderByDescending(c => c).ToArray();
            var maxZoom = zoomLevels[0];
            var tiles = new HashSet<Tile>();
            Tile lastTile = null;
            foreach (var c in coordinates)
            {
                foreach (var t in GetTilesDefinitionsFromCoordinate(c, maxZoom))
                {
                    if (t.Equals(lastTile))
                    {
                        continue;
                    }

                    lastTile = t;
                    if (tiles.Add(t))
                    {
                        var str = coordinates.ToString();
                        logger.Debug("Got tile {0} from coordinates {1}", t, c);
                        yield return t;
                        for (int i = 1; i < zoomLevels.Length; i++)
                        {
                            var higherLevelTile = Tile.FromOtherTile(t, zoomLevels[i]);
                            if (tiles.Add(higherLevelTile))
                            {
                                logger.Debug("Got tile {0} from coordinates {1}", higherLevelTile, c);
                                yield return higherLevelTile;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public Task<byte[]> GetTileData(Tile tile)
        {
            logger.Debug("Tile {0} - Getting tile data", tile);
            return source.GetTileDataAsync(tile);
        }

        private static IEnumerable<Tile> GetTilesDefinitionsFromCoordinate(GlobalCoordinates coordiate, int zoom)
        {
            var tile = new Tile(coordiate, zoom);
            yield return tile;
            foreach (var t in GetTilesAround(coordiate, zoom, 3000))
            {
                yield return t;
            }
        }

        private static IEnumerable<Tile> GetTilesDefinitionsFromCoordinates(IEnumerable<GlobalCoordinates> coordinates, int zoom)
        {
            foreach (var c in coordinates)
            {
                foreach (var t in GetTilesDefinitionsFromCoordinate(c, zoom))
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
