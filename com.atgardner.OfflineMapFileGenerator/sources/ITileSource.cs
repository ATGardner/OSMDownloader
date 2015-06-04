namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using System.Threading.Tasks;

    public interface ITileSource
    {
        Task<Tile> GetTileData(Tile tile);
    }
}
