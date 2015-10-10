namespace com.atgardner.OfflineMapFileGeneratorTests
{
    using com.atgardner.OMFG.tiles;
    using Xunit;

    public class BoundsTests
    {
        [Fact]
        public void TL_FitsBounds()
        {
            var bounds = new Bounds(1, 1, 1, 1, 2);
            var tl = bounds.TL;
            Assert.Equal(66.5, tl.Latitude.Degrees, 1);
            Assert.Equal(-90, tl.Longitude);
        }

        [Fact]
        public void BR_FitsBounds()
        {
            var bounds = new Bounds(1, 1, 1, 1, 2);
            var br = bounds.BR;
            Assert.Equal(0, br.Latitude.Degrees, 1);
            Assert.Equal(0, br.Longitude);
        }

        [Fact]
        public void BR_LargerThanTL_SetsBounds()
        {
            var bounds = new Bounds(1, 1, 1, 1, 2);
            var tl = bounds.TL;
            var br = bounds.BR;
            Assert.True(tl.Longitude < br.Longitude);
            Assert.True(tl.Latitude > br.Latitude);
        }
    }
}
