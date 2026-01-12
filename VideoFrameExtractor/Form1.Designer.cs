using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using LibVLCSharp.WinForms;

namespace VideoFrameExtractor
{
    partial class Form1
    {
        private IContainer components = null;
        private FolderBrowserDialog folderBrowserDialog;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem darkModeToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;

        // Batch Controls
        private Label label1;
        private TextBox txtVideoPath;
        private Button btnBrowse;
        private Label label2;
        private NumericUpDown numNthFrame;
        private Label label3;
        private ComboBox comboImageFormat;
        private Label labelResolution;
        private ComboBox comboResolution;
        private Label labelImageCountTitle;
        private TextBox textBoxImageCount;
        private Button btnExtract;
        private ProgressBar progressBarExtraction;
        private Button btnCancel;

        // Preview Controls
        private Panel panelPreviewControls;
        private VideoView videoView;
        private TrackBar trackBarSeek;
        private Label labelTimestamp;
        private Button btnPlayPause;
        private Button btnBrowsePreview;
        private Button btnExtractFrame;

        // Volume Controls
        private Label labelVolume;
        private TrackBar trackBarVolume;

        private System.Windows.Forms.Timer timer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            folderBrowserDialog = new FolderBrowserDialog();
            menuStrip1 = new MenuStrip();
            viewToolStripMenuItem = new ToolStripMenuItem();
            darkModeToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();

            // Batch Init
            label1 = new Label();
            txtVideoPath = new TextBox();
            btnBrowse = new Button();
            label2 = new Label();
            numNthFrame = new NumericUpDown();
            label3 = new Label();
            comboImageFormat = new ComboBox();
            labelResolution = new Label();
            comboResolution = new ComboBox();
            labelImageCountTitle = new Label();
            textBoxImageCount = new TextBox();
            btnExtract = new Button();
            progressBarExtraction = new ProgressBar();
            btnCancel = new Button();

            // Preview Init
            panelPreviewControls = new Panel();
            videoView = new VideoView();
            trackBarSeek = new TrackBar();
            labelTimestamp = new Label();
            btnPlayPause = new Button();
            btnBrowsePreview = new Button();
            btnExtractFrame = new Button();

            // Volume Init
            labelVolume = new Label();
            trackBarVolume = new TrackBar();

            timer1 = new System.Windows.Forms.Timer(components);

            // Form Defaults
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1200, 750);
            this.Text = "Form1";

            // Menu
            menuStrip1.Items.Add(viewToolStripMenuItem);
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Size = new Size(1200, 24);
            viewToolStripMenuItem.Text = "View";
            viewToolStripMenuItem.DropDownItems.Add(darkModeToolStripMenuItem);
            darkModeToolStripMenuItem.Text = "Dark mode";
            darkModeToolStripMenuItem.CheckOnClick = true;
            darkModeToolStripMenuItem.CheckedChanged += new System.EventHandler(this.darkModeToolStripMenuItem_CheckedChanged);

