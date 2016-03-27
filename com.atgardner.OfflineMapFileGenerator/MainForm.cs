namespace com.atgardner.OMFG
{
    using com.atgardner.OMFG.packagers;
    using com.atgardner.OMFG.sources;
    using com.atgardner.OMFG.utils;
    using NLog;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string SourceFile = @"sources\sources.json";

        private readonly List<string> inputFiles;
        private readonly MainController controller;
        private bool zoomChangeFromCode;

        public MainForm()
        {
            InitializeComponent();
            inputFiles = new List<string>();
            controller = new MainController();
            controller.ProgressChanged += controller_ProgressChanged;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateZoomCheckBoxes();
            try
            {
                await controller.InitAsync(SourceFile);
                cmbMapSource.DataSource = controller.Sources;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Failed reading sources");
                MessageBox.Show("Failed reading sources", "Missing sources", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                inputFiles.AddRange(dlgOpenFile.FileNames);
                var fileNames = from f in inputFiles
                                select Path.GetFileName(f);
                txtBxInput.Text = string.Join("; ", fileNames);
                logger.Debug("Input files: {0}", txtBxInput.Text);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            inputFiles.Clear();
            txtBxInput.Text = string.Empty;
            txtBxOutput.Text = string.Empty;
        }

        private void btnGenerateOutput_Click(object sender, EventArgs e)
        {
            GenerateOutput();
        }

        private void GenerateOutput()
        {
            var zoomLevels = GetZoomLevels();
            if (inputFiles.Count == 0 || zoomLevels.Length == 0)
            {
                return;
            }

            var firstInputFileName = Path.GetFileNameWithoutExtension(inputFiles.Last());
            var descriptor = cmbMapSource.SelectedItem as SourceDescriptor;
            txtBxOutput.Text = string.Format("{0} - {1} - {2}-{3}", firstInputFileName, descriptor.Name, zoomLevels.Min(), zoomLevels.Max());
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (inputFiles.Count == 0)
            {
                logger.Warn("No input files selected");
                MessageBox.Show("Please specify an input file or files", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var zoomLevels = GetZoomLevels();
            if (zoomLevels.Length == 0)
            {
                logger.Warn("No zoom level selected");
                MessageBox.Show("Please chose at least one zoom level", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var descriptor = cmbMapSource.SelectedItem as SourceDescriptor;
            string outputFile = txtBxOutput.Text;
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                GenerateOutput();
            }

            if (!Path.IsPathRooted(outputFile))
            {
                outputFile = Path.Combine("output", outputFile);
            }

            var formatType = getFormatType();
            if (formatType == FormatType.None)
            {
                logger.Warn("No output format selected");
                MessageBox.Show("Please specify at least one output format", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tlpContainer.Enabled = false;
            prgBar.Value = 0;
            await Task.Run(async () =>
            {
                await controller.DownloadTilesAsync(inputFiles.ToArray(), zoomLevels, descriptor, outputFile.Replace("\"", string.Empty), formatType);
            });
            tlpContainer.Enabled = true;
        }

        private FormatType getFormatType()
        {
            var result = FormatType.None;
            if (rdBtnBCNav.Checked)
            {
                result |= FormatType.BCNav;
            }

            if (rdBtnMB.Checked)
            {
                result |= FormatType.MBTiles;
            }

            return result;
        }

        private void cmbMapSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mapSource = cmbMapSource.SelectedItem as SourceDescriptor;
            if (mapSource != null)
            {
                logger.Debug("Changed source to {0}", mapSource);
                ResetZoomCheckBoxes(mapSource.MinZoom, mapSource.MaxZoom);
                Utils.ConfigLinkLabel(lnk, mapSource.Attribution);
            }
        }

        private void CreateZoomCheckBoxes()
        {
            for (int i = 0; i <= 20; i++)
            {
                var chkBx = new CheckBox();
                chkBx.Width = 40;
                chkBx.Text = i.ToString();
                chkBx.CheckedChanged += chkBxZoom_CheckedChanged;
                flpZoomLevels.Controls.Add(chkBx);
            }

            flpZoomLevels.Controls.SetChildIndex(chkBxAll, 21);
        }

        private void chkBxZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (zoomChangeFromCode)
            {
                return;
            }

            var zoomLevels = GetZoomLevels();
            var mapSource = cmbMapSource.SelectedItem as SourceDescriptor;
            var totalLevels = mapSource.MaxZoom - mapSource.MinZoom + 1;
            var checkState = CheckState.Unchecked;
            if (zoomLevels.Length > 0)
            {
                checkState = CheckState.Indeterminate;
            }

            if (zoomLevels.Length == totalLevels)
            {
                checkState = CheckState.Checked;
            }

            zoomChangeFromCode = true;
            chkBxAll.CheckState = checkState;
            zoomChangeFromCode = false;
        }

        private void chkBxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (zoomChangeFromCode)
            {
                return;
            }

            var chkBxAll = (CheckBox)sender;
            zoomChangeFromCode = true;
            foreach (CheckBox chkBx in flpZoomLevels.Controls)
            {
                if (chkBx.Enabled)
                {
                    chkBx.Checked = chkBxAll.Checked;
                }
            }

            zoomChangeFromCode = false;
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
            CheckBox checkBox;
            for (int i = 0; i < minZoom; i++)
            {
                checkBox = (CheckBox)flpZoomLevels.Controls[i];
                DisableCheckBox(checkBox);
            }

            for (int i = minZoom; i <= maxZoom; i++)
            {
                checkBox = (CheckBox)flpZoomLevels.Controls[i];
                EnableCheckBox(checkBox);
            }

            for (int i = maxZoom + 1; i <= 20; i++)
            {
                checkBox = (CheckBox)flpZoomLevels.Controls[i];
                DisableCheckBox(checkBox);
            }
        }

        private void controller_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                if (e.ProgressPercentage >= 0)
                {
                    prgBar.Value = e.ProgressPercentage;
                }

                lblStatus.Text = e.UserState as string;
            }));
        }

        private static void DisableCheckBox(CheckBox checkBox)
        {
            checkBox.Tag = checkBox.CheckState;
            if (checkBox.Checked)
            {
                checkBox.CheckState = CheckState.Indeterminate;
            }

            checkBox.Enabled = false;
        }

        private static void EnableCheckBox(CheckBox checkBox)
        {
            if (checkBox.Tag is CheckState)
            {
                checkBox.CheckState = (CheckState)checkBox.Tag;
                checkBox.Tag = null;
            }

            checkBox.Enabled = true;
        }

    }
}
