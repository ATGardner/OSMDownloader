namespace com.atgardner.OMFG
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.tiles;
    using com.atgardner.OMFG.utils;
    using NLog;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    class MainController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public SourceDescriptor[] Sources { get; private set; }

        public event ProgressChangedEventHandler ProgressChanged;

        private static string BaseFolder
        {
            get
            {
                var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(applicationData, "OMFG");
            }
        }

        public async Task Init(string sourceFile)
        {
            Sources = await SourceDescriptor.LoadSources(sourceFile);
        }

        public async void DownloadTiles(string[] inputFiles, int[] zoomLevels, SourceDescriptor descriptor, string outputFile, FormatType formatType)
        {
            logger.Debug("Getting tiles, inputFiles: {0}, outputFile: {1}, zoomLevels: {2}, source: {3}", inputFiles, outputFile, zoomLevels, descriptor);
            var manager = new TilesManager(descriptor);
            var coordinates = Utils.ExtractCoordinates(inputFiles);
            UpdateStatus(0, "Done Reading Files");
            logger.Trace("Got coordinates stream from input files");
            var map = manager.GetTileDefinitions(coordinates, zoomLevels);
            logger.Trace("Got tile definition stream from coordinates");
            var total = map.Count();
            var tasks = manager.GetTileData(map);
            var prevPercentage = -1;
            var current = 0;
            var packager = SQLitePackager.GetPackager(formatType, outputFile, map);
            using (packager)
            {
                await packager.Init();
                while (tasks.Count > 0)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    var tile = await task;
                    await packager.AddTile(tile);
                    tile.Image = null;
                    current++;
                    var progressPercentage = 100 * current / total;
                    if (progressPercentage > prevPercentage)
                    {
                        prevPercentage = progressPercentage;
                        UpdateStatus(progressPercentage, string.Format("{0}/{1} Tiles Downloaded", current, total));
                    }
                }

                UpdateStatus(100, string.Format("Done Downloading {0} tiles", total));
            }
        }

        public async Task UpgradeCache()
        {
            if (!Directory.Exists(BaseFolder))
            {
                return;
            }

            var sourceFolders = Directory.EnumerateDirectories(BaseFolder);
            foreach (var sourceFolder in sourceFolders)
            {
                try
                {
                    var cacheName = Path.GetFileName(sourceFolder);
                    if (cacheName == "cache")
                    {
                        // in case we have started upgrading before, and already have the "cache" subfolder there
                        continue;
                    }

                    var message = string.Format("upgrading {0} cache", cacheName);
                    UpdateStatus(-1, message);

#pragma warning disable 0618
                    // only used to upgrade to newer cache
                    var sourceCache = new FileDataCache(sourceFolder);
#pragma warning restore 0618

                    using (var targetCache = new CachePackager(cacheName))
                    {
                        await targetCache.Init();
                        foreach (var tile in sourceCache)
                        {
                            await sourceCache.GetData(tile);
                            await targetCache.AddTile(tile);
                        }
                    }

                    Directory.Delete(sourceFolder, true);
                }
                catch (Exception e)
                {
                    logger.Error("Failed upgrading cache", e);
                }
            }

            UpdateStatus(-1, "Done upgrading cached tiles");
        }

        private void UpdateStatus(int progressPercentage, string status)
        {
            var handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, new ProgressChangedEventArgs(progressPercentage, status));
            }
        }
    }
}
