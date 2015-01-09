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
            downloader.TileDownloaded += Downloader_TileDownloaded;
            sources = await MapSource.LoadSources(SourceFile);
            cmbMapSource.DataSource = sources;
        }

        private void Downloader_TileDownloaded(object sender, DownloadTileEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                var progressPercentage = 100 * e.Current / e.Total;
                prgBar.Value = progressPercentage;
                lblStatus.Text = string.Format("{0}/{1} Tiles Downloaded", e.Current, e.Total);
            }));
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
            var folderName = await await Task.Factory.StartNew(() => { return downloader.DownloadTiles(coordinates, zoomLevels, source); });
            lblStatus.Text = "Done Downloading";
            if (chkBxZip.Checked)
            {
                await Task.Factory.StartNew(() => ZipResult(folderName, source.Name));
                lblStatus.Text = "Done Zipping";
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
                    yield return new GlobalCoordinates(vector.Latitude, vector.Longitude);
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
