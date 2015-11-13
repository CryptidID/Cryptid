using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using Cryptid;
using SourceAFIS.Simple;

namespace cryptidDemo {
    public partial class AuthForm : Form {
        public AuthForm() {
            InitializeComponent();
        }

        private readonly RSAParameters PublicKey = Cryptid.Utils.Keys.PublicKey("public.xml");

        public byte[] PackedData { get; set; }
        public string Password { get; set; }

        private void button1_Click(object sender, EventArgs e) {
            openCidDialog.DefaultExt = "*.cid";
            openCidDialog.Filter = "Cryptid ID File (*.cid)|*.cid";
            if (openCidDialog.ShowDialog() == DialogResult.OK) {
                PackedData = File.ReadAllBytes(openCidDialog.FileName);
                cidLocation.Text = openCidDialog.FileName;
            }
        }

        private void password_TextChanged(object sender, EventArgs e) {
            Password = password.Text;
        }

        private void showInfoButton_Click(object sender, EventArgs e) {
            CandidateInfoForm info = new CandidateInfoForm();
            info.Show();
            info.LoadCandidateInfo(PackedData, Password);
        }

        private void button3_Click(object sender, EventArgs e) {
            FPSConnectForm connectDialog = new FPSConnectForm();;

            Enabled = false;
            DialogResult connectDr = connectDialog.ShowDialog(this);
            if (connectDr != DialogResult.OK) button3.Enabled = false;
            connectDialog.Close();
            Enabled = true;

            Fingerprint f = new Fingerprint();

            if (connectDialog.IsConnected) {
                Enabled = false;

                ScanFingerForm scanForm = new ScanFingerForm();
                DialogResult dr = scanForm.ShowDialog(this);
                if (dr == DialogResult.OK) {
                    f.AsBitmap = scanForm.Fingerprint;
                }
                scanForm.Dispose();

                Enabled = true;
            } else {
                //TODO: Allow to choose fingerprint image?
                MessageBox.Show("You are not connected to a fingerprint scanner!");
            }

            float authLikelyhood = CandidateDelegate.VerifyFingerprint(CandidateDelegate.Unpack(PackedData, Password, PublicKey), f);
            MessageBox.Show("Auth likelyhood: " + authLikelyhood.ToString("R"));
        }

        private void AuthForm_Load(object sender, EventArgs e) {

        }
    }
}
