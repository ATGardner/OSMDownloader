namespace com.atgardner.OMFG.tiles
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class Map : IEnumerable<Tile>
    {
        IDictionary<int, Layer> layers;

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

        public Map()
        {
            layers = new Dictionary<int, Layer>();
        }

        public void AddAll(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                AddTile(tile);
            }
        }

        public void AddTile(Tile tile)
        {
            Layer layer;
            if (!layers.TryGetValue(tile.Zoom, out layer))
            {
                layer = new Layer(tile.Zoom);
                layers[tile.Zoom] = layer;
            }

            layer.AddTile(tile);
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
