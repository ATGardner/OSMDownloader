namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.sources;
    using System;
    using System.Diagnostics;
    using System.IO;

    class FileDataCache : IDataCache
    {
        public byte[] GetData(MapSource source, Tile tile)
        {
            var filePath = CalculateFilePath(source, tile);
            var fi = new FileInfo(filePath);
            if (fi.Exists && fi.Length > 0)
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                return null;
            }
        }

        public void PutData(MapSource source, Tile tile)
        {
            var filePath = CalculateFilePath(source, tile);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, tile.Image);
        }

        private static string CalculateFilePath(MapSource source, Tile tile)
        {
            var ext = Path.GetExtension(source.Address);
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var processName = Process.GetCurrentProcess().ProcessName;
            var folder = Path.Combine(applicationData, processName, source.Name, tile.Zoom.ToString(), tile.X.ToString(), tile.Y.ToString());
            return Path.ChangeExtension(folder, ext);
        }
    }
}
