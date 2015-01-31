namespace com.atgardner.OMFG
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.tiles;
    using com.atgardner.OMFG.utils;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private static readonly string SourceFile = @"sources\sources.json";

        private readonly TilesManager manager;
        private MapSource[] sources;

        public MainForm()
        {
            InitializeComponent();
            manager = new TilesManager();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateZoomCheckBoxes();
            try
            {
                sources = await MapSource.LoadSources(SourceFile);
                cmbMapSource.DataSource = sources;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed reading sources", "Missing sources", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                var fileNames = from f in dlgOpenFile.FileNames select Path.GetFileName(f);
                txtBxInput.Text = string.Join("; ", fileNames);
                if (string.IsNullOrWhiteSpace(txtBxOutput.Text))
                {
                    txtBxOutput.Text = Path.GetFileNameWithoutExtension(fileNames.First());
                }
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var inputFiles = dlgOpenFile.FileNames;
            if (inputFiles.Length == 0)
            {
                MessageBox.Show("Please specify an input file or files", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var outputFile = txtBxOutput.Text;
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                MessageBox.Show("Please specify an output file name", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            await Task.Factory.StartNew(() => DownloadTiles(inputFiles, outputFile, zoomLevels, source));
            lblStatus.Text = string.Format("Done Downloading tiles");
            tlpContainer.Enabled = true;
        }

        private async void DownloadTiles(string[] inputFiles, string outputFile, int[] zoomLevels, MapSource source)
        {
            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            UpdateStatus("Done Reading File");
            var coordinates = FileUtils.ExtractCoordinates(inputFiles);
            var tiles = manager.GetTileDefinitions(coordinates, zoomLevels);
            var tasks = manager.GetTileData(source, tiles).ToList();
            var prevPercentage = -1;
            var current = 0;
            var total = tasks.Count;
            using (var packager = new SQLitePackager(outputFile))
            {
                await packager.Init();
                while (tasks.Count > 0)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    var tile = await task;
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
            HtmlUtils.ConfigLinkLabel(lnk, mapSource.Attribution);
        }

        private void CreateZoomCheckBoxes()
        {
            for (int i = 0; i <= 20; i++)
            {
                var chkBx = new CheckBox();
                chkBx.Width = 40;
                chkBx.Text = i.ToString();
                flpZoomLevels.Controls.Add(chkBx);
            }

            var chkBxAll = new CheckBox();
            chkBxAll.Text = "Check All";
            chkBxAll.CheckedChanged += chkBxAll_CheckedChanged;
            flpZoomLevels.Controls.Add(chkBxAll);
        }

        void chkBxAll_CheckedChanged(object sender, EventArgs e)
        {
            var chkBxAll = (CheckBox)sender;
            foreach (CheckBox chkBx in flpZoomLevels.Controls)
            {
                if (chkBx.Enabled)
                {
                    chkBx.Checked = chkBxAll.Checked;
                }
            }
        }

        private int[] GetZoomLevels()
        {
            var levels = new List<int>();
            foreach (CheckBox chkBx in flpZoomLevels.Controls)
            {
                int zoomLevel;
                if (chkBx.Enabled && chkBx.Checked && int.TryParse(chkBx.Text, out zoomLevel))
                {
                    levels.Add(zoomLevel);
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

        private void lnk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var lnk = (LinkLabel)sender;
            lnk.Links[lnk.Links.IndexOf(e.Link)].Visited = true;
            Process.Start(e.Link.LinkData.ToString());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutBox())
            {
                about.ShowDialog(this);
            }
        }
    }
}
