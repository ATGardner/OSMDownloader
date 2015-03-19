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
            var bounds = new Bounds(5);
            var tile = new Tile(111, 222, 5);
            bounds.AddTile(tile);
            Assert.Equal(111, bounds.MinX);
            Assert.Equal(1, bounds.Width);
            Assert.Equal(222, bounds.MinY);
            Assert.Equal(1, bounds.Height);
        }

        [Fact]
        public void AddTileWithWrongZoom_ThrowsException()
        {
            var bounds = new Bounds(5);
            var tile = new Tile(111, 222, 6);
            Assert.Throws(typeof(ArgumentException), () => bounds.AddTile(tile));
        }
    }
}
