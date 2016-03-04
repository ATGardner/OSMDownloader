namespace com.atgardner.OMFG.sources
{
    using System.Threading.Tasks;
    using tiles;
    using packagers;
    using utils;
    using System.IO;
    using System.Text;
    using Properties;
    using NLog;
    using System.Collections.Generic;
    using System;

    class MaperitiveSource : ITileSource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string name = "maperitive";
        private static readonly string maperitiveCommandLine = @"D:\Users\Noam\Downloads\OMFG\Maperitive\Maperitive.exe";
        private readonly IDataCache dataCache;
        private readonly IDictionary<Tile, Task> tasks;
        private Task currentTask = Task.FromResult(0);

        public MaperitiveSource()
        {
            dataCache = new CachePackager(name);
            tasks = new Dictionary<Tile, Task>();
        }

        public async Task<byte[]> GetTileDataAsync(Tile tile)
        {
            logger.Debug("Tile {0} - Getting tile data async", tile);
            var data = await dataCache.GetDataAsync(tile);
            if (data == null)
            {
                logger.Debug("Tile {0} - Data not found in cache", tile);
                var tileAt10 = Tile.FromOtherTile(tile, 11);
                if (tasks.ContainsKey(tile))
                {
                    logger.Debug("Tile {0} - Already generating data for {1}, waiting", tile, tileAt10);
                    await tasks[tile];
                    logger.Debug("Tile {0} - Done awaiting old task");
                    data = await dataCache.GetDataAsync(tile);
                }
                else
                {
                    await GenerateTilesAsync(tileAt10);
                    logger.Debug("Tile {0} - Done generating tiles", tile);
                    data = await dataCache.GetDataAsync(tile);
                }
            }

            return data;
        }

        private Task GenerateTilesAsync(Tile tile)
        {
            logger.Debug("Tile {0} - Adding tile task", tile);
            tasks[tile] = currentTask.ContinueWith(CreateTileTask(tile)).Unwrap();
            currentTask = tasks[tile];
            logger.Debug("Tile {0} - Returning currentTask {1}", tile, currentTask.Status);
            return currentTask;
        }

        private static Func<object, Task> CreateTileTask(Tile tile)
        {
            Func<object, Task> func  = async o =>
            {
                try
                {
                    logger.Debug("Tile {0} - Generating Maperitive data", tile);
                    var scriptFile = Path.GetTempFileName();
                    logger.Debug("Tile {0} - Creating script text", tile);
                    var scriptText = CreateScriptText(tile);
                    logger.Debug("Tile {0} - Writing script text to file", tile);
                    await WriteTextAsync(scriptFile, scriptText);
                    logger.Debug("Tile {0} - Calling maperitive", tile);
                    await CallMaperitiveAsync(scriptFile);
                    logger.Debug("Tile {0} - Deleting script file", tile);
                    File.Delete(scriptFile);
                    logger.Debug("Tile {0} - Packaging tiles", tile);
                    await PackageTilesAsync();
                    logger.Debug("Tile {0} - Done generating Maperitive data", tile);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Tile {0} - Failed generating maperitive data");
                }
            };
            return func;
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

        private static async Task PackageTilesAsync()
        {
            var a = 123;
        }
    }
}
