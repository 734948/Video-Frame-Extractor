# VideoFrameExtractor

A robust Windows Forms application for extracting frames from video files. Features two main modes: **Batch Extraction** for bulk processing using FFmpeg, and **Single Frame Extraction** for precise, interactive snapshot capturing using LibVLC.

---

## ⚡ Quick Start: Download Dependencies
This application requires FFmpeg to work. Before running, please follow these 3 steps:

1.  1. **Download FFmpeg:** [Click here](https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip) to download FFmpeg (Essentials Build).
2.  **Extract Files:** Open the downloaded zip file and go to the `bin` folder.
3.  **Copy & Paste:** Copy `ffmpeg.exe` and `ffprobe.exe` and paste them into the same folder as `VideoFrameExtractor.exe`.

---

## 🚀 Features

### 1. Batch Extraction Tab
- **Bulk Processing:** Extract every Nth frame from any common video format (MP4, AVI, MOV, MKV, etc.).
- **Smart Resolution Filtering:** Automatically detects video resolution and hides "upscaling" options to prevent errors.
- **Customization:** Choose output format (JPG, PNG, BMP, WEBP) and resolution (4K, 2K, 1080p, etc.).
- **Real-time Progress:** Visual progress bar with accurate duration tracking.
- **Robust Cancellation:** Cleanly interrupt the process at any time without corrupting files.

### 2. Single Frame Extraction Tab (New in v2.0)
- **Interactive Player:** Play, pause, and seek through videos to find the perfect moment.
- **Precision Capture:** Pause the video and click "Save Frame" to capture the exact displayed frame.
- **Smart Options Popup:** 
  - Choose Format (JPG, PNG, etc.)
  - Choose Resolution (Original, 1080p, 720p).
  - *Note: Higher resolutions are automatically disabled if the source video is smaller to ensure quality.*
- **Volume Control:** Built-in volume slider for the video preview.
- **Playback Safety:** Automatically pauses video when saving to prevent missed frames.

### 3. General
- **Dark Mode:** Built-in dark theme for comfortable usage.
- **No Upscaling:** Logic ensures you never accidentally upscale an image, preserving quality.

---

## 🛠️ Installation & Usage

1. **Download:** Get the latest Release from the Releases tab.
2. **Setup:** Place `ffmpeg.exe` and `ffprobe.exe` in the application folder (see Quick Start above).
3. **Run:** Open `VideoFrameExtractor.exe`.

### How to Use
**Batch Mode:**
1. Select a video file.
2. Choose "Nth Frame" (e.g., 10 to extract every 10th frame).
3. Select Format and Resolution.
4. Click **Extract**.

**Single Frame Mode:**
1. Switch to the "Single Frame Extraction" tab.
2. Browse and load a video.
3. Use the player to find your specific frame.
4. Click **Save Frame**.
5. A popup will appear—select your desired resolution and format, then click OK.

---

## 📜 Version History

- **v2.0 (Current):** 
  - Added **Single Frame Extraction** tab with interactive video player.
  - Added **Smart Resolution Logic** (greys out invalid resolutions).
  - Added **Volume Controller** for video preview.
  - Added **Dark Mode** toggle.
  - Improved progress bar accuracy for batch processing.
- **v1.2:** Major update with robust cancel implementation using multi-task async pattern.
- **v1.1:** Added output resolution selector allowing scaled frame extraction.
- **v1.0:** Initial stable version with basic extraction, progress, and cancellation.

---

## 💻 Requirements

- Windows OS (10/11 recommended)
- .NET 6.0 or higher (depending on build target)
- FFmpeg and FFprobe binaries (must be in app directory)
- LibVLCSharp (NuGet package included in source)

---

## 📧 Contact

For any issues or feature requests, please open an issue on this GitHub repository.
