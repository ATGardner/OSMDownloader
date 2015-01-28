namespace com.atgardner.TilesDownloader
{
    using com.atgardner.Downloader;
    using Gavaghan.Geodesy;
    using Ionic.Zip;
    using MKCoolsoft.GPXLib;
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
                txtBxInput.Text = string.Join("; ", dlgOpenFile.FileNames);
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var fileNames = dlgOpenFile.FileNames;
            if (fileNames.Length == 0)
            {
                MessageBox.Show("Please specify an input file or files", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var zoomLevels = GetZoomLevels();
            if (zoomLevels.Length == 0)
            {
                MessageBox.Show("Please chose at least one zoom level", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var source = cmbMapSource.SelectedItem as MapSource;
            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            await Task.Factory.StartNew(() => DownloadTiles(fileNames, zoomLevels, source));
            lblStatus.Text = string.Format("Done Downloading tiles");
            tlpContainer.Enabled = true;
        }

        private async void DownloadTiles(string[] fileNames, int[] zoomLevels, MapSource source)
        {
            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            UpdateStatus("Done Reading File");
            var coordinates = FileUtils.ExtractCoordinates(fileNames);
            var tileFiles = downloader.DownloadTiles(coordinates, zoomLevels, source);
            var prevPercentage = 0;
            var current = 0;
            var total = tileFiles.Count;
            using (var packager = new SQLitePackager(fileNames[0]))
            {
                await packager.Init();
                while (tileFiles.Count > 0)
                {
                    var task = await Task.WhenAny(tileFiles);
                    tileFiles.Remove(task);
                    var tile = await task;
                    //await HandleResult(path, source, tile);
                    packager.AddTile(tile);
                    current++;
                    var progressPercentage = 100 * current / total;
                    if (progressPercentage > prevPercentage)
                    {
                        prevPercentage = progressPercentage;
                        UpdateProgBar(progressPercentage);
                        UpdateStatus(string.Format("{0}/{1} Tiles Downloaded", current, total));
                    }
                }
            }

            UpdateStatus(string.Format("Done Downloading {0} tiles", total));
        }

        private void UpdateStatus(string status)
        {
            BeginInvoke((MethodInvoker)(() => lblStatus.Text = status));
        }

        private void UpdateProgBar(int value)
        {
            BeginInvoke((MethodInvoker)(() => prgBar.Value = value));
        }

        private void cmbMapSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mapSource = cmbMapSource.SelectedItem as MapSource;
            ResetZoomCheckBoxes(mapSource.MinZoom, mapSource.MaxZoom);
            lnk.Text = mapSource.Attribution;
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

            var outputFolder = CreateOutputFolder(sourceFile, source);
            var outputFile = tileFile.Replace(source.Name, outputFolder);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            using (FileStream srcStream = File.Open(tileFile, FileMode.Open))
            {
                using (FileStream destStream = File.Create(outputFile))
                {
                    await srcStream.CopyToAsync(destStream);
                }
            }
        }

        private static string CreateOutputFolder(string sourceFile, MapSource source)
        {
            var sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            var outputFolder = String.Format("{0} - {1}", sourceFileName, source.Name);
            Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }
    }
}
