namespace VideoFrameExtractor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            txtVideoPath = new TextBox();
            btnBrowse = new Button();
            btnExtract = new Button();
            numNthFrame = new NumericUpDown();
            folderBrowserDialog = new FolderBrowserDialog();
            progressBarExtraction = new ProgressBar();
            btnCancel = new Button();
            comboImageFormat = new ComboBox();
            label3 = new Label();
            labelResolution = new Label();
            comboResolution = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)numNthFrame).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(66, 48);
            label1.Name = "label1";
            label1.Size = new Size(119, 20);
            label1.TabIndex = 0;
            label1.Text = "Select Video File";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(66, 161);
            label2.Name = "label2";
            label2.Size = new Size(174, 20);
            label2.TabIndex = 1;
            label2.Text = "Enter Nth Frame Number";
            // 
            // txtVideoPath
            // 
            txtVideoPath.Location = new Point(66, 87);
            txtVideoPath.Name = "txtVideoPath";
            txtVideoPath.ReadOnly = true;
            txtVideoPath.Size = new Size(150, 27);
            txtVideoPath.TabIndex = 2;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(294, 87);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(119, 29);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(66, 378);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(119, 29);
            btnExtract.TabIndex = 4;
            btnExtract.Text = "Extract Frames";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // numNthFrame
            // 
            numNthFrame.Location = new Point(295, 161);
            numNthFrame.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNthFrame.Name = "numNthFrame";
            numNthFrame.Size = new Size(150, 27);
            numNthFrame.TabIndex = 5;
            numNthFrame.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // progressBarExtraction
            // 
            progressBarExtraction.Location = new Point(66, 435);
            progressBarExtraction.Name = "progressBarExtraction";
            progressBarExtraction.Size = new Size(507, 29);
            progressBarExtraction.TabIndex = 6;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(699, 435);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // comboImageFormat
            // 
            comboImageFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            comboImageFormat.FormattingEnabled = true;
            comboImageFormat.Items.AddRange(new object[] { "jpg", "png", "bmp", "tiff" });
            comboImageFormat.Location = new Point(293, 229);
            comboImageFormat.Name = "comboImageFormat";
            comboImageFormat.Size = new Size(151, 28);
            comboImageFormat.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(66, 229);
            label3.Name = "label3";
            label3.Size = new Size(146, 20);
            label3.TabIndex = 9;
            label3.Text = "Select Image Format";
            // 
            // labelResolution
            // 
            labelResolution.AutoSize = true;
            labelResolution.Location = new Point(66, 298);
            labelResolution.Name = "labelResolution";
            labelResolution.Size = new Size(173, 20);
            labelResolution.TabIndex = 12;
            labelResolution.Text = "Select Output Resolution";
            // 
            // comboResolution
            // 
            comboResolution.DropDownStyle = ComboBoxStyle.DropDownList;
            comboResolution.FormattingEnabled = true;
            comboResolution.Items.AddRange(new object[] { "Original", "1920×1080", "1280x720", "854x480", "640x360", "426x240" });
            comboResolution.Location = new Point(293, 298);
            comboResolution.Name = "comboResolution";
            comboResolution.Size = new Size(151, 28);
            comboResolution.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 503);
            Controls.Add(comboResolution);
            Controls.Add(labelResolution);
            Controls.Add(label3);
            Controls.Add(comboImageFormat);
            Controls.Add(btnCancel);
            Controls.Add(progressBarExtraction);
            Controls.Add(numNthFrame);
            Controls.Add(btnExtract);
            Controls.Add(btnBrowse);
            Controls.Add(txtVideoPath);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)numNthFrame).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVideoPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.NumericUpDown numNthFrame;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ProgressBar progressBarExtraction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox comboImageFormat;
        private System.Windows.Forms.Label label3;
        private Label labelResolution;
        private ComboBox comboResolution;
    }
}
