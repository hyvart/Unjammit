﻿namespace Jammit
{
  partial class SongWindow
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
      this.button1 = new System.Windows.Forms.Button();
      this.timePos = new System.Windows.Forms.Label();
      this.timeRemain = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.mixerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.seekBar = new System.Windows.Forms.TrackBar();
      this.albumArtwork = new System.Windows.Forms.PictureBox();
      this.waveform1 = new Jammit.Controls.Waveform();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.score1 = new Jammit.Controls.Score();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.seekBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.albumArtwork)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.button1.BackColor = System.Drawing.Color.Transparent;
      this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.button1.FlatAppearance.BorderSize = 0;
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.button1.ForeColor = System.Drawing.Color.White;
      this.button1.Location = new System.Drawing.Point(503, 521);
      this.button1.Margin = new System.Windows.Forms.Padding(0);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(58, 28);
      this.button1.TabIndex = 5;
      this.button1.Text = "Play";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new System.EventHandler(this.playPauseButton_Click);
      // 
      // timePos
      // 
      this.timePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.timePos.AutoSize = true;
      this.timePos.BackColor = System.Drawing.Color.Transparent;
      this.timePos.ForeColor = System.Drawing.Color.White;
      this.timePos.Location = new System.Drawing.Point(500, 401);
      this.timePos.Name = "timePos";
      this.timePos.Size = new System.Drawing.Size(34, 13);
      this.timePos.TabIndex = 6;
      this.timePos.Text = "00:00";
      // 
      // timeRemain
      // 
      this.timeRemain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.timeRemain.AutoSize = true;
      this.timeRemain.BackColor = System.Drawing.Color.Transparent;
      this.timeRemain.ForeColor = System.Drawing.Color.White;
      this.timeRemain.Location = new System.Drawing.Point(851, 401);
      this.timeRemain.Name = "timeRemain";
      this.timeRemain.Size = new System.Drawing.Size(37, 13);
      this.timeRemain.TabIndex = 7;
      this.timeRemain.Text = "-00:00";
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.groupBox1.Controls.Add(this.mixerFlowPanel);
      this.groupBox1.ForeColor = System.Drawing.Color.White;
      this.groupBox1.Location = new System.Drawing.Point(165, 396);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(322, 153);
      this.groupBox1.TabIndex = 14;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Mixer";
      // 
      // mixerFlowPanel
      // 
      this.mixerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mixerFlowPanel.Location = new System.Drawing.Point(3, 16);
      this.mixerFlowPanel.Name = "mixerFlowPanel";
      this.mixerFlowPanel.Size = new System.Drawing.Size(316, 134);
      this.mixerFlowPanel.TabIndex = 0;
      // 
      // seekBar
      // 
      this.seekBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.seekBar.Location = new System.Drawing.Point(540, 398);
      this.seekBar.Name = "seekBar";
      this.seekBar.Size = new System.Drawing.Size(305, 45);
      this.seekBar.TabIndex = 15;
      this.seekBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.seekBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.seekBar_MouseUp);
      // 
      // albumArtwork
      // 
      this.albumArtwork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.albumArtwork.Location = new System.Drawing.Point(12, 402);
      this.albumArtwork.Name = "albumArtwork";
      this.albumArtwork.Size = new System.Drawing.Size(147, 147);
      this.albumArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.albumArtwork.TabIndex = 8;
      this.albumArtwork.TabStop = false;
      // 
      // waveform1
      // 
      this.waveform1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.waveform1.BackColor = System.Drawing.Color.Black;
      this.waveform1.ForeColor = System.Drawing.Color.Green;
      this.waveform1.Location = new System.Drawing.Point(503, 424);
      this.waveform1.Name = "waveform1";
      this.waveform1.SamplePosition = ((long)(0));
      this.waveform1.Samples = ((long)(0));
      this.waveform1.Size = new System.Drawing.Size(385, 94);
      this.waveform1.TabIndex = 18;
      this.waveform1.TabStop = false;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(900, 24);
      this.menuStrip1.TabIndex = 19;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
      this.closeToolStripMenuItem.Text = "Close song";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // score1
      // 
      this.score1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.score1.Location = new System.Drawing.Point(9, 36);
      this.score1.Name = "score1";
      this.score1.SamplePosition = ((long)(0));
      this.score1.Samples = ((long)(0));
      this.score1.Size = new System.Drawing.Size(879, 354);
      this.score1.TabIndex = 20;
      // 
      // SongWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.ClientSize = new System.Drawing.Size(900, 561);
      this.Controls.Add(this.score1);
      this.Controls.Add(this.waveform1);
      this.Controls.Add(this.seekBar);
      this.Controls.Add(this.albumArtwork);
      this.Controls.Add(this.timeRemain);
      this.Controls.Add(this.timePos);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.menuStrip1);
      this.DoubleBuffered = true;
      this.MainMenuStrip = this.menuStrip1;
      this.MinimumSize = new System.Drawing.Size(916, 600);
      this.Name = "SongWindow";
      this.ShowIcon = false;
      this.Text = "Score";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SongWindow_FormClosing);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.seekBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.albumArtwork)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label timePos;
    private System.Windows.Forms.Label timeRemain;
    private System.Windows.Forms.PictureBox albumArtwork;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TrackBar seekBar;
    private System.Windows.Forms.FlowLayoutPanel mixerFlowPanel;
    private Controls.Waveform waveform1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private Controls.Score score1;
  }
}