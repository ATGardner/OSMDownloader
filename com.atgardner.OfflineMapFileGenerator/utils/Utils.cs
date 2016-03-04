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
    using System.Diagnostics;
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

        public static async Task<byte[]> GetFileDataAsync(string filePath)
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
                logger.Debug("Extracting coordinates from '{0}'", fileName);
                foreach (var c in ExtractCoordinates(fileName))
                {
                    yield return c;
                }

                logger.Debug("Done extracting coordinates from '{0}'", fileName);
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

        public static async Task<byte[]> PerformDownloadAsync(string address)
        {
            byte[] result = null;
            using (var webClient = new WebClient())
            {
                for (var retry = 0; retry < 3; retry++)
                {
                    try
                    {
                        webClient.Headers.Add("user-agent", "Offline Map File Generator");
                        logger.Debug("Downloading data, address: {0}, retry: {1}", address, retry);
                        result = await webClient.DownloadDataTaskAsync(address);
                        logger.Debug("Done downloading data, address: {0}", address);
                        return result;
                    }
                    catch (WebException e)
                    {
                        logger.Warn("Failed downloading tile, retry #{1}, address: {0}, exception: {2}", address, retry, e);
                        var response = (HttpWebResponse)e.Response;
                        if (response != null && (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden))
                        {
                            return null;
                        }
                    }
                }

                return null;
            }
        }

        public static GlobalCoordinates ToCoordinates(int x, int y, int zoom)
        {
            double n = Math.PI - ((2.0 * Math.PI * y) / Math.Pow(2.0, zoom));
            var longitude = (x / Math.Pow(2.0, zoom) * 360.0) - 180.0;
            var latitude = 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
            return new GlobalCoordinates(latitude, longitude);
        }

        public static async Task<int> RunProcessAsync(string fileName, string args)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName, Arguments = args,
                    UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = workingDirectory
                },
                EnableRaisingEvents = true
            })
            {
                logger.Debug("Running process async, fileName: {0}", fileName);
                var result = await RunProcessAsync(process).ConfigureAwait(false);
                logger.Debug("Done running process async, fileName: {0}, result: {1}", fileName, result);
                return result;
            }
        }

        private static Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode);
            process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
            process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

            bool started = process.Start();
            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
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

        private static string GetKmlString(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.Equals(ext, ".kml", StringComparison.InvariantCultureIgnoreCase))
            {
                return File.ReadAllText(fileName);
            }

            using (var kmz = KmzFile.Open(fileName))
            {
                return kmz.ReadKml();
            }
        }

        private static Element GetKmlRoot(string fileName)
        {
            try
            {
                var kmlString = GetKmlString(fileName);
                var parser = new Parser();
                parser.ParseString(kmlString, false);
                return parser.Root;
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed loading data from  {0}", fileName);
                return new Kml();
            }
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
            try
            {
                gpx.LoadFromFile(fileName);
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed loading data from  {0}", fileName);
            }

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
