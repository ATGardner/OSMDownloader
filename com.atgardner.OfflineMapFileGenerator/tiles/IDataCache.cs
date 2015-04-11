namespace com.atgardner.OMFG.tiles
{
    using System.Threading.Tasks;

    interface IDataCache
    {
        Task Init();

        Task<bool> HasData(Tile tile);

        Task GetData(Tile tile);

        Task PutData(Tile tile);
    }
}
