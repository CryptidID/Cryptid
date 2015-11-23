namespace CryptidDemo {
    partial class AuthForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthForm));
            this.openCidDialog = new System.Windows.Forms.OpenFileDialog();
            this.cidLocation = new System.Windows.Forms.TextBox();
            this.chooseCid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.showInfoButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chainID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // openCidDialog
            // 
            this.openCidDialog.FileName = "openFileDialog1";
            // 
            // cidLocation
            // 
            this.cidLocation.Location = new System.Drawing.Point(13, 13);
            this.cidLocation.Name = "cidLocation";
            this.cidLocation.ReadOnly = true;
            this.cidLocation.Size = new System.Drawing.Size(191, 20);
            this.cidLocation.TabIndex = 0;
            // 
            // chooseCid
            // 
            this.chooseCid.Location = new System.Drawing.Point(210, 11);
            this.chooseCid.Name = "chooseCid";
            this.chooseCid.Size = new System.Drawing.Size(87, 23);
            this.chooseCid.TabIndex = 1;
            this.chooseCid.Text = "Choose CID...";
            this.chooseCid.UseVisualStyleBackColor = true;
            this.chooseCid.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Password:";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(74, 73);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(223, 20);
            this.password.TabIndex = 3;
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.password_TextChanged);
            // 
            // showInfoButton
            // 
            this.showInfoButton.Location = new System.Drawing.Point(141, 99);
            this.showInfoButton.Name = "showInfoButton";
            this.showInfoButton.Size = new System.Drawing.Size(75, 23);
            this.showInfoButton.TabIndex = 4;
            this.showInfoButton.Text = "Show Info";
            this.showInfoButton.UseVisualStyleBackColor = true;
            this.showInfoButton.Click += new System.EventHandler(this.showInfoButton_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(222, 99);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Full Auth";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "OR  ID string:";
            // 
            // chainID
            // 
            this.chainID.Location = new System.Drawing.Point(89, 45);
            this.chainID.Name = "chainID";
            this.chainID.Size = new System.Drawing.Size(208, 20);
            this.chainID.TabIndex = 7;
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 127);
            this.Controls.Add(this.chainID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.showInfoButton);
            this.Controls.Add(this.password);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chooseCid);
            this.Controls.Add(this.cidLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AuthForm";
            this.Text = "Cryptid Authenticator";
            this.Load += new System.EventHandler(this.AuthForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openCidDialog;
        private System.Windows.Forms.TextBox cidLocation;
        private System.Windows.Forms.Button chooseCid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Button showInfoButton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox chainID;
    }
}