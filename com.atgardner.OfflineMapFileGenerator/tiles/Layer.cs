namespace com.atgardner.OMFG.tiles
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Layer : IEnumerable<Tile>
    {
        private Bounds bounds;
        public Bounds Bounds
        {
            get
            {
                return bounds ?? (bounds = CreateBounds());
            }
        }

        private readonly ISet<Tile> tiles;
        private readonly int zoom;

        public Layer(int zoom)
        {
            this.zoom = zoom;
            tiles = new HashSet<Tile>();
        }

        public bool AddTile(Tile tile)
        {
            if (zoom != tile.Zoom)
            {
                var message = string.Format("Tile {0} must be of zoom {1}", tile, zoom);
                throw new ArgumentException(message, "tile");
            }

            bounds = null;
            return tiles.Add(tile);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private Bounds CreateBounds()
        {
            var minX = tiles.Min(t => t.X);
            var minY = tiles.Min(t => t.Y);
            var maxX = tiles.Max(t => t.X);
            var maxY = tiles.Max(t => t.Y);
            return new Bounds(minX, minY, maxX, maxY, zoom);
        }
    }
}
