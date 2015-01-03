using com.atgardner.Downloader;
using Gavaghan.Geodesy;
using Ionic.Zip;
using Newtonsoft.Json;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.atgardner.TilesDownloader
{
    public partial class TilesDownloaderForm : Form
    {
        public TilesDownloaderForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateZoomCheckBoxes();
            var json = File.ReadAllText("sources.json");
            MapSource[] sources = JsonConvert.DeserializeObject<MapSource[]>(json);
            cmbMapSource.DataSource = sources;
            Downloader.Downloader.TileDownloaded += Downloader_TileDownloaded;
        }

        void Downloader_TileDownloaded(object sender, DownloadTileEventArgs e)
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
                MessageBox.Show("Please specify an input KML or KMZ file");
                return;
            }

            var zoomLevels = GetZoomLevels();
            if (zoomLevels.Length == 0)
            {
                MessageBox.Show("Please chose at least one zoom level");
                return;
            }

            btnRun.Enabled = false;
            prgBar.Value = 0;
            var source = cmbMapSource.SelectedItem as MapSource;
            var argument = new Tuple<string, int[], MapSource>(path, zoomLevels, source);
            worker.RunWorkerAsync(argument);
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

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as Tuple<string, int[], MapSource>;
            var path = argument.Item1;
            var zoomLevels = argument.Item2;
            MapSource source = argument.Item3;
            var kml = GetKml(path);
            worker.ReportProgress(0, "Done Reading File");
            var coordinates = ExtractCoordinates(kml).ToArray();
            Downloader.Downloader.DownloadTiles(coordinates, zoomLevels, source.Name, source.Address);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgBar.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 100)
            {
                CompleteDownload();
            }

            if (e.UserState is string)
            {
                lblStatus.Text = (string)e.UserState;
            }
        }

        private void cmbMapSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mapSource = cmbMapSource.SelectedItem as MapSource;
            ResetZoomCheckBoxes(mapSource.MinZoom, mapSource.MaxZoom);
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
                if (chkBx.Visible && chkBx.Checked)
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
                flpZoomLevels.Controls[i].Visible = false;
            }

            for (int i = minZoom; i <= maxZoom; i++)
            {
                flpZoomLevels.Controls[i].Visible = true;
            }

            for (int i = maxZoom + 1; i <= 20; i++)
            {
                flpZoomLevels.Controls[i].Visible = false;
            }
        }

        private void CompleteDownload()
        {
            btnRun.Enabled = true;
            Invoke((MethodInvoker)(() => lblStatus.Text = "Done Downloading"));
            if (chkBxZip.Checked)
            {
                ZipResult();
                Invoke((MethodInvoker)(() => lblStatus.Text = "Done Zipping"));
            }
        }

        private void ZipResult()
        {
            var source = cmbMapSource.SelectedItem as MapSource;
            using (var zip = new ZipFile(source.Name + ".zip"))
            {
                zip.AddDirectory(source.Name);
                zip.Save();
            }
        }
    }
}
