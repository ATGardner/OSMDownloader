using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OSMDownloader
{
    class Program
    {
        private static int server = 0;

        static void Main(string[] args)
        {
            var path = @"halfmiles_pct_tracks_waypoints.kmz";
            var kml = GetKml(path);
            var minZoom = 9;
            var maxZoom = 15;
            var coordinates = ExtractCoordinates(kml);
            var tiles = GenerateTiles(coordinates, minZoom, maxZoom);
            var downloads = DownloadTiles(tiles).ToArray();
            Task.WaitAll(downloads);
            Console.WriteLine(downloads.Length);
        }

        private static KmlFile GetKml(string path)
        {
            var kmz = KmzFile.Open(path);
            return kmz.GetDefaultKmlFile();
        }

        private static IEnumerable<Vector> ExtractCoordinates(KmlFile kml)
        {
            foreach (var element in kml.Root.Flatten().OfType<Geometry>())
            {
                if (element is LineString)
                {
                    foreach (var vector in ((LineString)element).Coordinates)
                    {
                        yield return vector;
                    }
                }
                else if (element is Point)
                {
                    yield return ((Point)element).Coordinate;
                }
                else
                {
                    throw new Exception("Unrecognized element type");
                }
            }
        }

        private static IEnumerable<Tuple<int, int, int>> GenerateTiles(IEnumerable<Vector> coordinates, int minZoom, int maxZoom)
        {
            foreach (var c in coordinates)
            {
                var lon = c.Longitude;
                var lat = c.Latitude;
                for (int i = minZoom; i <= maxZoom; i++)
                {
                    var t = WorldToTilePos(lon, lat, i);
                    yield return new Tuple<int, int, int>(t.Item1, t.Item2, i);
                }
            }
        }

        private static IEnumerable<Task> DownloadTiles(IEnumerable<Tuple<int, int, int>> tiles)
        {
            var set = new HashSet<Tuple<int, int, int>>();
            foreach (var tile in tiles)
            {
                if (!set.Contains(tile)/* && set.Count() < 10*/)
                {
                    set.Add(tile);
                    yield return Task.Factory.StartNew(() => Console.WriteLine("tile"));// DownloadTile(tile);
                }
            }
        }

        private static Tuple<int, int> WorldToTilePos(double lon, double lat, int zoom)
        {
            var x = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            var y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return new Tuple<int, int>(x, y);
        }

        private static Task DownloadTile(Tuple<int, int, int> tile)
        {
            var currentServer = GetCurrentServer();
            var directory = string.Format("{0}/{1}", tile.Item3, tile.Item1);
            var fileName = string.Format("{0}/{1}.png", directory, tile.Item2);
            var address = string.Format("{0}/{1}", currentServer, fileName);
            Directory.CreateDirectory(directory);
            var webClient = new WebClient();
            return webClient.DownloadFileTaskAsync(address, fileName);
        }

        private static string GetCurrentServer()
        {
            var serverStr = "";
            switch (server)
            {
                case 0:
                    serverStr = "a";
                    break;
                case 1:
                    serverStr = "b";
                    break;
                case 2:
                    serverStr = "c";
                    break;
            }

            server = (server + 1) % 3;
            return string.Format("http://{0}.tile.opencyclemap.org/cycle", serverStr);
        }
    }
}
