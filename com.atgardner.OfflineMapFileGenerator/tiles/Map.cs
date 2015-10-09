namespace com.atgardner.OMFG.tiles
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class Map : IEnumerable<Tile>
    {
        private readonly int[] zoomLevels;
        private readonly IDictionary<int, Layer> layers;

        public Layer this[int zoom]
        {
            get
            {
                return layers[zoom];
            }
        }

        public int[] ZoomLevels
        {
            get { return layers.Keys.OrderBy(c => c).ToArray(); }
        }

        public Map(int[] zoomLevels)
        {
            layers = new Dictionary<int, Layer>();
            this.zoomLevels = zoomLevels.OrderByDescending(c => c).ToArray();
            foreach (var zoom in this.zoomLevels)
            {
                layers[zoom] = new Layer(zoom);
            }
        }

        public void AddTile(Tile tile)
        {
            foreach (var pair in layers)
            {
                var other = Tile.FromOtherTile(tile, pair.Key);
                var layer = pair.Value;
                if (!pair.Value.AddTile(other))
                {
                    break;
                }
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (var layer in layers.Values)
            {
                foreach (var tile in layer)
                {
                    yield return tile;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
