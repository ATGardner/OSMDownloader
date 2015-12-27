namespace com.atgardner.OMFG.sources
{
    using System.Threading.Tasks;
    using tiles;
    using packagers;
    using utils;
    using System.IO;
    using System.Text;
    using Properties;
    using System.Collections.Generic;

    class MaperitiveSource : ITileSource
    {
        private static readonly string name = "maperitive";
        private static readonly string maperitiveCommandLine = @"D:\Users\Noam\Downloads\OMFG\Maperitive\Maperitive.exe";
        private static readonly IDictionary<Tile, Task> runningTasks = new Dictionary<Tile, Task>();
        private readonly IDataCache dataCache;

        public MaperitiveSource()
        {
            dataCache = new CachePackager(name);
        }

        public async Task<byte[]> GetTileDataAsync(Tile tile)
        {
            var data = await dataCache.GetDataAsync(tile);
            if (data == null)
            {
                var tileAt10 = Tile.FromOtherTile(tile, 10);
                if (runningTasks.ContainsKey(tileAt10))
                {
                    await runningTasks[tileAt10];
                }
                else
                {
                    var task = GenerateTilesAsync(tileAt10);
                    runningTasks[tileAt10] = task;
                    await task;
                    runningTasks.Remove(tileAt10);
                    data = await dataCache.GetDataAsync(tile);
                }
            }

            return data;
        }

        private static async Task GenerateTilesAsync(Tile tile)
        {
            var scriptFile = Path.GetTempFileName();
            var scriptText = CreateScriptText(tile);
            await WriteTextAsync(scriptFile, scriptText);
            await CallMaperitiveAsync(scriptFile);
            await PackageTilesAsync();
            File.Delete(scriptFile);
        }

        private static async Task PackageTilesAsync()
        {

        }

        private static string CreateScriptText(Tile tile)
        {
            var tl = Utils.ToCoordinates(tile.X, tile.Y, tile.Zoom);
            var br = Utils.ToCoordinates(tile.X + 1, tile.Y + 1, tile.Zoom);
            return string.Format(Resources.MaperitiveScriptTemplate, br.Longitude, tl.Latitude, tl.Longitude, br.Latitude);
        }

        private static async Task WriteTextAsync(string filePath, string text)
        {
            var encodedText = Encoding.UTF8.GetBytes(text);
            using (var sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        private static async Task CallMaperitiveAsync(string scriptFile)
        {
            var args = string.Format("-exitafter {0}", scriptFile);
            await Utils.RunProcessAsync(maperitiveCommandLine, args);
        }
    }
}
