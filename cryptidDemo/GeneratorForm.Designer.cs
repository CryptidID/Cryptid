namespace cryptidDemo {
    partial class GeneratorForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneratorForm));
            this.headshotBox = new System.Windows.Forms.PictureBox();
            this.headshotDialog = new System.Windows.Forms.OpenFileDialog();
            this.firstName = new System.Windows.Forms.TextBox();
            this.middleName = new System.Windows.Forms.TextBox();
            this.lastName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.zipcode = new System.Windows.Forms.TextBox();
            this.state = new System.Windows.Forms.TextBox();
            this.city = new System.Windows.Forms.TextBox();
            this.address = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.country = new System.Windows.Forms.TextBox();
            this.feet = new System.Windows.Forms.NumericUpDown();
            this.inches = new System.Windows.Forms.NumericUpDown();
            this.eye = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.sex = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rawOutput = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uidOutput = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.sigOutput = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.fpTemplateOutput = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.genCard = new System.Windows.Forms.Button();
            this.uploadBlockchain = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.fpBox = new System.Windows.Forms.PictureBox();
            this.dob = new System.Windows.Forms.DateTimePicker();
            this.issued = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.generateButton = new System.Windows.Forms.Button();
            this.fingerprintButton = new System.Windows.Forms.Button();
            this.headshotButton = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.cidSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadCryptidIdButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.feet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inches)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpBox)).BeginInit();
            this.SuspendLayout();
            // 
            // headshotBox
            // 
            this.headshotBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headshotBox.Location = new System.Drawing.Point(12, 12);
            this.headshotBox.Name = "headshotBox";
            this.headshotBox.Size = new System.Drawing.Size(160, 200);
            this.headshotBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.headshotBox.TabIndex = 0;
            this.headshotBox.TabStop = false;
            // 
            // headshotDialog
            // 
            this.headshotDialog.FileName = "openFileDialog1";
            // 
            // firstName
            // 
            this.firstName.Location = new System.Drawing.Point(186, 31);
            this.firstName.Name = "firstName";
            this.firstName.Size = new System.Drawing.Size(100, 20);
            this.firstName.TabIndex = 0;
            this.firstName.TextChanged += new System.EventHandler(this.firstName_TextChanged);
            // 
            // middleName
            // 
            this.middleName.Location = new System.Drawing.Point(293, 31);
            this.middleName.Name = "middleName";
            this.middleName.Size = new System.Drawing.Size(100, 20);
            this.middleName.TabIndex = 1;
            this.middleName.TextChanged += new System.EventHandler(this.middleName_TextChanged);
            // 
            // lastName
            // 
            this.lastName.Location = new System.Drawing.Point(399, 31);
            this.lastName.Name = "lastName";
            this.lastName.Size = new System.Drawing.Size(100, 20);
            this.lastName.TabIndex = 2;
            this.lastName.TextChanged += new System.EventHandler(this.lastName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(187, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "First Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Middle Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(400, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(400, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Zip Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(290, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "State";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(187, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "City";
            // 
            // zipcode
            // 
            this.zipcode.Location = new System.Drawing.Point(403, 112);
            this.zipcode.MaxLength = 9;
            this.zipcode.Name = "zipcode";
            this.zipcode.Size = new System.Drawing.Size(96, 20);
            this.zipcode.TabIndex = 7;
            this.zipcode.TextChanged += new System.EventHandler(this.zipcode_TextChanged);
            // 
            // state
            // 
            this.state.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.state.Location = new System.Drawing.Point(293, 112);
            this.state.MaxLength = 2;
            this.state.Name = "state";
            this.state.Size = new System.Drawing.Size(49, 20);
            this.state.TabIndex = 5;
            this.state.TextChanged += new System.EventHandler(this.state_TextChanged);
            // 
            // city
            // 
            this.city.Location = new System.Drawing.Point(186, 112);
            this.city.Name = "city";
            this.city.Size = new System.Drawing.Size(100, 20);
            this.city.TabIndex = 4;
            this.city.TextChanged += new System.EventHandler(this.city_TextChanged);
            // 
            // address
            // 
            this.address.Location = new System.Drawing.Point(186, 70);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(313, 20);
            this.address.TabIndex = 3;
            this.address.TextChanged += new System.EventHandler(this.address_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(187, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Address";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(345, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Country";
            // 
            // country
            // 
            this.country.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.country.Location = new System.Drawing.Point(348, 112);
            this.country.MaxLength = 3;
            this.country.Name = "country";
            this.country.Size = new System.Drawing.Size(49, 20);
            this.country.TabIndex = 6;
            this.country.TextChanged += new System.EventHandler(this.country_TextChanged);
            // 
            // feet
            // 
            this.feet.Location = new System.Drawing.Point(390, 152);
            this.feet.Name = "feet";
            this.feet.Size = new System.Drawing.Size(51, 20);
            this.feet.TabIndex = 10;
            this.feet.ValueChanged += new System.EventHandler(this.feet_ValueChanged);
            // 
            // inches
            // 
            this.inches.Location = new System.Drawing.Point(448, 151);
            this.inches.Name = "inches";
            this.inches.Size = new System.Drawing.Size(51, 20);
            this.inches.TabIndex = 11;
            this.inches.ValueChanged += new System.EventHandler(this.inches_ValueChanged);
            // 
            // eye
            // 
            this.eye.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eye.FormattingEnabled = true;
            this.eye.Location = new System.Drawing.Point(186, 151);
            this.eye.Name = "eye";
            this.eye.Size = new System.Drawing.Size(96, 21);
            this.eye.TabIndex = 8;
            this.eye.SelectedIndexChanged += new System.EventHandler(this.eye_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 135);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Eye Color";
            // 
            // sex
            // 
            this.sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sex.FormattingEnabled = true;
            this.sex.Location = new System.Drawing.Point(288, 151);
            this.sex.Name = "sex";
            this.sex.Size = new System.Drawing.Size(96, 21);
            this.sex.TabIndex = 9;
            this.sex.SelectedIndexChanged += new System.EventHandler(this.sex_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(290, 135);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Sex";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(390, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Feet";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(445, 135);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Inches";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage5);
            this.tabControl.Location = new System.Drawing.Point(516, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(533, 314);
            this.tabControl.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rawOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(525, 288);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Raw Output";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rawOutput
            // 
            this.rawOutput.Location = new System.Drawing.Point(6, 5);
            this.rawOutput.Multiline = true;
            this.rawOutput.Name = "rawOutput";
            this.rawOutput.ReadOnly = true;
            this.rawOutput.Size = new System.Drawing.Size(513, 280);
            this.rawOutput.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uidOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(525, 288);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "UID";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // uidOutput
            // 
            this.uidOutput.Location = new System.Drawing.Point(6, 5);
            this.uidOutput.Multiline = true;
            this.uidOutput.Name = "uidOutput";
            this.uidOutput.ReadOnly = true;
            this.uidOutput.Size = new System.Drawing.Size(513, 277);
            this.uidOutput.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.sigOutput);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(525, 288);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Signature";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // sigOutput
            // 
            this.sigOutput.Location = new System.Drawing.Point(6, 5);
            this.sigOutput.Multiline = true;
            this.sigOutput.Name = "sigOutput";
            this.sigOutput.ReadOnly = true;
            this.sigOutput.Size = new System.Drawing.Size(513, 280);
            this.sigOutput.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.fpTemplateOutput);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(525, 288);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Fingerprint Template";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // fpTemplateOutput
            // 
            this.fpTemplateOutput.Location = new System.Drawing.Point(6, 5);
            this.fpTemplateOutput.Multiline = true;
            this.fpTemplateOutput.Name = "fpTemplateOutput";
            this.fpTemplateOutput.ReadOnly = true;
            this.fpTemplateOutput.Size = new System.Drawing.Size(513, 280);
            this.fpTemplateOutput.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.loadCryptidIdButton);
            this.tabPage5.Controls.Add(this.genCard);
            this.tabPage5.Controls.Add(this.uploadBlockchain);
            this.tabPage5.Controls.Add(this.save);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(525, 288);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Misc";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // genCard
            // 
            this.genCard.Location = new System.Drawing.Point(4, 66);
            this.genCard.Name = "genCard";
            this.genCard.Size = new System.Drawing.Size(518, 23);
            this.genCard.TabIndex = 2;
            this.genCard.Text = "Generate Identification Card";
            this.genCard.UseMnemonic = false;
            this.genCard.UseVisualStyleBackColor = true;
            this.genCard.Click += new System.EventHandler(this.genCard_Click);
            // 
            // uploadBlockchain
            // 
            this.uploadBlockchain.Location = new System.Drawing.Point(4, 36);
            this.uploadBlockchain.Name = "uploadBlockchain";
            this.uploadBlockchain.Size = new System.Drawing.Size(518, 23);
            this.uploadBlockchain.TabIndex = 1;
            this.uploadBlockchain.Text = "Upload to Blockchain";
            this.uploadBlockchain.UseVisualStyleBackColor = true;
            this.uploadBlockchain.Click += new System.EventHandler(this.uploadBlockchain_Click);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(4, 4);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(255, 23);
            this.save.TabIndex = 0;
            this.save.Text = "Save Cryptid ID";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // fpBox
            // 
            this.fpBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fpBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fpBox.Location = new System.Drawing.Point(32, 218);
            this.fpBox.Name = "fpBox";
            this.fpBox.Size = new System.Drawing.Size(120, 108);
            this.fpBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fpBox.TabIndex = 26;
            this.fpBox.TabStop = false;
            // 
            // dob
            // 
            this.dob.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dob.Location = new System.Drawing.Point(186, 191);
            this.dob.Name = "dob";
            this.dob.Size = new System.Drawing.Size(156, 20);
            this.dob.TabIndex = 12;
            this.dob.ValueChanged += new System.EventHandler(this.dob_ValueChanged);
            // 
            // issued
            // 
            this.issued.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.issued.Location = new System.Drawing.Point(344, 191);
            this.issued.Name = "issued";
            this.issued.Size = new System.Drawing.Size(155, 20);
            this.issued.TabIndex = 13;
            this.issued.ValueChanged += new System.EventHandler(this.issued_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(186, 175);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "Date Of Birth";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(345, 175);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 13);
            this.label14.TabIndex = 31;
            this.label14.Text = "Issue Date";
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(186, 303);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(313, 23);
            this.generateButton.TabIndex = 17;
            this.generateButton.Text = "Generate!";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // fingerprintButton
            // 
            this.fingerprintButton.Location = new System.Drawing.Point(344, 274);
            this.fingerprintButton.Name = "fingerprintButton";
            this.fingerprintButton.Size = new System.Drawing.Size(155, 23);
            this.fingerprintButton.TabIndex = 16;
            this.fingerprintButton.Text = "Scan Fingerprint...";
            this.fingerprintButton.UseVisualStyleBackColor = true;
            this.fingerprintButton.Click += new System.EventHandler(this.fingerprintButton_Click);
            // 
            // headshotButton
            // 
            this.headshotButton.Location = new System.Drawing.Point(186, 274);
            this.headshotButton.Name = "headshotButton";
            this.headshotButton.Size = new System.Drawing.Size(148, 23);
            this.headshotButton.TabIndex = 15;
            this.headshotButton.Text = "Choose Headshot...";
            this.headshotButton.UseVisualStyleBackColor = true;
            this.headshotButton.Click += new System.EventHandler(this.headshotButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(187, 218);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 33;
            this.label15.Text = "Password";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(186, 234);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(313, 20);
            this.password.TabIndex = 14;
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.password_TextChanged);
            // 
            // loadCryptidIdButton
            // 
            this.loadCryptidIdButton.Location = new System.Drawing.Point(265, 4);
            this.loadCryptidIdButton.Name = "loadCryptidIdButton";
            this.loadCryptidIdButton.Size = new System.Drawing.Size(257, 23);
            this.loadCryptidIdButton.TabIndex = 3;
            this.loadCryptidIdButton.Text = "Load Cryptid ID";
            this.loadCryptidIdButton.UseVisualStyleBackColor = true;
            this.loadCryptidIdButton.Click += new System.EventHandler(this.loadCryptidIdButton_Click);
            // 
            // GeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 332);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.password);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.issued);
            this.Controls.Add(this.dob);
            this.Controls.Add(this.fingerprintButton);
            this.Controls.Add(this.fpBox);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.sex);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.eye);
            this.Controls.Add(this.inches);
            this.Controls.Add(this.feet);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.country);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.address);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.zipcode);
            this.Controls.Add(this.state);
            this.Controls.Add(this.city);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lastName);
            this.Controls.Add(this.middleName);
            this.Controls.Add(this.firstName);
            this.Controls.Add(this.headshotButton);
            this.Controls.Add(this.headshotBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeneratorForm";
            this.Text = "Cryptid ID Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.headshotBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.feet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inches)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox headshotBox;
        private System.Windows.Forms.OpenFileDialog headshotDialog;
        private System.Windows.Forms.TextBox firstName;
        private System.Windows.Forms.TextBox middleName;
        private System.Windows.Forms.TextBox lastName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox zipcode;
        private System.Windows.Forms.TextBox state;
        private System.Windows.Forms.TextBox city;
        private System.Windows.Forms.TextBox address;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox country;
        private System.Windows.Forms.NumericUpDown feet;
        private System.Windows.Forms.NumericUpDown inches;
        private System.Windows.Forms.ComboBox eye;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox sex;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox rawOutput;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox uidOutput;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox sigOutput;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox fpTemplateOutput;
        private System.Windows.Forms.PictureBox fpBox;
        private System.Windows.Forms.DateTimePicker dob;
        private System.Windows.Forms.DateTimePicker issued;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Button fingerprintButton;
        private System.Windows.Forms.Button headshotButton;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.SaveFileDialog cidSaveDialog;
        private System.Windows.Forms.Button genCard;
        private System.Windows.Forms.Button uploadBlockchain;
        private System.Windows.Forms.Button loadCryptidIdButton;
    }
}

