namespace com.atgardner.OMFG.tiles
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.utils;
    using Gavaghan.Geodesy;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    [Obsolete("Use the new CachePackager instead")]
    class FileDataCache : IEnumerable<Tile>, IEnumerable
    {
        private static readonly Regex pattern = new Regex(@"\\(?<zoom>\d+)\\(?<x>\d+)\\(?<y>\d+)(?<ext>\.(jpg|png))");
        private readonly string fullPath;
        private string ext;

        public FileDataCache(string fullPath)
            : this(fullPath, string.Empty)
        {
        }

        public FileDataCache(string fullPath, string ext)
        {
            this.fullPath = fullPath;
            this.ext = ext;
        }

        public Task<bool> HasData(Tile tile)
        {
            var filePath = CalculateFilePath(fullPath, tile);
            var fi = new FileInfo(filePath);
            return Task.FromResult(fi.Exists && fi.Length > 0);
        }

        public async Task GetData(Tile tile)
        {
            var filePath = CalculateFilePath(fullPath, tile);
            tile.Image = await Utils.GetFileData(filePath);
        }

        public async Task PutData(Tile tile)
        {
            var filePath = CalculateFilePath(fullPath, tile);
            if (tile.Image != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.WriteAsync(tile.Image, 0, tile.Image.Length);
                }
            }
        }

        private string CalculateFilePath(string fullPath, Tile tile)
        {
            var folder = Path.Combine(fullPath, tile.Zoom.ToString(), tile.X.ToString(), tile.Y.ToString());
            return Path.ChangeExtension(folder, ext);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            var imageFiles = Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories);
            foreach (var imageFile in imageFiles)
            {
                var match = pattern.Match(imageFile);
                int x, y, zoom;
                if (!int.TryParse(match.Groups["x"].Value, out x) || !int.TryParse(match.Groups["y"].Value, out y) || !int.TryParse(match.Groups["zoom"].Value, out zoom))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(ext))
                {
                    ext = match.Groups["ext"].Value;
                }

                var tile = new Tile(x, y, zoom);
                yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
