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
    using System.Threading.Tasks;
    using System.Collections.Generic;

    class MainController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event ProgressChangedEventHandler ProgressChanged;

        private static string BaseFolder
        {
            get
            {
                var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(applicationData, "OMFG");
            }
        }

        public async Task DownloadTilesAsync(string[] inputFiles, int[] zoomLevels, SourceDescriptor descriptor, string outputFile, FormatType formatType)
        {
            logger.Debug("Starting to download tiles from {0} files, into '{1}'", inputFiles.Length, outputFile);
            var manager = new TilesManager(descriptor.GetSource());
            var coordinates = Utils.ExtractCoordinates(inputFiles);
            var tiles = manager.GetTileDefinitions(coordinates, zoomLevels);
            var total = 0;
            var current = 0;
            using (var packager = SQLitePackager.GetPackager(formatType, outputFile, descriptor.Attribution))
            {
                var tasks = new List<Task>();
                await packager.InitAsync();
                foreach (var t in tiles)
                {
                    total++;
                    var futureData = manager.GetTileData(t);
                    var task = packager.AddTileAsync(t, futureData);
                    if (task.IsCompleted)
                    {
                        current++;
                        if (current % 10 == 0)
                        {
                            UpdateStatus(-1, string.Format("{0} Tiles processed", current));
                        }
                    }
                    else
                    {
                        tasks.Add(task);
                    }
                }

                var prevPercentage = -1;
                while (tasks.Count > 0)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    current++;
                    var progressPercentage = 100 * current / total;
                    if (progressPercentage > prevPercentage)
                    {
                        prevPercentage = progressPercentage;
                        UpdateStatus(progressPercentage, string.Format("{0}/{1} Tiles processed", current, total));
                    }
                }

                logger.Debug("Done processing {0} tiles", total);
                UpdateStatus(100, string.Format("Done processing {0} tiles", total));
                await packager.DoneAsync();
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
