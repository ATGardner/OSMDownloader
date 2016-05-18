namespace com.atgardner.OMFG
{
    using packagers;
    using sources;
    using utils;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    partial class MainForm : Form
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly List<string> inputFiles;
        private MainController controller;
        private bool zoomChangeFromCode;

        public MainController MainController
        {
            set
            {
                controller = value;
                controller.ProgressChanged += controller_ProgressChanged;
            }
        }

        public IEnumerable<string> InputFiles
        {
            get
            {
                return inputFiles;
            }
            set
            {
                inputFiles.AddRange(value);
                var fileNames = from f in inputFiles
                                select Path.GetFileName(f);
                txtBxInput.Text = string.Join("; ", fileNames);
                logger.Debug("Input files: {0}", txtBxInput.Text);
            }
        }

        public IEnumerable<int> ZoomLevels
        {
            get
            {
                return from c in flpZoomLevels.Controls.Cast<CheckBox>()
                       where c.Enabled && c.Checked && c != chkBxAll
                       select Convert.ToInt32(c.Text);
            }
            set
            {
                zoomChangeFromCode = true;
                foreach (CheckBox chkBx in flpZoomLevels.Controls)
                {
                    if (chkBx != chkBxAll)
                    {
                        var zoomLevel = Convert.ToInt32(chkBx.Text);
                        var shouldCheck = value.Contains(zoomLevel);
                        if (chkBx.Enabled)
                        {
                            chkBx.Checked = shouldCheck;
                        }
                        else
                        {
                            chkBx.Tag = chkBx.CheckState;
                            if (shouldCheck)
                            {
                                chkBx.CheckState = CheckState.Indeterminate;
                            }
                        }
                    }
                }

                zoomChangeFromCode = false;
            }
        }

        public SourceDescriptor SourceDescriptor
        {
            get
            {
                return cmbMapSource.SelectedItem as SourceDescriptor;
            }
            set
            {
                cmbMapSource.SelectedItem = value;
            }
        }

        public string SourceDescriptorString
        {
            set
            {
                cmbMapSource.ValueMember = value;
            }
        }

        public string OutputFile
        {
            get
            {
                return txtBxOutput.Text;
            }
            set
            {
                txtBxOutput.Text = value;
            }
        }

        public FormatType FormatType
        {
            get
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
            set
            {
                rdBtnBCNav.Checked = value.HasFlag(FormatType.BCNav);
                rdBtnMB.Checked = value.HasFlag(FormatType.MBTiles);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            inputFiles = new List<string>();
            CreateZoomCheckBoxes();
        }

        public MainForm(SourceDescriptor[] sources) : this()
        {
            cmbMapSource.DataSource = sources;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                InputFiles = dlgOpenFile.FileNames;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InputFiles = new string[0];
            OutputFile = string.Empty;
        }

        private void GenerateOutput()
        {
            var zoomLevels = ZoomLevels;
            var zoomCount = zoomLevels.Count();
            if (inputFiles.Count == 0 || zoomCount == 0)
            {
                return;
            }

            var firstInputFileName = Path.GetFileNameWithoutExtension(inputFiles.Last());
            var descriptor = SourceDescriptor;
            OutputFile = string.Format("{0} - {1} - {2}-{3}", firstInputFileName, descriptor.Name, ZoomLevels.Min(), ZoomLevels.Max());
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (inputFiles.Count == 0)
            {
                logger.Warn("No input files selected");
                MessageBox.Show("Please specify an input file or files", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var zoomLevels = ZoomLevels.ToArray();
            var zoomCount = zoomLevels.Count();
            if (zoomCount == 0)
            {
                logger.Warn("No zoom level selected");
                MessageBox.Show("Please chose at least one zoom level", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var descriptor = SourceDescriptor;
            if (string.IsNullOrWhiteSpace(OutputFile))
            {
                GenerateOutput();
            }

            if (!Path.IsPathRooted(OutputFile))
            {
                OutputFile = Path.Combine("output", OutputFile);
            }

            var formatType = FormatType;
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
                await controller.DownloadTilesAsync(inputFiles.ToArray(), zoomLevels, descriptor, OutputFile.Replace("\"", string.Empty), formatType);
            });
            tlpContainer.Enabled = true;
        }

        private void cmbMapSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mapSource = SourceDescriptor;
            if (mapSource != null)
            {
                flpZoomLevels.Enabled = true;
                logger.Debug("Changed source to {0}", mapSource);
                ResetZoomCheckBoxes(mapSource.MinZoom, mapSource.MaxZoom);
                Utils.ConfigLinkLabel(lnk, mapSource.Attribution);
            } else
            {
                flpZoomLevels.Enabled = false;
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

            var zoomLevels = ZoomLevels;
            var mapSource = SourceDescriptor;
            var totalLevels = mapSource.MaxZoom - mapSource.MinZoom + 1;
            var checkState = CheckState.Unchecked;
            var zoomCount = zoomLevels.Count();
            if (zoomCount > 0)
            {
                checkState = CheckState.Indeterminate;
            }

            if (zoomCount == totalLevels)
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
