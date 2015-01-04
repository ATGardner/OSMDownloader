﻿namespace com.atgardner.TilesDownloader
{
    partial class TilesDownloaderForm
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
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblMapSource = new System.Windows.Forms.Label();
            this.cmbMapSource = new System.Windows.Forms.ComboBox();
            this.lblZoomLevels = new System.Windows.Forms.Label();
            this.flpZoomLevels = new System.Windows.Forms.FlowLayoutPanel();
            this.chkBxZip = new System.Windows.Forms.CheckBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.status = new System.Windows.Forms.StatusStrip();
            this.prgBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspContainer = new System.Windows.Forms.ToolStripContainer();
            this.tlpContainer.SuspendLayout();
            this.status.SuspendLayout();
            this.tspContainer.BottomToolStripPanel.SuspendLayout();
            this.tspContainer.ContentPanel.SuspendLayout();
            this.tspContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpContainer
            // 
            this.tlpContainer.ColumnCount = 3;
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpContainer.Controls.Add(this.lblInput, 0, 0);
            this.tlpContainer.Controls.Add(this.txtBxInput, 1, 0);
            this.tlpContainer.Controls.Add(this.btnBrowse, 2, 0);
            this.tlpContainer.Controls.Add(this.lblMapSource, 0, 1);
            this.tlpContainer.Controls.Add(this.cmbMapSource, 1, 1);
            this.tlpContainer.Controls.Add(this.lblZoomLevels, 0, 2);
            this.tlpContainer.Controls.Add(this.flpZoomLevels, 1, 2);
            this.tlpContainer.Controls.Add(this.chkBxZip, 0, 4);
            this.tlpContainer.Controls.Add(this.btnRun, 2, 4);
            this.tlpContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContainer.Location = new System.Drawing.Point(0, 0);
            this.tlpContainer.Name = "tlpContainer";
            this.tlpContainer.Padding = new System.Windows.Forms.Padding(10);
            this.tlpContainer.RowCount = 5;
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpContainer.Size = new System.Drawing.Size(584, 314);
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
            this.txtBxInput.Location = new System.Drawing.Point(90, 14);
            this.txtBxInput.Name = "txtBxInput";
            this.txtBxInput.Size = new System.Drawing.Size(410, 20);
            this.txtBxInput.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.AutoSize = true;
            this.btnBrowse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowse.Location = new System.Drawing.Point(506, 13);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(26, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblMapSource
            // 
            this.lblMapSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMapSource.AutoSize = true;
            this.lblMapSource.Location = new System.Drawing.Point(13, 46);
            this.lblMapSource.Name = "lblMapSource";
            this.lblMapSource.Size = new System.Drawing.Size(68, 13);
            this.lblMapSource.TabIndex = 6;
            this.lblMapSource.Text = "Map Source:";
            // 
            // cmbMapSource
            // 
            this.cmbMapSource.DisplayMember = "Name";
            this.cmbMapSource.FormattingEnabled = true;
            this.cmbMapSource.Location = new System.Drawing.Point(90, 42);
            this.cmbMapSource.Name = "cmbMapSource";
            this.cmbMapSource.Size = new System.Drawing.Size(121, 21);
            this.cmbMapSource.TabIndex = 7;
            this.cmbMapSource.SelectedIndexChanged += new System.EventHandler(this.cmbMapSource_SelectedIndexChanged);
            // 
            // lblZoomLevels
            // 
            this.lblZoomLevels.AutoSize = true;
            this.lblZoomLevels.Location = new System.Drawing.Point(13, 66);
            this.lblZoomLevels.Name = "lblZoomLevels";
            this.lblZoomLevels.Size = new System.Drawing.Size(71, 13);
            this.lblZoomLevels.TabIndex = 9;
            this.lblZoomLevels.Text = "Zoom Levels:";
            // 
            // flpZoomLevels
            // 
            this.flpZoomLevels.AutoSize = true;
            this.flpZoomLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpZoomLevels.Location = new System.Drawing.Point(90, 69);
            this.flpZoomLevels.Name = "flpZoomLevels";
            this.flpZoomLevels.Size = new System.Drawing.Size(410, 7);
            this.flpZoomLevels.TabIndex = 8;
            // 
            // chkBxZip
            // 
            this.chkBxZip.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkBxZip.AutoSize = true;
            this.tlpContainer.SetColumnSpan(this.chkBxZip, 2);
            this.chkBxZip.Location = new System.Drawing.Point(13, 281);
            this.chkBxZip.Name = "chkBxZip";
            this.chkBxZip.Size = new System.Drawing.Size(153, 17);
            this.chkBxZip.TabIndex = 10;
            this.chkBxZip.Text = "Zip for Mobile Atlas Creator";
            this.chkBxZip.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.AutoSize = true;
            this.btnRun.Location = new System.Drawing.Point(506, 278);
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
            this.dlgOpenFile.Filter = "Google Earth Files (*.kml, *.kmz)|*.kml;*.kmz|All files|*.*";
            this.dlgOpenFile.SupportMultiDottedExtensions = true;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
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
            this.tspContainer.ContentPanel.Size = new System.Drawing.Size(584, 314);
            this.tspContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tspContainer.Location = new System.Drawing.Point(0, 0);
            this.tspContainer.Name = "tspContainer";
            this.tspContainer.Size = new System.Drawing.Size(584, 361);
            this.tspContainer.TabIndex = 2;
            this.tspContainer.Text = "toolStripContainer1";
            // 
            // TilesDownloaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.tspContainer);
            this.Name = "TilesDownloaderForm";
            this.Text = "Tiles Downloader";
            this.tlpContainer.ResumeLayout(false);
            this.tlpContainer.PerformLayout();
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.tspContainer.BottomToolStripPanel.ResumeLayout(false);
            this.tspContainer.BottomToolStripPanel.PerformLayout();
            this.tspContainer.ContentPanel.ResumeLayout(false);
            this.tspContainer.ResumeLayout(false);
            this.tspContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpContainer;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtBxInput;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripContainer tspContainer;
        private System.Windows.Forms.ToolStripProgressBar prgBar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblMapSource;
        private System.Windows.Forms.ComboBox cmbMapSource;
        private System.Windows.Forms.Label lblZoomLevels;
        private System.Windows.Forms.FlowLayoutPanel flpZoomLevels;
        private System.Windows.Forms.CheckBox chkBxZip;
    }
}

