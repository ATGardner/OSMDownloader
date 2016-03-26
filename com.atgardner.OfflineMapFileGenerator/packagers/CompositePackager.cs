namespace com.atgardner.OMFG.packagers
{
    using System.Linq;
    using System.Threading.Tasks;
    using com.atgardner.OMFG.tiles;
    using utils;
    using System.IO;
    class CompositePackager : IPackager
    {
        private readonly IPackager[] packagers;

        public string OutputFile
        {
            get { return packagers[0].OutputFile; }
        }

        public string Attribution
        {
            get { return packagers[0].Attribution; }
        }

        public CompositePackager(params IPackager[] packagers)
        {
            this.packagers = packagers;
        }

        public Task AddTileAsync(Tile tile, Task<byte[]> futureData)
        {
            var tasks = from p in packagers
                        select p.AddTileAsync(tile, futureData);
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
            var tasks = from p in packagers
                        select p.InitAsync();
            return Task.WhenAll(tasks);
        }

        public Task DoneAsync()
        {
            var tasks = from p in packagers
                        select DoneAndZip(p);
            return Task.WhenAll(tasks);

        }

        private async Task DoneAndZip(IPackager packager)
        {
            await packager.DoneAsync();
            await Utils.ZipResult(packager.OutputFile, packager.Attribution);
            Directory.Delete(Path.GetDirectoryName(packager.OutputFile), true);
        }
    }
}
