namespace com.atgardner.Downloader
{
    using com.atgardner.Downloader.Properties;
    using Gavaghan.Geodesy;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class Downloader
    {
        private static readonly Regex subDomainRegExp = new Regex(@"\[(.*)\]");
        private static readonly GeodeticCalculator calc = new GeodeticCalculator();
        private static readonly int[] degrees = new [] { 0, 90, 180, 270 };
        private static readonly int limit = 100;

        public event EventHandler<DownloadTileEventArgs> TileDownloaded;
        private int subDomainNum;
        private int counter;
        private int total;

        public string DownloadTiles(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels, MapSource source)
        {
            var hash = ComputeHash(coordinates);
            var folderName = string.Format("{0}-{1}", source.Name, hash);
            subDomainNum = 0;
            counter = 0;
            var tiles = GenerateTiles(coordinates, zoomLevels).ToArray();
            total = tiles.Count();
            var set = new Dictionary<Tile, Task>();
            foreach (var tile in tiles)
            {
                if (source.Ammount == 0)
                {
                    break;
                }

                if (!set.ContainsKey(tile))
                {
                    set[tile] = DownloadTileAsync(folderName, source, tile);
                }
            }

            Task.WaitAll(set.Values.ToArray());
            return folderName;
        }

        private IEnumerable<Tile> GenerateTiles(IEnumerable<GlobalCoordinates> coordinates, int[] zoomLevels)
        {
            foreach (var c in coordinates)
            {
                var lon = c.Longitude.Degrees;
                var lat = c.Latitude.Degrees;
                foreach (var zoom in zoomLevels)
                {
                    yield return WorldToTilePos(c, zoom);
                    if (zoom > 13)
                    {
                        foreach (var c2 in GetCoordinatesAround(c, 1609.34))
                        {
                            yield return WorldToTilePos(c2, zoom);
                        }
                    }
                }
            }
        }

        private async Task<Tile> DownloadTileAsync(string folderName, MapSource source, Tile tile)
        {
            var address = GetAddress(source.Address, tile);
            var ext = Path.GetExtension(address);
            var fileName = string.Format("{0}/{1}/{2}/{3}{4}", folderName, tile.Zoom, tile.X, tile.Y, ext);
            if (File.Exists(fileName))
            {
                IncreaseCounter();
                return tile;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            source.LastAccess = DateTime.Today;
            source.Ammount--;
            await PerformDownload(address, fileName);
            IncreaseCounter();
            return tile;
        }

        private static async Task PerformDownload(string address, string fileName)
        {
            //var webClient = new WebClient();
            //await webClient.DownloadFileTaskAsync(address, fileName);
            //(new Thread(() =>
            //{
            //    await Task.Delay(100);
            //    Console.WriteLine("downloadig {0}", address);
            //})).Start();

            await Task.Run(new Action(async () =>
            {
                await Task.Delay(1000);
            }));
        }

        private void IncreaseCounter()
        {
            var prevPercentage = 100 * counter / total;
            counter++;
            var newPercentage = 100 * counter / total;
            if (prevPercentage < newPercentage)
            {
                FireTileDownloaded(newPercentage);
            }
        }
        
        private void FireTileDownloaded(int progressPercentage)
        {
            var handler = TileDownloaded;
            var args = new DownloadTileEventArgs(progressPercentage);
            if (handler != null)
            {
                handler(null, args);
            }
        }

        private static IEnumerable<GlobalCoordinates> GetCoordinatesAround(GlobalCoordinates origin, double distance)
        {
            foreach (var startBearing in degrees)
            {
                yield return calc.CalculateEndingGlobalCoordinates(Ellipsoid.WGS84, origin, startBearing, distance);
            }
        }

        private static Tile WorldToTilePos(GlobalCoordinates coordinate, int zoom)
        {
            var lon = coordinate.Longitude.Degrees;
            var lat = coordinate.Latitude.Degrees;
            var x = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            var y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return new Tile(x, y, zoom);
        }

        private string GetAddress(string addressTemplate, Tile tile)
        {
            var match = subDomainRegExp.Match(addressTemplate);
            if (match.Success)
            {
                var subDomain = match.Groups[1].Value;
                var currentSubDomain = subDomain.Substring(subDomainNum, 1);
                subDomainNum = (subDomainNum + 1) % subDomain.Length;
                addressTemplate = subDomainRegExp.Replace(addressTemplate, currentSubDomain);
            }

            return addressTemplate.Replace("{z}", "{zoom}").Replace("{zoom}", tile.Zoom.ToString()).Replace("{x}", tile.X.ToString()).Replace("{y}", tile.Y.ToString());
        }

        private static string ComputeHash(IEnumerable<GlobalCoordinates> coordinates)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(stream, coordinates.ToArray());
                bytes = stream.ToArray();
            }

            var md5 = MD5.Create();
            var hash = md5.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
