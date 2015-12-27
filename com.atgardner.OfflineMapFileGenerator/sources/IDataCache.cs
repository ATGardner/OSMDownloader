namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using System.Threading.Tasks;

    interface IDataCache
    {
        Task<byte[]> GetDataAsync(Tile tile);

        Task PutDataAsync(Tile tile, byte[] data);
    }
}
