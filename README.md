# VideoFrameExtractor

A Windows Forms application for extracting every Nth frame from video files using FFmpeg. Supports selecting output image format, output resolution scaling, real-time extraction progress, and cancellation feature.

---

## Features

- Extract every Nth frame from any common video format (MP4, AVI, MOV, MKV, etc.)
- Choose output image format (e.g., JPG, PNG)
- Select output resolution (Original or scaled resolutions like 1920x1080, 1280x720, etc.)
- Real-time progress bar displaying extraction progress
- Responsive UI with a cancel button that interrupts the extraction cleanly
- Supports FFmpeg progress reading from standard output for smooth feedback

---

## Installation & Usage

1. Place `ffmpeg.exe` and `ffprobe.exe` in the application directory.
2. Open the application.
3. Browse and select a video file.
4. Choose desired Nth frame extraction rate.
5. Select output image format and resolution.
6. Click Extract and wait for completion or cancel anytime.
7. Extracted frames will be saved in the chosen output folder.

---

## Version History

- **v1.0**: Initial stable version with basic extraction, progress, and cancellation.
- **v1.1**: Added output resolution selector allowing scaled frame extraction.
- **v1.2**: Major update with robust cancel implementation using multi-task async pattern for stable cancellation and UI responsiveness.

---

## Requirements

- Windows OS
- .NET Framework (compatible with your Visual Studio target version)
- FFmpeg and FFprobe binaries

---

## Contribution

Pull requests and suggestions are welcome. Please ensure any features or fixes maintain UI responsiveness and process cancellation stability.

---

## License

Specify your license here if needed, e.g., MIT License.

---

## Contact

For any issues or feature requests, please open an issue on GitHub.
