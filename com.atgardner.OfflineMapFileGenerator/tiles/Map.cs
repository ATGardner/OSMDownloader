namespace com.atgardner.OMFG.tiles
{
    using System.Collections;
    using System.Collections.Generic;

    class Map : IEnumerable<Tile>
    {
        IDictionary<int, Layer> layers;

        public Layer this[int zoom]
        {
            get
            {
                return layers[zoom];
            }
        }

        public Map()
        {
            layers = new Dictionary<int, Layer>();
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
