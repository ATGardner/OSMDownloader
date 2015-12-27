namespace com.atgardner.OMFG.sources
{
    using tiles;
    using System.Threading.Tasks;

    public interface ITileSource
    {
        Task<byte[]> GetTileDataAsync(Tile tile);
    }
}
