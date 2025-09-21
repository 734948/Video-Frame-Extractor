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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtVideoPath = new System.Windows.Forms.TextBox();
            btnBrowse = new System.Windows.Forms.Button();
            btnExtract = new System.Windows.Forms.Button();
            numNthFrame = new System.Windows.Forms.NumericUpDown();
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            progressBarExtraction = new System.Windows.Forms.ProgressBar();
            btnCancel = new System.Windows.Forms.Button();
            comboImageFormat = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)numNthFrame).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(66, 48);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(119, 20);
            label1.TabIndex = 0;
            label1.Text = "Select Video File";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(66, 137);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(174, 20);
            label2.TabIndex = 1;
            label2.Text = "Enter Nth Frame Number";
            // 
            // txtVideoPath
            // 
            txtVideoPath.Location = new System.Drawing.Point(66, 87);
            txtVideoPath.Name = "txtVideoPath";
            txtVideoPath.ReadOnly = true;
            txtVideoPath.Size = new System.Drawing.Size(150, 27);
            txtVideoPath.TabIndex = 2;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(294, 87);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(119, 29);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnExtract
            // 
            btnExtract.Location = new System.Drawing.Point(294, 178);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(119, 29);
            btnExtract.TabIndex = 4;
            btnExtract.Text = "Extract Frames";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // numNthFrame
            // 
            numNthFrame.Location = new System.Drawing.Point(66, 178);
            numNthFrame.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNthFrame.Name = "numNthFrame";
            numNthFrame.Size = new System.Drawing.Size(150, 27);
            numNthFrame.TabIndex = 5;
            numNthFrame.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // progressBarExtraction
            // 
            progressBarExtraction.Location = new System.Drawing.Point(66, 333);
            progressBarExtraction.Name = "progressBarExtraction";
            progressBarExtraction.Size = new System.Drawing.Size(507, 29);
            progressBarExtraction.TabIndex = 6;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(631, 333);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(94, 29);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // comboImageFormat
            // 
            comboImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboImageFormat.FormattingEnabled = true;
            comboImageFormat.Items.AddRange(new object[] { "jpg", "png", "bmp", "tiff" });
            comboImageFormat.Location = new System.Drawing.Point(66, 282);
            comboImageFormat.Name = "comboImageFormat";
            comboImageFormat.Size = new System.Drawing.Size(151, 28);
            comboImageFormat.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(66, 234);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(146, 20);
            label3.TabIndex = 9;
            label3.Text = "Select Image Format";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
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
    }
}
