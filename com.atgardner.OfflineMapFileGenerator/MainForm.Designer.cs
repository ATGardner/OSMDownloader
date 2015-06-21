namespace com.atgardner.OMFG
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpContainer = new System.Windows.Forms.TableLayoutPanel();
            this.lblInput = new System.Windows.Forms.Label();
            this.txtBxInput = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtBxOutput = new System.Windows.Forms.TextBox();
            this.lblMapSource = new System.Windows.Forms.Label();
            this.cmbMapSource = new System.Windows.Forms.ComboBox();
            this.lblZoomLevels = new System.Windows.Forms.Label();
            this.flpZoomLevels = new System.Windows.Forms.FlowLayoutPanel();
            this.chkBxAll = new System.Windows.Forms.CheckBox();
            this.lblTarget = new System.Windows.Forms.Label();
            this.flpTarget = new System.Windows.Forms.FlowLayoutPanel();
            this.rdBtnBCNav = new System.Windows.Forms.RadioButton();
            this.rdBtnOruxMaps = new System.Windows.Forms.RadioButton();
            this.lnk = new System.Windows.Forms.LinkLabel();
            this.btnRun = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.status = new System.Windows.Forms.StatusStrip();
            this.prgBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspContainer = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClear = new System.Windows.Forms.Button();
            this.tlpContainer.SuspendLayout();
            this.flpZoomLevels.SuspendLayout();
            this.flpTarget.SuspendLayout();
            this.status.SuspendLayout();
            this.tspContainer.BottomToolStripPanel.SuspendLayout();
            this.tspContainer.ContentPanel.SuspendLayout();
            this.tspContainer.TopToolStripPanel.SuspendLayout();
            this.tspContainer.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpContainer
            // 
            this.tlpContainer.ColumnCount = 4;
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpContainer.Controls.Add(this.lblInput, 0, 0);
            this.tlpContainer.Controls.Add(this.txtBxInput, 1, 0);
            this.tlpContainer.Controls.Add(this.btnAdd, 2, 0);
            this.tlpContainer.Controls.Add(this.btnClear, 3, 0);
            this.tlpContainer.Controls.Add(this.lblOutput, 0, 1);
            this.tlpContainer.Controls.Add(this.txtBxOutput, 1, 1);
            this.tlpContainer.Controls.Add(this.lblMapSource, 0, 2);
            this.tlpContainer.Controls.Add(this.cmbMapSource, 1, 2);
            this.tlpContainer.Controls.Add(this.lblZoomLevels, 0, 3);
            this.tlpContainer.Controls.Add(this.flpZoomLevels, 1, 3);
            this.tlpContainer.Controls.Add(this.lblTarget, 0, 4);
            this.tlpContainer.Controls.Add(this.flpTarget, 1, 4);
            this.tlpContainer.Controls.Add(this.lnk, 0, 5);
            this.tlpContainer.Controls.Add(this.btnRun, 2, 7);
            this.tlpContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContainer.Location = new System.Drawing.Point(0, 0);
            this.tlpContainer.Name = "tlpContainer";
            this.tlpContainer.Padding = new System.Windows.Forms.Padding(10);
            this.tlpContainer.RowCount = 8;
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.Size = new System.Drawing.Size(584, 315);
            this.tlpContainer.TabIndex = 0;
            // 
            // lblInput
            // 
            this.lblInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(13, 18);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(50, 13);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input file:";
            // 
            // txtBxInput
            // 
            this.txtBxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBxInput.Location = new System.Drawing.Point(95, 14);
            this.txtBxInput.Name = "txtBxInput";
            this.txtBxInput.Size = new System.Drawing.Size(387, 20);
            this.txtBxInput.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.AutoSize = true;
            this.btnAdd.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdd.Location = new System.Drawing.Point(488, 13);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(36, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblOutput
            // 
            this.lblOutput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(13, 45);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(58, 13);
            this.lblOutput.TabIndex = 12;
            this.lblOutput.Text = "Output file:";
            // 
            // txtBxOutput
            // 
            this.txtBxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBxOutput.Location = new System.Drawing.Point(95, 42);
            this.txtBxOutput.Name = "txtBxOutput";
            this.txtBxOutput.Size = new System.Drawing.Size(387, 20);
            this.txtBxOutput.TabIndex = 13;
            // 
            // lblMapSource
            // 
            this.lblMapSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMapSource.AutoSize = true;
            this.lblMapSource.Location = new System.Drawing.Point(13, 72);
            this.lblMapSource.Name = "lblMapSource";
            this.lblMapSource.Size = new System.Drawing.Size(68, 13);
            this.lblMapSource.TabIndex = 6;
            this.lblMapSource.Text = "Map Source:";
            // 
            // cmbMapSource
            // 
            this.cmbMapSource.DisplayMember = "Name";
            this.cmbMapSource.FormattingEnabled = true;
            this.cmbMapSource.Location = new System.Drawing.Point(95, 68);
            this.cmbMapSource.Name = "cmbMapSource";
            this.cmbMapSource.Size = new System.Drawing.Size(121, 21);
            this.cmbMapSource.TabIndex = 7;
            this.cmbMapSource.SelectedIndexChanged += new System.EventHandler(this.cmbMapSource_SelectedIndexChanged);
            // 
            // lblZoomLevels
            // 
            this.lblZoomLevels.AutoSize = true;
            this.lblZoomLevels.Location = new System.Drawing.Point(13, 92);
            this.lblZoomLevels.Name = "lblZoomLevels";
            this.lblZoomLevels.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblZoomLevels.Size = new System.Drawing.Size(71, 17);
            this.lblZoomLevels.TabIndex = 9;
            this.lblZoomLevels.Text = "Zoom Levels:";
            // 
            // flpZoomLevels
            // 
            this.flpZoomLevels.AutoSize = true;
            this.flpZoomLevels.Controls.Add(this.chkBxAll);
            this.flpZoomLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpZoomLevels.Location = new System.Drawing.Point(95, 95);
            this.flpZoomLevels.Name = "flpZoomLevels";
            this.flpZoomLevels.Size = new System.Drawing.Size(387, 30);
            this.flpZoomLevels.TabIndex = 8;
            // 
            // chkBxAll
            // 
            this.chkBxAll.Location = new System.Drawing.Point(3, 3);
            this.chkBxAll.Name = "chkBxAll";
            this.chkBxAll.Size = new System.Drawing.Size(104, 24);
            this.chkBxAll.TabIndex = 16;
            this.chkBxAll.Text = "Check All";
            this.chkBxAll.UseVisualStyleBackColor = true;
            this.chkBxAll.CheckedChanged += new System.EventHandler(this.chkBxAll_CheckedChanged);
            // 
            // lblTarget
            // 
            this.lblTarget.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(13, 136);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(76, 13);
            this.lblTarget.TabIndex = 14;
            this.lblTarget.Text = "Target Format:";
            // 
            // flpTarget
            // 
            this.flpTarget.AutoSize = true;
            this.flpTarget.Controls.Add(this.rdBtnBCNav);
            this.flpTarget.Controls.Add(this.rdBtnOruxMaps);
            this.flpTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTarget.Location = new System.Drawing.Point(95, 131);
            this.flpTarget.Name = "flpTarget";
            this.flpTarget.Size = new System.Drawing.Size(387, 23);
            this.flpTarget.TabIndex = 15;
            // 
            // rdBtnBCNav
            // 
            this.rdBtnBCNav.AutoSize = true;
            this.rdBtnBCNav.Checked = true;
            this.rdBtnBCNav.Location = new System.Drawing.Point(3, 3);
            this.rdBtnBCNav.Name = "rdBtnBCNav";
            this.rdBtnBCNav.Size = new System.Drawing.Size(135, 17);
            this.rdBtnBCNav.TabIndex = 0;
            this.rdBtnBCNav.TabStop = true;
            this.rdBtnBCNav.Text = "BackCountry Navigator";
            this.rdBtnBCNav.UseVisualStyleBackColor = true;
            // 
            // rdBtnOruxMaps
            // 
            this.rdBtnOruxMaps.AutoSize = true;
            this.rdBtnOruxMaps.Location = new System.Drawing.Point(144, 3);
            this.rdBtnOruxMaps.Name = "rdBtnOruxMaps";
            this.rdBtnOruxMaps.Size = new System.Drawing.Size(76, 17);
            this.rdBtnOruxMaps.TabIndex = 1;
            this.rdBtnOruxMaps.TabStop = true;
            this.rdBtnOruxMaps.Text = "Orux Maps";
            this.rdBtnOruxMaps.UseVisualStyleBackColor = true;
            // 
            // lnk
            // 
            this.lnk.AllowDrop = true;
            this.lnk.AutoSize = true;
            this.tlpContainer.SetColumnSpan(this.lnk, 2);
            this.lnk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnk.Location = new System.Drawing.Point(13, 157);
            this.lnk.Name = "lnk";
            this.lnk.Size = new System.Drawing.Size(469, 13);
            this.lnk.TabIndex = 11;
            this.lnk.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_LinkClicked);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.AutoSize = true;
            this.tlpContainer.SetColumnSpan(this.btnRun, 2);
            this.btnRun.Location = new System.Drawing.Point(506, 279);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(65, 23);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "kml";
            this.dlgOpenFile.Filter = "GPS Data Files|*.kml;*.kmz;*.gpx";
            this.dlgOpenFile.Multiselect = true;
            this.dlgOpenFile.SupportMultiDottedExtensions = true;
            // 
            // status
            // 
            this.status.Dock = System.Windows.Forms.DockStyle.None;
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prgBar,
            this.lblStatus});
            this.status.Location = new System.Drawing.Point(0, 0);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(584, 22);
            this.status.TabIndex = 1;
            this.status.Text = "statusStrip1";
            // 
            // prgBar
            // 
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(100, 16);
            this.prgBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tspContainer
            // 
            // 
            // tspContainer.BottomToolStripPanel
            // 
            this.tspContainer.BottomToolStripPanel.Controls.Add(this.status);
            // 
            // tspContainer.ContentPanel
            // 
            this.tspContainer.ContentPanel.Controls.Add(this.tlpContainer);
            this.tspContainer.ContentPanel.Size = new System.Drawing.Size(584, 315);
            this.tspContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tspContainer.Location = new System.Drawing.Point(0, 0);
            this.tspContainer.Name = "tspContainer";
            this.tspContainer.Size = new System.Drawing.Size(584, 361);
            this.tspContainer.TabIndex = 2;
            this.tspContainer.Text = "toolStripContainer1";
            // 
            // tspContainer.TopToolStripPanel
            // 
            this.tspContainer.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Location = new System.Drawing.Point(530, 13);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(41, 23);
            this.btnClear.TabIndex = 16;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.tspContainer);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Offline Map Creator";
            this.tlpContainer.ResumeLayout(false);
            this.tlpContainer.PerformLayout();
            this.flpZoomLevels.ResumeLayout(false);
            this.flpTarget.ResumeLayout(false);
            this.flpTarget.PerformLayout();
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.tspContainer.BottomToolStripPanel.ResumeLayout(false);
            this.tspContainer.BottomToolStripPanel.PerformLayout();
            this.tspContainer.ContentPanel.ResumeLayout(false);
            this.tspContainer.TopToolStripPanel.ResumeLayout(false);
            this.tspContainer.TopToolStripPanel.PerformLayout();
            this.tspContainer.ResumeLayout(false);
            this.tspContainer.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpContainer;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtBxInput;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripContainer tspContainer;
        private System.Windows.Forms.ToolStripProgressBar prgBar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblMapSource;
        private System.Windows.Forms.ComboBox cmbMapSource;
        private System.Windows.Forms.Label lblZoomLevels;
        private System.Windows.Forms.FlowLayoutPanel flpZoomLevels;
        private System.Windows.Forms.LinkLabel lnk;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtBxOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.FlowLayoutPanel flpTarget;
        private System.Windows.Forms.RadioButton rdBtnBCNav;
        private System.Windows.Forms.RadioButton rdBtnOruxMaps;
        private System.Windows.Forms.CheckBox chkBxAll;
        private System.Windows.Forms.Button btnClear;
    }
}

