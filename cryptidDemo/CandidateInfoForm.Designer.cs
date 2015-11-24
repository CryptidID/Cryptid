namespace CryptidDemo {
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chainHistory = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.candidateDump.Location = new System.Drawing.Point(6, 6);
            this.candidateDump.Multiline = true;
            this.candidateDump.Name = "candidateDump";
            this.candidateDump.ReadOnly = true;
            this.candidateDump.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.candidateDump.Size = new System.Drawing.Size(495, 161);
            this.candidateDump.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(179, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(515, 199);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.candidateDump);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(507, 173);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chainHistory);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(507, 173);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chain History";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chainHistory
            // 
            this.chainHistory.Location = new System.Drawing.Point(6, 6);
            this.chainHistory.Name = "chainHistory";
            this.chainHistory.Size = new System.Drawing.Size(495, 161);
            this.chainHistory.TabIndex = 0;
            this.chainHistory.UseCompatibleStateImageBehavior = false;
            this.chainHistory.DoubleClick += new System.EventHandler(this.chainHistory_DoubleClick);
            // 
            // CandidateInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 224);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.headshotBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CandidateInfoForm";
            this.Text = "Cryptid ID Info";
            this.Load += new System.EventHandler(this.CandidateInfoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox headshotBox;
        private System.Windows.Forms.TextBox candidateDump;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView chainHistory;
    }
}