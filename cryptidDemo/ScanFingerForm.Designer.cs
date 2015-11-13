namespace cryptidDemo {
    partial class ScanFingerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.stateText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // stateText
            // 
            this.stateText.AutoSize = true;
            this.stateText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateText.ForeColor = System.Drawing.Color.Snow;
            this.stateText.Location = new System.Drawing.Point(0, 0);
            this.stateText.Name = "stateText";
            this.stateText.Padding = new System.Windows.Forms.Padding(20);
            this.stateText.Size = new System.Drawing.Size(236, 60);
            this.stateText.TabIndex = 1;
            this.stateText.Text = "Place Finger on Sensor";
            this.stateText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScanFingerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(239, 64);
            this.Controls.Add(this.stateText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ScanFingerForm";
            this.Text = "Scan Finger";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ScanFingerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label stateText;
    }
}