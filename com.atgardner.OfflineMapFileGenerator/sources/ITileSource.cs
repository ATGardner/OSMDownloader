namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using System.Threading.Tasks;

    interface ITileSource
    {
        Task<Tile> GetTileData(Tile tile);
    }
}
