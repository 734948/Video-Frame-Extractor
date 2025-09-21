using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoFrameExtractor
{
    public partial class Form1 : Form
    {
        private Process ffmpegProcess = null;
        private CancellationTokenSource cancellationTokenSource = null;

        public Form1()
        {
            InitializeComponent();
            comboImageFormat.SelectedIndex = 0;
            comboResolution.SelectedIndex = 0;

            btnCancel.Enabled = false;
            progressBarExtraction.Style = ProgressBarStyle.Continuous;
            progressBarExtraction.Value = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv" };
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

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            btnCancel.Enabled = true;
            btnExtract.Enabled = false;
            btnBrowse.Enabled = false;
            progressBarExtraction.Value = 0;

            string videoPath = txtVideoPath.Text;
            string outputFolder = folderBrowserDialog.SelectedPath;
            int nthFrame = (int)numNthFrame.Value;
            string imgFormat = comboImageFormat.SelectedItem.ToString();
            string resolution = comboResolution.SelectedItem.ToString();

            try
            {
                double duration = GetVideoDuration(videoPath);

                await ExtractFramesWithProgressAsync(videoPath, outputFolder, nthFrame, duration, imgFormat, resolution, cancellationTokenSource.Token);

                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    MessageBox.Show("Frame extraction completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Frame extraction cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Frame extraction cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during extraction:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCancel.Enabled = false;
                btnExtract.Enabled = true;
                btnBrowse.Enabled = true;
                progressBarExtraction.Value = 0;
            }
        }

        private double GetVideoDuration(string videoPath)
        {
            string ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
            if (!File.Exists(ffprobePath))
                throw new FileNotFoundException("ffprobe.exe not found in app folder.");

            var psi = new ProcessStartInfo(ffprobePath,
                $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{videoPath}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (double.TryParse(output, out double duration)) return duration;
            }
            return 0;
        }

        private async Task ExtractFramesWithProgressAsync(string videoPath, string outputFolder, int nthFrame, double duration, string imgFormat, string resolution, CancellationToken cancellationToken)
        {
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");

            string scaleFilter = "";

            if (resolution != "Original")
            {
                var dims = resolution.Split('x');
                if (dims.Length == 2)
                {
                    scaleFilter = $"scale={dims[0]}:{dims[1]},";
                }
            }

            string filter = $"{scaleFilter}select='not(mod(n\\,{nthFrame}))'";

            string args = $"-i \"{videoPath}\" -vf \"{filter}\" -vsync vfr -progress pipe:1 -nostats \"{Path.Combine(outputFolder, $"frame_%04d.{imgFormat}")}\"";

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo(ffmpegPath, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            ffmpegProcess.Start();

            var outputReadTask = Task.Run(async () =>
            {
                while (!ffmpegProcess.HasExited && !ffmpegProcess.StandardOutput.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var line = await ffmpegProcess.StandardOutput.ReadLineAsync();

                    if (line.StartsWith("out_time_ms="))
                    {
                        var outTimeMsStr = line.Substring("out_time_ms=".Length);
                        if (double.TryParse(outTimeMsStr, out double outTimeMs))
                        {
                            double progressTime = outTimeMs / 1000000.0;
                            int percent = (int)((progressTime / duration) * 100);
                            if (percent > 100) percent = 100;

                            this.Invoke((Action)(() =>
                            {
                                progressBarExtraction.Value = percent;
                            }));
                        }
                    }
                }
            }, cancellationToken);

            var processWaitTask = ffmpegProcess.WaitForExitAsync();

            await Task.WhenAny(outputReadTask, processWaitTask);

            if (cancellationToken.IsCancellationRequested)
            {
                if (!ffmpegProcess.HasExited)
                {
                    try
                    {
                        ffmpegProcess.Kill();
                    }
                    catch { }
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(outputReadTask, processWaitTask);

            if (ffmpegProcess.ExitCode != 0 && !cancellationToken.IsCancellationRequested)
            {
                string error = await ffmpegProcess.StandardError.ReadToEndAsync();
                throw new Exception($"FFmpeg error: {error}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                btnCancel.Enabled = false;
            }
        }
    }
}
