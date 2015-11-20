using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid;
using SourceAFIS.Simple;
using Keys = Cryptid.Utils.Keys;

namespace cryptidDemo {
    public partial class AuthForm : Form {
        private readonly RSAParameters PublicKey = Keys.PublicKey("public.xml");

        public AuthForm() {
            InitializeComponent();
        }

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
            var info = new CandidateInfoForm();
            info.Show();
            info.LoadCandidateInfo(PackedData, Password);
        }

        private void button3_Click(object sender, EventArgs e) {
            var connectDialog = new FPSConnectForm();
            ;

            Enabled = false;
            var connectDr = connectDialog.ShowDialog(this);
            if (connectDr != DialogResult.OK) button3.Enabled = false;
            connectDialog.Close();
            Enabled = true;

            var f = new Fingerprint();

            if (connectDialog.IsConnected) {
                Enabled = false;

                var scanForm = new ScanFingerForm();
                var dr = scanForm.ShowDialog(this);
                if (dr == DialogResult.OK) {
                    f.AsBitmap = scanForm.Fingerprint;
                }
                scanForm.Dispose();

                Enabled = true;
            }
            else {
                //TODO: Allow to choose fingerprint image?
                MessageBox.Show("You are not connected to a fingerprint scanner!");
            }

            float authLikelyhood;
            try {
                authLikelyhood =
                    CandidateDelegate.VerifyFingerprint(CandidateDelegate.Unpack(PackedData, Password, PublicKey), f);
            }
            catch (CryptographicException ex) {
                MessageBox.Show("Couldn't verify provided data.");
                return;
            }
            MessageBox.Show("Auth likelyhood: " + authLikelyhood.ToString("R"));
        }

        private void AuthForm_Load(object sender, EventArgs e) {
        }
    }
}