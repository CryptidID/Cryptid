namespace cryptidDemo {
    partial class FPSConnectForm {
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
            this.baudrates = new System.Windows.Forms.ComboBox();
            this.ports = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.connectBg = new System.Windows.Forms.Panel();
            this.connectText = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.connectProgress = new System.Windows.Forms.ProgressBar();
            this.connectBg.SuspendLayout();
            this.SuspendLayout();
            // 
            // baudrates
            // 
            this.baudrates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudrates.FormattingEnabled = true;
            this.baudrates.Location = new System.Drawing.Point(139, 32);
            this.baudrates.Name = "baudrates";
            this.baudrates.Size = new System.Drawing.Size(121, 21);
            this.baudrates.TabIndex = 0;
            this.baudrates.SelectedIndexChanged += new System.EventHandler(this.baudrates_SelectedIndexChanged);
            // 
            // ports
            // 
            this.ports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ports.FormattingEnabled = true;
            this.ports.Location = new System.Drawing.Point(12, 32);
            this.ports.Name = "ports";
            this.ports.Size = new System.Drawing.Size(121, 21);
            this.ports.TabIndex = 1;
            this.ports.SelectedIndexChanged += new System.EventHandler(this.ports_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port Number";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Baudrate";
            // 
            // connectBg
            // 
            this.connectBg.BackColor = System.Drawing.Color.Red;
            this.connectBg.Controls.Add(this.connectProgress);
            this.connectBg.Controls.Add(this.connectText);
            this.connectBg.Location = new System.Drawing.Point(12, 60);
            this.connectBg.Name = "connectBg";
            this.connectBg.Size = new System.Drawing.Size(248, 51);
            this.connectBg.TabIndex = 4;
            this.connectBg.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // connectText
            // 
            this.connectText.AutoSize = true;
            this.connectText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectText.ForeColor = System.Drawing.Color.Snow;
            this.connectText.Location = new System.Drawing.Point(64, 15);
            this.connectText.Name = "connectText";
            this.connectText.Size = new System.Drawing.Size(119, 20);
            this.connectText.TabIndex = 0;
            this.connectText.Text = "Disconnected";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(12, 117);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(121, 23);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(139, 117);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(121, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // connectProgress
            // 
            this.connectProgress.Location = new System.Drawing.Point(0, 0);
            this.connectProgress.Name = "connectProgress";
            this.connectProgress.Size = new System.Drawing.Size(248, 51);
            this.connectProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.connectProgress.TabIndex = 1;
            this.connectProgress.Value = 1;
            this.connectProgress.Visible = false;
            // 
            // FPSConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 150);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.connectBg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ports);
            this.Controls.Add(this.baudrates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FPSConnectForm";
            this.Text = "Connect To Fingerprint Scanner";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FPSConnectForm_Load);
            this.connectBg.ResumeLayout(false);
            this.connectBg.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox baudrates;
        private System.Windows.Forms.ComboBox ports;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel connectBg;
        private System.Windows.Forms.Label connectText;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ProgressBar connectProgress;
    }
}