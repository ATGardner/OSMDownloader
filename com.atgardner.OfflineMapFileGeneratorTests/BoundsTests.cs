namespace com.atgardner.OfflineMapFileGeneratorTests
{
    using com.atgardner.OMFG.tiles;
    using Gavaghan.Geodesy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class BoundsTests
    {
        private static Tile FromLonLat(double lon, double lat, int zoom)
        {
            var c = new GlobalCoordinates(lon, lat);
            return new Tile(c, zoom);
        }
        [Fact]
        public void AddTile_SetsBounds()
        {
            var bounds = new Bounds(2);
            var c = new GlobalCoordinates(60, -90);
            var tile = new Tile(c, 2); // tile 2/1/1
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(1, bounds.Width);
            Assert.Equal(1, bounds.Height);
        }

        [Fact]
        public void AddTiles_InARow_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = FromLonLat(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            tile = FromLonLat(60, 90, 2); //tile 2/3/1
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(3, bounds.Width);
            Assert.Equal(1, bounds.Height);
        }

        [Fact]
        public void AddTiles_InAColumn_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = FromLonLat(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            tile = FromLonLat(-70, -90, 2); //tile 2/1/3
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(1, bounds.Width);
            Assert.Equal(3, bounds.Height);
        }

        [Fact]
        public void AddTiles_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = FromLonLat(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            tile = FromLonLat(-70, 90, 2); //tile 2/3/3
            bounds.AddTile(tile);
            Assert.Equal(1, bounds.MinX);
            Assert.Equal(1, bounds.MinY);
            Assert.Equal(3, bounds.Width);
            Assert.Equal(3, bounds.Height);
        }

        [Fact]
        public void BR_LargerThanTL_SetsBounds()
        {
            var bounds = new Bounds(2);
            var tile = FromLonLat(60, -90, 2); // tile 2/1/1
            bounds.AddTile(tile);
            var tl = bounds.TL;
            var br = bounds.BR;
            Assert.True(tl.Longitude < br.Longitude);
            Assert.True(tl.Latitude > br.Latitude);
        }

        [Fact]
        public void AddTileWithWrongZoom_ThrowsException()
        {
            var bounds = new Bounds(1);
            var tile = FromLonLat(60, -90, 2); // tile 2/1/1
            Assert.Throws(typeof(ArgumentException), () => bounds.AddTile(tile));
        }
    }
}
