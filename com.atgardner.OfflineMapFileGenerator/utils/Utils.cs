namespace com.atgardner.OMFG.utils
{
    using Gavaghan.Geodesy;
    using MKCoolsoft.GPXLib;
    using NLog;
    using SharpKml.Base;
    using SharpKml.Dom;
    using SharpKml.Engine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    static class Utils
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Regex hrefRegex = new Regex(@"<a href=""(?<href>[^""]*)"">(?<text>[^<]*)</a>");

        public static async Task<byte[]> GetFileData(string filePath)
        {
            var fi = new FileInfo(filePath);
            var data = new byte[fi.Length];
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.ReadAsync(data, 0, data.Length);
            }

            return data;
        }

        public static IEnumerable<GlobalCoordinates> ExtractCoordinates(string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                foreach (var c in Utils.ExtractCoordinates(fileName))
                {
                    yield return c;
                }
            }
        }

        public static string ComputeHash(string str)
        {
            var md5 = MD5.Create();
            var bytes = Encoding.ASCII.GetBytes(str);
            var hash = md5.ComputeHash(bytes);
            var hashStr = BitConverter.ToString(hash);
            return hashStr.Replace("-", string.Empty).ToLower();
        }

        public static void ConfigLinkLabel(LinkLabel lnkLabel, string sourceString)
        {
            lnkLabel.Links.Clear();
            lnkLabel.Text = sourceString;
            if (string.IsNullOrWhiteSpace(sourceString))
            {
                return;
            }

            var match = hrefRegex.Match(sourceString);
            while (match.Success)
            {
                var href = match.Groups["href"];
                var text = match.Groups["text"];
                sourceString = sourceString.Replace(match.Value, text.Value);
                lnkLabel.Text = sourceString;
                lnkLabel.Links.Add(match.Index, text.Length, href.Value);
                match = hrefRegex.Match(sourceString);
            }
        }

        public static async Task<byte[]> PerformDownload(string address)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.Headers.Add("user-agent", "Offline Map File Generator");
                    return await webClient.DownloadDataTaskAsync(address);
                }
                catch (WebException e)
                {
                    logger.Error("Failed downloading tile, address: {0}, exception: {1}", address, e);
                    var response = (HttpWebResponse)e.Response;
                    if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private static IEnumerable<GlobalCoordinates> ExtractCoordinates(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (ext == ".kml" || ext == ".kmz")
            {
                return ExtractCoordinatesFromKml(fileName);
            }
            else
            {
                return ExtractCoordinatesFromGpx(fileName);
            }
        }

        private static string GetKmlString(string path)
        {
            var ext = Path.GetExtension(path);
            if (string.Equals(ext, ".kml", StringComparison.InvariantCultureIgnoreCase))
            {
                return File.ReadAllText(path);
            }

            using (var kmz = KmzFile.Open(path))
            {
                return kmz.ReadKml();
            }
        }

        private static Element GetKmlRoot(string path)
        {
            var kmlString = GetKmlString(path);
            var parser = new Parser();
            parser.ParseString(kmlString, false);
            return parser.Root;
        }

        private static IEnumerable<GlobalCoordinates> ExtractCoordinatesFromKml(string fileName)
        {
            var root = GetKmlRoot(fileName);
            foreach (var element in root.Flatten().OfType<Geometry>())
            {
                foreach (var c in ExtractCoordinate(element))
                {
                    yield return c;
                }
            }
        }

        private static IEnumerable<GlobalCoordinates> ExtractCoordinate(Geometry element)
        {
            if (element is MultipleGeometry)
            {
                foreach (var g in ((MultipleGeometry)element).Geometry)
                {
                    foreach (var c in ExtractCoordinate(g))
                    {
                        yield return c;
                    }
                }
            }
            else if (element is LineString)
            {
                foreach (var vector in ((LineString)element).Coordinates)
                {
                    yield return CreateCoordinate(vector.Latitude, vector.Longitude);
                }
            }
            else if (element is Point)
            {
                var vector = ((Point)element).Coordinate;
                yield return CreateCoordinate(vector.Latitude, vector.Longitude);
            }
            else
            {
                throw new Exception("Unrecognized element type");
            }
        }

        private static IEnumerable<GlobalCoordinates> ExtractCoordinatesFromGpx(string fileName)
        {
            GPXLib gpx = new GPXLib();
            gpx.LoadFromFile(fileName);
            foreach (var waypoint in FlattenGpx(gpx))
            {
                yield return CreateCoordinate((double)waypoint.Lat, (double)waypoint.Lon);
            }
        }

        private static IEnumerable<Wpt> FlattenGpx(GPXLib gpx)
        {
            foreach (var route in gpx.RteList)
            {
                foreach (var waypoint in route.RteptList)
                {
                    yield return waypoint;
                }
            }

            foreach (var track in gpx.TrkList)
            {
                foreach (var seg in track.TrksegList)
                {
                    foreach (var waypoint in seg.TrkptList)
                    {
                        yield return waypoint;
                    }
                }
            }

            foreach (var waypoint in gpx.WptList)
            {
                yield return waypoint;
            }
        }

        private static GlobalCoordinates CreateCoordinate(double lat, double lon)
        {
            var latAng = new Gavaghan.Geodesy.Angle(lat);
            var lonAng = new Gavaghan.Geodesy.Angle(lon);
            return new GlobalCoordinates(latAng, lonAng);
        }
    }
}
