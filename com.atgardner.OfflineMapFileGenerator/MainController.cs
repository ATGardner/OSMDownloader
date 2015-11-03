namespace com.atgardner.OMFG
{
    using packagers;
    using sources;
    using tiles;
    using utils;
    using NLog;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task DownloadTiles(string[] inputFiles, int[] zoomLevels, SourceDescriptor descriptor, string outputFile, FormatType formatType)
        {
            logger.Debug("Getting tiles, inputFiles: {0}, outputFile: {1}, zoomLevels: {2}, source: {3}", inputFiles, outputFile, zoomLevels, descriptor);
            var manager = new TilesManager(descriptor.GetSource());
            var coordinates = Utils.ExtractCoordinates(inputFiles);
            UpdateStatus(0, "Done Reading Files");
            logger.Trace("Got coordinates stream from input files");
            var map = manager.GetTileDefinitions(coordinates, zoomLevels);
            logger.Trace("Got tile definition stream from coordinates");
            var total = map.Count();
            var tasks = manager.GetTileData(map);
            var prevPercentage = -1;
            var current = 0;
            var missing = 0;
            var packager = SQLitePackager.GetPackager(formatType, outputFile, descriptor.Attribution);
            using (packager)
            {
                await packager.Init();
                while (tasks.Count > 0)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    var tile = await task;
                    if (tile.HasData)
                    {
                        try
                        {
                            await packager.AddTile(tile);
                        }
                        catch (Exception e)
                        {
                            logger.Warn("Failed adding tile {0}, error: {1}", tile, e);
                        }

                        tile.Image = null;
                    }
                    else
                    {
                        missing++;
                        logger.Warn("Source {0} does not contain tile {1}", descriptor, tile);
                    }

                    current++;
                    var progressPercentage = 100 * current / total;
                    if (progressPercentage > prevPercentage)
                    {
                        prevPercentage = progressPercentage;
                        UpdateStatus(progressPercentage, string.Format("{0}/{1} Tiles processed", current, total));
                    }
                }

                UpdateStatus(100, string.Format("Done processing {0} tiles, {1} tiles are missing ({2:P})", total, missing, (float)missing / total));
            }
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
