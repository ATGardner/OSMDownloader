namespace com.atgardner.OfflineMapFileGeneratorTests
{
    using com.atgardner.OMFG.tiles;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class BoundsTests
    {
        [Fact]
        public void AddTile_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = new Tile(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(1, bounds.Width);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(1, bounds.Height);
        }

        [Fact]
        public void AddSeveralTiles_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = new Tile(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            tile = new Tile(60, 90, 2); //tile 2/3/1
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(3, bounds.Width);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(1, bounds.Height);
        }

        [Fact]
        public void AddTileWithWrongZoom_ThrowsException()
        {
            var bounds = new Bounds(1);
            var tile = new Tile(60, -90, 2); // tile 2/1/1
            Assert.Throws(typeof(ArgumentException), () => bounds.AddTile(tile));
        }
    }
}
