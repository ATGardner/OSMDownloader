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
    using System.Windows.Forms;

    public partial class TilesDownloaderForm : Form
    {
        private static readonly string SourceFile = "sources.json";

        private readonly Downloader downloader;
        private readonly MapSource[] sources;
        private string folderName;

        public TilesDownloaderForm()
        {
            InitializeComponent();
            downloader = new Downloader();
            sources = MapSource.LoadSources(SourceFile);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateZoomCheckBoxes();
            cmbMapSource.DataSource = sources;
            downloader.TileDownloaded += Downloader_TileDownloaded;
        }

        private void Downloader_TileDownloaded(object sender, DownloadTileEventArgs e)
        {
            worker.ReportProgress(e.ProgressPercentage);
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

            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            var source = cmbMapSource.SelectedItem as MapSource;
            var argument = new Tuple<string, int[], MapSource>(path, zoomLevels, source);
            worker.RunWorkerAsync(argument);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as Tuple<string, int[], MapSource>;
            var path = argument.Item1;
            var zoomLevels = argument.Item2;
            MapSource source = argument.Item3;
            var kml = GetKml(path);
            worker.ReportProgress(0, "Done Reading File");
            var coordinates = ExtractCoordinates(kml);
            folderName = downloader.DownloadTiles(coordinates, zoomLevels, source);
            worker.ReportProgress(0, "Done Downloading");
            if (chkBxZip.Checked)
            {
                ZipResult(folderName, source.Name);
            }

            worker.ReportProgress(0, "Done Zipping");
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgBar.Value = e.ProgressPercentage;
            if (e.UserState is string)
            {
                lblStatus.Text = (string)e.UserState;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MapSource.SaveSources(SourceFile, sources);
            var source = cmbMapSource.SelectedItem as MapSource;
            if (source.Ammount == 0)
            {
                var message = string.Format("Your daily download limit for {0} has been reached. Please try again tomorrow.", source.Name);
                MessageBox.Show(message, "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
                if (element is LineString)
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
                    yield return new GlobalCoordinates(vector.Longitude, vector.Latitude);
                }
                else
                {
                    throw new Exception("Unrecognized element type");
                }
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

        private static void ZipResult(string folderName, string sourceName)
        {
            using (var zip = new ZipFile(sourceName + ".zip"))
            {
                zip.AddDirectory(folderName);
                zip.Save();
            }

            Directory.Delete(folderName, true);
        }
    }
}
