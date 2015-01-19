namespace com.atgardner.TilesDownloader
{
    using com.atgardner.Downloader;
    using Gavaghan.Geodesy;
    using Ionic.Zip;
    using SharpKml.Dom;
    using SharpKml.Engine;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class TilesDownloaderForm : Form
    {
        private static readonly string SourceFile = "sources.json";

        private readonly Downloader downloader;
        private MapSource[] sources;

        public TilesDownloaderForm()
        {
            InitializeComponent();
            downloader = new Downloader();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateZoomCheckBoxes();
            sources = await MapSource.LoadSources(SourceFile);
            cmbMapSource.DataSource = sources;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtBxInput.Text = dlgOpenFile.FileName;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var path = txtBxInput.Text;
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Please specify an input KML or KMZ file", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var zoomLevels = GetZoomLevels();
            if (zoomLevels.Length == 0)
            {
                MessageBox.Show("Please chose at least one zoom level", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var source = cmbMapSource.SelectedItem as MapSource;
            DownloadTiles(path, zoomLevels, source);
        }


        private async void DownloadTiles(string path, int[] zoomLevels, MapSource source)
        {
            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            var kml = await Task.Factory.StartNew(() => GetKml(path));
            lblStatus.Text = "Done Reading File";
            var coordinates = ExtractCoordinates(kml);
            var tileFiles = downloader.DownloadTiles(coordinates, zoomLevels, source);
            var prevPercentage = 0;
            var current = 0;
            var total = tileFiles.Count;
            using (var packager = new SQLitePackager(path))
            {
                await packager.Init();
                while (tileFiles.Count > 0)
                {
                    var task = await Task.WhenAny(tileFiles);
                    tileFiles.Remove(task);
                    var tileFile = await task;
                    await HandleResult(path, source, tileFile);
                    current++;
                    var progressPercentage = 100 * current / total;
                    if (progressPercentage > prevPercentage)
                    {
                        prevPercentage = progressPercentage;
                        prgBar.Value = progressPercentage;
                        lblStatus.Text = string.Format("{0}/{1} Tiles Downloaded", current, total);
                    }
                }
            }

            lblStatus.Text = string.Format("Done Downloading {0} tiles", total);
            //if (chkBxZip.Checked)
            //{
            //    lblStatus.Text = "Zipping Resulting Tiles";
            //    var outputFolder = CreateOutputFolder(path, source);
            //    await Task.Factory.StartNew(() => ZipResult(outputFolder));
            //    lblStatus.Text = "Done Zipping";
            //}

            tlpContainer.Enabled = true;
        }

        private void cmbMapSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mapSource = cmbMapSource.SelectedItem as MapSource;
            ResetZoomCheckBoxes(mapSource.MinZoom, mapSource.MaxZoom);
            lnk.Text = mapSource.Attribution;
        }

        private KmlFile GetKml(string path)
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

        private IEnumerable<GlobalCoordinates> ExtractCoordinates(KmlFile kml)
        {
            foreach (var element in kml.Root.Flatten().OfType<Geometry>())
            {
                foreach (var c in ExtractCoordinate(element))
                {
                    yield return c;
                }
            }
        }

        private IEnumerable<GlobalCoordinates> ExtractCoordinate(Geometry element)
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
                    var lon = new Gavaghan.Geodesy.Angle(vector.Longitude);
                    var lat = new Gavaghan.Geodesy.Angle(vector.Latitude);
                    yield return new GlobalCoordinates(lat, lon);
                }
            }
            else if (element is Point)
            {
                var vector = ((Point)element).Coordinate;
                yield return new GlobalCoordinates(vector.Latitude, vector.Longitude);
            }
            else
            {
                throw new Exception("Unrecognized element type");
            }
        }

        private void CreateZoomCheckBoxes()
        {
            for (int i = 0; i <= 20; i++)
            {
                var chkBx = new CheckBox();
                chkBx.Text = i.ToString();
                flpZoomLevels.Controls.Add(chkBx);
            }
        }

        private int[] GetZoomLevels()
        {
            var levels = new List<int>();
            foreach (CheckBox chkBx in flpZoomLevels.Controls)
            {
                if (chkBx.Enabled && chkBx.Checked)
                {
                    levels.Add(int.Parse(chkBx.Text));
                }
            }

            return levels.ToArray();
        }

        private void ResetZoomCheckBoxes(int minZoom, int maxZoom)
        {
            for (int i = 0; i < minZoom; i++)
            {
                flpZoomLevels.Controls[i].Enabled = false;
            }

            for (int i = minZoom; i <= maxZoom; i++)
            {
                flpZoomLevels.Controls[i].Enabled = true;
            }

            for (int i = maxZoom + 1; i <= 20; i++)
            {
                flpZoomLevels.Controls[i].Enabled = false;
            }
        }

        private async Task HandleResult(string sourceFile, MapSource source, string tileFile)
        {
            if (!File.Exists(tileFile))
            {
                return;
            }

            //var outputFolder = CreateOutputFolder(sourceFile, source);
            //var outputFile = tileFile.Replace(source.Name, outputFolder);
            //Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            //using (FileStream srcStream = File.Open(tileFile, FileMode.Open))
            //{
            //    using (FileStream destStream = File.Create(outputFile))
            //    {
            //        await srcStream.CopyToAsync(destStream);
            //    }
            //}
        }

        private static string CreateOutputFolder(string sourceFile, MapSource source)
        {
            var sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            var outputFolder = String.Format("{0} - {1}", sourceFileName, source.Name);
            Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }

        private static void ZipResult(string folderName)
        {
            using (var zip = new ZipFile(folderName + ".zip"))
            {
                zip.AddDirectory(folderName);
                zip.Save();
            }

            Directory.Delete(folderName, true);
        }
    }
}