            // Tabs
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);

            // --- TAB 1: BATCH ---
            tabPage1.Text = "Batch Extraction";
            tabPage1.Padding = new Padding(16);

            label1.Location = new Point(24, 28); label1.Text = "Select Video File"; label1.AutoSize = true;
            txtVideoPath.Location = new Point(230, 24); txtVideoPath.Size = new Size(800, 23);
            btnBrowse.Location = new Point(1040, 23); btnBrowse.Size = new Size(100, 25); btnBrowse.Text = "Browse..."; btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            label2.Location = new Point(24, 68); label2.Text = "Enter Nth Frame Number"; label2.AutoSize = true;
            numNthFrame.Location = new Point(230, 64); numNthFrame.Minimum = 1; numNthFrame.Maximum = 1000000; numNthFrame.Value = 10;

            label3.Location = new Point(24, 108); label3.Text = "Select Image Format"; label3.AutoSize = true;
            comboImageFormat.Location = new Point(230, 104); comboImageFormat.Items.AddRange(new object[] { "png", "jpg", "bmp", "webp" });

            labelResolution.Location = new Point(24, 148); labelResolution.Text = "Select Output Resolution"; labelResolution.AutoSize = true;
            comboResolution.Location = new Point(230, 144); comboResolution.Items.AddRange(new object[] { "Original", "1280x720", "1920x1080", "3840x2160" });

            labelImageCountTitle.Location = new Point(24, 188); labelImageCountTitle.Text = "Estimated output images:"; labelImageCountTitle.AutoSize = true;
            textBoxImageCount.Location = new Point(230, 184); textBoxImageCount.ReadOnly = true;

            btnExtract.Location = new Point(470, 183); btnExtract.Size = new Size(120, 25); btnExtract.Text = "Extract Frames"; btnExtract.Click += new System.EventHandler(this.btnExtract_Click);

            progressBarExtraction.Location = new Point(24, 240); progressBarExtraction.Size = new Size(1030, 24);
            btnCancel.Location = new Point(1064, 239); btnCancel.Size = new Size(100, 25); btnCancel.Text = "Cancel"; btnCancel.Enabled = false; btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            tabPage1.Controls.AddRange(new Control[] { label1, txtVideoPath, btnBrowse, label2, numNthFrame, label3, comboImageFormat, labelResolution, comboResolution, labelImageCountTitle, textBoxImageCount, btnExtract, progressBarExtraction, btnCancel });

            // --- TAB 2: PREVIEW ---
            tabPage2.Text = "Single Frame Extraction";

            // Panel for Controls
            panelPreviewControls.Dock = DockStyle.Bottom;
            panelPreviewControls.Height = 120; // Enough space for buttons/trackbar
            panelPreviewControls.Padding = new Padding(10);

            // Seek Bar
            trackBarSeek.Dock = DockStyle.Top;
            trackBarSeek.Height = 45;
            trackBarSeek.Scroll += new System.EventHandler(this.trackBarSeek_Scroll);

            // Controls
            btnPlayPause.Location = new Point(10, 60); btnPlayPause.Size = new Size(100, 30); btnPlayPause.Text = "Play"; btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            btnBrowsePreview.Location = new Point(120, 60); btnBrowsePreview.Size = new Size(140, 30); btnBrowsePreview.Text = "Browse Video"; btnBrowsePreview.Click += new System.EventHandler(this.btnBrowsePreview_Click);
            btnExtractFrame.Location = new Point(270, 60); btnExtractFrame.Size = new Size(140, 30); btnExtractFrame.Text = "Save Frame"; btnExtractFrame.Click += new System.EventHandler(this.btnExtractFrame_Click);

            // Volume Controls (New)
            labelVolume.Location = new Point(450, 65); labelVolume.AutoSize = true; labelVolume.Text = "Volume:";
            trackBarVolume.Location = new Point(500, 60); trackBarVolume.Size = new Size(150, 45);
            trackBarVolume.Minimum = 0; trackBarVolume.Maximum = 100; trackBarVolume.Value = 100; trackBarVolume.TickStyle = TickStyle.None;
            trackBarVolume.Scroll += new System.EventHandler(this.trackBarVolume_Scroll);

            labelTimestamp.Location = new Point(660, 65); labelTimestamp.AutoSize = true; labelTimestamp.Text = "00:00:00 / 00:00:00";

            panelPreviewControls.Controls.Add(btnPlayPause);
            panelPreviewControls.Controls.Add(btnBrowsePreview);
            panelPreviewControls.Controls.Add(btnExtractFrame);
            panelPreviewControls.Controls.Add(labelVolume);
            panelPreviewControls.Controls.Add(trackBarVolume);
            panelPreviewControls.Controls.Add(labelTimestamp);
            panelPreviewControls.Controls.Add(trackBarSeek); // Docks to top

            videoView.Dock = DockStyle.Fill;
            videoView.BackColor = Color.Black;

            tabPage2.Controls.Add(videoView);
            tabPage2.Controls.Add(panelPreviewControls);

            this.Controls.Add(tabControl1);
            this.Controls.Add(menuStrip1);
            this.MainMenuStrip = menuStrip1;
        }
    }
}
