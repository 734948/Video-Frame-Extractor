using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VideoFrameExtractor
{
    public partial class Form1 : Form
    {
        // Fields for Batch Processing
        private Process ffmpegProcess = null;
        private CancellationTokenSource cancellationTokenSource = null;
        private long totalFrames = 0;
        private string lastOutputFolder = string.Empty;

        // Fields for Video Player
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private string currentVideoPath;
        private int _videoWidth = 0;
        private int _videoHeight = 0;

        // Fields for Fullscreen Logic
        private bool _isFullscreen = false;
        private FormBorderStyle _prevBorderStyle;
        private FormWindowState _prevWindowState;
        private bool _prevTopMost;
        private Rectangle _prevBounds;

        public Form1()
        {
            InitializeComponent();

            // Init TrackBar Range
            trackBarSeek.Minimum = 0;
            trackBarSeek.Maximum = 1000;
            trackBarSeek.Value = 0;

            // Init LibVLC
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = _mediaPlayer;

            // Media Player Events
            _mediaPlayer.Paused += (s, e) => Invoke((Action)(() => { btnPlayPause.Text = "Play"; timer1.Stop(); }));
            _mediaPlayer.Playing += (s, e) => Invoke((Action)(() => { btnPlayPause.Text = "Pause"; timer1.Start(); }));
            _mediaPlayer.Stopped += (s, e) => Invoke((Action)(() => { btnPlayPause.Text = "Play"; timer1.Stop(); trackBarSeek.Value = 0; }));

            timer1.Interval = 500;
            timer1.Tick += Timer1_Tick;

            // UI Defaults
            if (comboImageFormat.Items.Count > 0) comboImageFormat.SelectedIndex = 0;
            if (comboResolution.Items.Count > 0) comboResolution.SelectedIndex = 0;
            labelTimestamp.Text = "00:00:00 / 00:00:00";

            // Event Wiring
            txtVideoPath.TextChanged += async (s, e) => await OnBatchVideoSelectedAsync();
            numNthFrame.ValueChanged += async (s, e) => await UpdatePreviewImageCountAsync();

            // Fullscreen Logic
            videoView.DoubleClick += (_, __) => ToggleFullscreen();
            this.KeyPreview = true;
            this.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.F11 || (e.KeyCode == Keys.Escape && _isFullscreen))
                {
                    e.Handled = true; ToggleFullscreen();
                }
            };

            this.Load += (_, __) => LayoutBatchTab();
            this.Resize += (_, __) => LayoutBatchTab();
        }

        private void darkModeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            bool isDark = darkModeToolStripMenuItem.Checked;
            this.BackColor = isDark ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
            this.ForeColor = isDark ? Color.White : Color.Black;
            UpdateTheme(this.Controls, isDark);
        }

        private void UpdateTheme(Control.ControlCollection controls, bool isDark)
        {
            Color darkBack = Color.FromArgb(60, 60, 60);
            Color darkText = Color.White;
            foreach (Control c in controls)
            {
                if (c is LibVLCSharp.WinForms.VideoView) continue;
                if (c is Button || c is TextBox || c is ComboBox || c is NumericUpDown)
                {
                    c.BackColor = isDark ? darkBack : SystemColors.Window;
                    c.ForeColor = isDark ? darkText : SystemColors.ControlText;
                }
                else if (c is Label || c is CheckBox || c is Panel)
                {
                    c.ForeColor = isDark ? darkText : SystemColors.ControlText;
                    if (c is Panel) c.BackColor = isDark ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
                }
                if (c.HasChildren) UpdateTheme(c.Controls, isDark);
            }
        }

        private void LayoutBatchTab()
        {
            if (tabPage1 == null || tabPage1.IsDisposed) return;
            int rightMargin = 24;
            int clientRight = tabPage1.ClientSize.Width - rightMargin;
            if (clientRight < 200) return;
            btnBrowse.Left = clientRight - btnBrowse.Width;
            txtVideoPath.Width = btnBrowse.Left - 10 - txtVideoPath.Left;
            btnCancel.Left = clientRight - btnCancel.Width;
            progressBarExtraction.Width = btnCancel.Left - 10 - progressBarExtraction.Left;
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _prevBorderStyle = this.FormBorderStyle; _prevWindowState = this.WindowState;
                _prevTopMost = this.TopMost; _prevBounds = this.Bounds;
                this.FormBorderStyle = FormBorderStyle.None; this.WindowState = FormWindowState.Maximized;
                this.TopMost = true; _isFullscreen = true;
            }
            else
            {
                this.TopMost = _prevTopMost; this.WindowState = _prevWindowState;
                this.FormBorderStyle = _prevBorderStyle; if (_prevWindowState == FormWindowState.Normal) this.Bounds = _prevBounds;
                _isFullscreen = false;
            }
        }

        // --- BATCH LOGIC ---

        private async Task OnBatchVideoSelectedAsync()
        {
            if (!File.Exists(txtVideoPath.Text)) return;

            // Get Frame Count
            try
            {
                totalFrames = await GetVideoFrameCountAsync(txtVideoPath.Text);
                await UpdatePreviewImageCountAsync();
            }
            catch { }

            // Get Resolution & Update Dropdown
            try
            {
                var size = await GetVideoResolutionAsync(txtVideoPath.Text);
                PopulateResolutionDropdown(comboResolution, size.Width, size.Height);
            }
            catch { }
        }

        private void PopulateResolutionDropdown(ComboBox combo, int vidW, int vidH)
        {
            combo.Items.Clear();
            combo.Items.Add($"Original ({vidW}x{vidH})");

            var standards = new (string Name, int W, int H)[] {
                ("4K", 3840, 2160),
                ("2K", 2560, 1440),
                ("1080p", 1920, 1080),
                ("720p", 1280, 720),
                ("480p", 854, 480),
                ("360p", 640, 360)
            };

            foreach (var s in standards)
            {
                if (vidW >= s.W || vidH >= s.H)
                {
                    combo.Items.Add($"{s.Name} ({s.W}x{s.H})");
                }
            }
            combo.SelectedIndex = 0;
        }

        private Task UpdatePreviewImageCountAsync()
        {
            if (totalFrames > 0 && numNthFrame.Value > 0)
            {
                long c = totalFrames / (long)numNthFrame.Value;
                textBoxImageCount.Text = c.ToString();
            }
            return Task.CompletedTask;
        }

        private Task<long> GetVideoFrameCountAsync(string path) => Task.Run(() => {
            string ff = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
            if (!File.Exists(ff)) return 0L;
            var psi = new ProcessStartInfo(ff, $"-v error -select_streams v:0 -count_frames -show_entries stream=nb_read_frames -of default=noprint_wrappers=1:nokey=1 \"{path}\"") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
            using (var p = Process.Start(psi))
            {
                string o = p.StandardOutput.ReadToEnd(); p.WaitForExit();
                return long.TryParse(o.Trim(), out long v) ? v : 0L;
            }
        });

        private Task<(int Width, int Height)> GetVideoResolutionAsync(string path) => Task.Run(() => {
            string ff = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
            if (!File.Exists(ff)) return (0, 0);

            var psi = new ProcessStartInfo(ff, $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{path}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();
                var parts = output.Split('x');
                if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int h))
                {
                    return (w, h);
                }
                return (0, 0);
            }
        });

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog { Filter = "Videos|*.mp4;*.mkv;*.avi;*.mov" };
            if (d.ShowDialog() == DialogResult.OK) txtVideoPath.Text = d.FileName;
        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtVideoPath.Text)) return;
            if (Directory.Exists(lastOutputFolder)) folderBrowserDialog.SelectedPath = lastOutputFolder;
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
            lastOutputFolder = folderBrowserDialog.SelectedPath;

            cancellationTokenSource = new CancellationTokenSource();
            btnExtract.Enabled = false; btnCancel.Enabled = true; progressBarExtraction.Value = 0;

            try
            {
                double durationSec = GetVideoDuration(txtVideoPath.Text);
                string selRes = (comboResolution.SelectedItem ?? "Original").ToString();
                string scaleFilter = "";

                if (!selRes.StartsWith("Original"))
                {
                    var match = Regex.Match(selRes, @"\((\d+)x(\d+)\)");
                    if (match.Success)
                    {
                        scaleFilter = $"scale={match.Groups[1].Value}:{match.Groups[2].Value},";
                    }
                }

                string filter = $"{scaleFilter}select='not(mod(n\\,{(int)numNthFrame.Value}))'";
                string ff = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
                var psi = new ProcessStartInfo(ff, $"-i \"{txtVideoPath.Text}\" -vf \"{filter}\" -vsync vfr -progress pipe:1 -nostats \"{Path.Combine(lastOutputFolder, $"frame_%04d.{(comboImageFormat.SelectedItem ?? "png")}")}\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                ffmpegProcess = Process.Start(psi);

                await Task.Run(async () => {
                    while (!ffmpegProcess.HasExited)
                    {
                        string line = await ffmpegProcess.StandardOutput.ReadLineAsync();
                        if (line == null) break;
                        if (line.StartsWith("out_time_ms="))
                        {
                            if (long.TryParse(line.Substring(12), out long micros))
                            {
                                double seconds = micros / 1000000.0;
                                int pct = (durationSec > 0) ? (int)((seconds / durationSec) * 100) : 0;
                                Invoke((Action)(() => progressBarExtraction.Value = Math.Min(100, Math.Max(0, pct))));
                            }
                        }
                    }
                });

                await ffmpegProcess.WaitForExitAsync(cancellationTokenSource.Token);

                if (cancellationTokenSource.IsCancellationRequested) MessageBox.Show("Cancelled");
                else { progressBarExtraction.Value = 100; MessageBox.Show("Done!"); }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { btnExtract.Enabled = true; btnCancel.Enabled = false; }
        }

        private double GetVideoDuration(string path)
        {
            try
            {
                string ff = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
                if (!File.Exists(ff)) return 0;
                var psi = new ProcessStartInfo(ff, $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{path}\"") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
                using (var p = Process.Start(psi))
                {
                    string o = p.StandardOutput.ReadToEnd(); p.WaitForExit();
                    return double.TryParse(o.Trim(), out double d) ? d : 0;
                }
            }
            catch { return 0; }
        }

        private void btnCancel_Click(object sender, EventArgs e) => cancellationTokenSource?.Cancel();

        // --- PREVIEW LOGIC ---
        private async void btnBrowsePreview_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog { Filter = "Videos|*.mp4;*.mkv;*.avi;*.mov" };
            if (d.ShowDialog() == DialogResult.OK)
            {
                currentVideoPath = d.FileName;

                // 1. Get Resolution using FFprobe (Reliable)
                try
                {
                    var size = await GetVideoResolutionAsync(currentVideoPath);
                    _videoWidth = size.Width;
                    _videoHeight = size.Height;
                }
                catch
                {
                    _videoWidth = 0; _videoHeight = 0;
                }

                // 2. Play Video
                var media = new Media(_libVLC, new Uri(currentVideoPath));
                _mediaPlayer.Media = media;
                _mediaPlayer.Play();
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer.IsPlaying) _mediaPlayer.Pause(); else _mediaPlayer.Play();
        }

        private void btnExtractFrame_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer.Media == null) return;

            // Only pause if currently playing.
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
            }

            // Pass the FFprobe-detected dimensions
            using (var dlg = new SnapshotOptionsDialog(_videoWidth, _videoHeight))
            {
                if (darkModeToolStripMenuItem.Checked) dlg.EnableDarkMode();

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string ext = dlg.SelectedFormat;
                    uint w = dlg.SelectedWidth;
                    uint h = dlg.SelectedHeight;

                    if (Directory.Exists(lastOutputFolder)) folderBrowserDialog.SelectedPath = lastOutputFolder;
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        lastOutputFolder = folderBrowserDialog.SelectedPath;
                        string filename = $"snap_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
                        string fullPath = Path.Combine(lastOutputFolder, filename);

                        if (_mediaPlayer.TakeSnapshot(0, fullPath, w, h)) MessageBox.Show($"Saved to: {filename}");
                        else
                        {
                            MessageBox.Show("Failed. Resetting player...");
                            _mediaPlayer.Stop();
                        }
                    }
                }
            }
        }

        private void trackBarSeek_Scroll(object sender, EventArgs e)
        {
            if (_mediaPlayer.Media != null) _mediaPlayer.Position = trackBarSeek.Value / 1000f;
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = trackBarVolume.Value;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
            {
                float pos = _mediaPlayer.Position;
                int newVal = (int)(pos * trackBarSeek.Maximum);
                if (newVal < trackBarSeek.Minimum) newVal = trackBarSeek.Minimum;
                if (newVal > trackBarSeek.Maximum) newVal = trackBarSeek.Maximum;
                trackBarSeek.Value = newVal;
                labelTimestamp.Text = $"{TimeSpan.FromMilliseconds(_mediaPlayer.Time):hh\\:mm\\:ss} / {TimeSpan.FromMilliseconds(_mediaPlayer.Length):hh\\:mm\\:ss}";
            }
        }
    }

    // ==========================================
    // SMART POPUP (Hides Upscaling Options)
    // ==========================================
    public class SnapshotOptionsDialog : Form
    {
        private ComboBox cmbFormat;
        private ComboBox cmbResolution;
        private Button btnOK;
        private Button btnCancel;
        private Label lblFormat;
        private Label lblRes;

        public string SelectedFormat => cmbFormat.SelectedItem.ToString();
        public uint SelectedWidth { get; private set; } = 0;
        public uint SelectedHeight { get; private set; } = 0;

        public SnapshotOptionsDialog(int vidW, int vidH)
        {
            this.Text = "Save Frame Options";
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            lblFormat = new Label { Text = "Image Format:", Location = new Point(20, 20), AutoSize = true };
            cmbFormat = new ComboBox { Location = new Point(120, 18), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFormat.Items.AddRange(new object[] { "jpg", "png", "bmp", "webp" });
            cmbFormat.SelectedIndex = 0;

            lblRes = new Label { Text = "Resolution:", Location = new Point(20, 60), AutoSize = true };
            cmbResolution = new ComboBox { Location = new Point(120, 58), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            // Add Original
            cmbResolution.Items.Add($"Original ({vidW}x{vidH})");

            var standards = new (string Name, int W, int H)[] {
                ("4K", 3840, 2160),
                ("2K", 2560, 1440),
                ("1080p", 1920, 1080),
                ("720p", 1280, 720),
                ("480p", 854, 480)
            };

            foreach (var s in standards)
            {
                // HIDE if video is smaller than this option
                if ((vidW == 0 && vidH == 0) || (vidW >= s.W || vidH >= s.H))
                {
                    cmbResolution.Items.Add($"{s.Name} ({s.W}x{s.H})");
                }
            }
            cmbResolution.SelectedIndex = 0;

            // Updated Button Sizes and Positions
            btnOK = new Button { Text = "OK", Location = new Point(130, 110), DialogResult = DialogResult.OK, Height = 30, Width = 80 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(220, 110), DialogResult = DialogResult.Cancel, Height = 30, Width = 80 };

            btnOK.Click += BtnOK_Click;

            this.Controls.AddRange(new Control[] { lblFormat, cmbFormat, lblRes, cmbResolution, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        public void EnableDarkMode()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            cmbFormat.BackColor = Color.FromArgb(60, 60, 60);
            cmbFormat.ForeColor = Color.White;

            cmbResolution.BackColor = Color.FromArgb(60, 60, 60);
            cmbResolution.ForeColor = Color.White;

            btnOK.BackColor = Color.FromArgb(60, 60, 60);
            btnOK.ForeColor = Color.White;

            btnCancel.BackColor = Color.FromArgb(60, 60, 60);
            btnCancel.ForeColor = Color.White;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SelectedWidth = 0;
            SelectedHeight = 0;
            string res = cmbResolution.SelectedItem.ToString();

            if (!res.StartsWith("Original"))
            {
                var match = Regex.Match(res, @"\((\d+)x(\d+)\)");
                if (match.Success)
                {
                    SelectedWidth = uint.Parse(match.Groups[1].Value);
                    SelectedHeight = uint.Parse(match.Groups[2].Value);
                }
            }
        }
    }
}
