namespace com.atgardner.OMFG.packagers
{
    using com.atgardner.OMFG.tiles;
    using System;
    using System.Threading.Tasks;

    public interface IPackager : IDisposable
    {
        string OutputFile { get; }

        Task InitAsync();

        Task AddTileAsync(Tile tile, Task<byte[]> futureData);

        Task DoneAsync();
    }
}
