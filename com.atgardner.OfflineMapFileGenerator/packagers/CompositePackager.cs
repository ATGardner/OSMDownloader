namespace com.atgardner.OMFG.packagers
{
    using System.Linq;
    using System.Threading.Tasks;
    using com.atgardner.OMFG.tiles;

    class CompositePackager : IPackager
    {
        private readonly IPackager[] packagers;

        public CompositePackager(params IPackager[] packagers)
        {
            this.packagers = packagers;
        }

        public Task AddTileAsync(Tile tile, Task<byte[]> futureData)
        {
            var tasks = from p in packagers select p.AddTileAsync(tile, futureData);
            return Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var p in packagers)
            {
                p.Dispose();
            }
        }

        public Task InitAsync()
        {
            var tasks = from p in packagers select p.InitAsync();
            return Task.WhenAll(tasks);
        }
    }
}
