namespace cryptidDemo {
    partial class CandidateInfoForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CandidateInfoForm));
            this.headshotBox = new System.Windows.Forms.PictureBox();
            this.candidateDump = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).BeginInit();
            this.SuspendLayout();
            // 
            // headshotBox
            // 
            this.headshotBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headshotBox.Location = new System.Drawing.Point(12, 12);
            this.headshotBox.Name = "headshotBox";
            this.headshotBox.Size = new System.Drawing.Size(160, 200);
            this.headshotBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.headshotBox.TabIndex = 1;
            this.headshotBox.TabStop = false;
            // 
            // candidateDump
            // 
            this.candidateDump.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.candidateDump.Location = new System.Drawing.Point(178, 12);
            this.candidateDump.Multiline = true;
            this.candidateDump.Name = "candidateDump";
            this.candidateDump.ReadOnly = true;
            this.candidateDump.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.candidateDump.Size = new System.Drawing.Size(513, 200);
            this.candidateDump.TabIndex = 2;
            // 
            // CandidateInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 224);
            this.Controls.Add(this.candidateDump);
            this.Controls.Add(this.headshotBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CandidateInfoForm";
            this.Text = "Cryptid ID Info";
            this.Load += new System.EventHandler(this.CandidateInfoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox headshotBox;
        private System.Windows.Forms.TextBox candidateDump;
    }
}