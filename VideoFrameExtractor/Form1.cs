using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoFrameExtractor
{
    public partial class Form1 : Form
    {
        private Process ffmpegProcess = null;
        private volatile bool cancelRequested = false;

        public Form1()
        {
            InitializeComponent();
            comboImageFormat.SelectedIndex = 0;
            btnCancel.Enabled = false;
            progressBarExtraction.Style = ProgressBarStyle.Continuous;
            progressBarExtraction.Value = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtVideoPath.Text = ofd.FileName;
            }
        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVideoPath.Text) || !File.Exists(txtVideoPath.Text))
            {
                MessageBox.Show("Please select a valid video file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numNthFrame.Value < 1)
            {
                MessageBox.Show("Nth frame must be 1 or greater.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            cancelRequested = false;
            btnCancel.Enabled = true;
            btnExtract.Enabled = false;
            btnBrowse.Enabled = false;

            progressBarExtraction.Value = 0;
            progressBarExtraction.Style = ProgressBarStyle.Continuous;

            string videoPath = txtVideoPath.Text;
            string outputFolder = folderBrowserDialog.SelectedPath;
            int nthFrame = (int)numNthFrame.Value;
            string imgFormat = comboImageFormat.SelectedItem.ToString();

            try
            {
                double duration = GetVideoDuration(videoPath);

                ffmpegProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                        Arguments = $"-i \"{videoPath}\" -vf \"select='not(mod(n\\,{nthFrame}))'\" -vsync vfr -progress pipe:1 -nostats \"{Path.Combine(outputFolder, $"frame_%04d.{imgFormat}")}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                ffmpegProcess.Start();

                Task progressTask = Task.Run(async () =>
                {
                    while (!ffmpegProcess.StandardOutput.EndOfStream)
                    {
                        if (cancelRequested)
                        {
                            try { ffmpegProcess.Kill(); }
                            catch { }
                            break;
                        }
                        string line = await ffmpegProcess.StandardOutput.ReadLineAsync();

                        if (line.StartsWith("out_time_ms="))
                        {
                            if (double.TryParse(line.Substring("out_time_ms=".Length), out double outTimeMs))
                            {
                                int percent = (int)((outTimeMs / 1000000.0) / duration * 100);
                                if (percent > 100) percent = 100;
                                progressBarExtraction.Invoke(new Action(() => progressBarExtraction.Value = percent));
                            }
                        }
                    }
                });

                await Task.WhenAny(progressTask, Task.Run(() => ffmpegProcess.WaitForExit()));

                if (cancelRequested)
                {
                    MessageBox.Show("Extraction cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ffmpegProcess.ExitCode != 0)
                {
                    MessageBox.Show("FFmpeg error occurred during extraction.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Extraction completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                ffmpegProcess?.Dispose();
                ffmpegProcess = null;

                btnCancel.Enabled = false;
                btnExtract.Enabled = true;
                btnBrowse.Enabled = true;

                progressBarExtraction.Value = 0;
            }
        }

        private double GetVideoDuration(string videoPath)
        {
            string ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
            if (!File.Exists(ffprobePath)) throw new FileNotFoundException("ffprobe.exe not found in application folder.");

            ProcessStartInfo psi = new ProcessStartInfo(ffprobePath, $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{videoPath}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (double.TryParse(output, out double duration)) return duration;
            }

            return 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!cancelRequested)
            {
                cancelRequested = true;
                btnCancel.Enabled = false;

                try
                {
                    ffmpegProcess?.Kill();
                }
                catch
                {
                    // ignore if already exited
                }
            }
        }
    }
}
