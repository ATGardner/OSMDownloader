namespace com.atgardner.OMFG.sources
{
    using com.atgardner.OMFG.tiles;
    using System.Threading.Tasks;

    interface IDataCache
    {
        Task GetData(Tile tile);

        Task PutData(Tile tile);
    }
}
