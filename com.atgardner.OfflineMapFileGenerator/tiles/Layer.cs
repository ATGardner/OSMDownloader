namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Layer : IEnumerable<Tile>
    {
        public Bounds Bounds { get; private set; }
        private readonly ICollection<Tile> tiles;
        private readonly int zoom;

        public Layer(int zoom)
        {
            this.zoom = zoom;
            Bounds = new Bounds(zoom);
            tiles = new HashSet<Tile>();
        }

        public void AddTile(Tile tile)
        {
            if (zoom != tile.Zoom)
            {
                var message = string.Format("Tile {0} must be of zoom {1}", tile, zoom);
                throw new ArgumentException(message, "tile");
            }

            Bounds.AddTile(tile);
            tiles.Add(tile);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
