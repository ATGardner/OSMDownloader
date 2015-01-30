using Gavaghan.Geodesy;
using MKCoolsoft.GPXLib;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.atgardner.OMFG
{
    static class FileUtils
    {
        public static IEnumerable<GlobalCoordinates> ExtractCoordinates(string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                foreach (var c in FileUtils.ExtractCoordinates(fileName))
                {
                    yield return c;
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

        private static KmlFile GetKml(string path)
        {
            var ext = Path.GetExtension(path);
            KmlFile kml;
            if (string.Equals(ext, ".kmz", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var kmz = KmzFile.Open(path))
                {
                    kml = kmz.GetDefaultKmlFile();
                }
            }
            else
            {
                using (var stream = File.OpenRead(path))
                {
                    kml = KmlFile.Load(stream);
                }
            }

            return kml;
        }

        private static IEnumerable<GlobalCoordinates> ExtractCoordinatesFromKml(string fileName)
        {
            var kml = GetKml(fileName);
            foreach (var element in kml.Root.Flatten().OfType<Geometry>())
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
