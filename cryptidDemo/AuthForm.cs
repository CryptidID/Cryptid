#region

using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid;
using Cryptid.Utils;
using CryptidDemo.Properties;
using SourceAFIS.Simple;
using Keys = Cryptid.Utils.Keys;

#endregion

namespace CryptidDemo {
    public partial class AuthForm : Form {
        private readonly RSAParameters _publicKey = Keys.PublicKey("public.xml");

        public AuthForm() {
            InitializeComponent();
        }

        public byte[] PackedData { get; set; }
        public string Password { get; set; }

        private void button1_Click(object sender, EventArgs e) {
            openCidDialog.DefaultExt = "*.cid";
            openCidDialog.Filter = Resources.CRYPTID_ID_FILTER;
            if (openCidDialog.ShowDialog() == DialogResult.OK) {
                PackedData = File.ReadAllBytes(openCidDialog.FileName);
                cidLocation.Text = openCidDialog.FileName;
            }
        }

        private void password_TextChanged(object sender, EventArgs e) {
            Password = password.Text;
        }

        private void showInfoButton_Click(object sender, EventArgs e) {
            if (PackedData == null && string.IsNullOrWhiteSpace(chainID.Text)) {
                MessageBox.Show(Resources.NEED_CID_OR_CHAIN_ID_ERROR, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(chainID.Text)) {
                var info = new CandidateInfoForm();
                info.LoadCandidateInfo(PackedData, Password);
                info.Show();
            }
            else {
                var info = new CandidateInfoForm();
                byte[] packed = CandidateDelegate.GetPackedCandidate(Convert.FromBase64String(chainID.Text));
                info.LoadCandidateInfo(packed, Password, Convert.FromBase64String(chainID.Text));
                info.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            var connectDialog = new FpsConnectForm();

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
                MessageBox.Show(Resources.FPS_NOT_CONNECTED_ERROR);
            }

            float authLikelyhood;
            try {
                authLikelyhood =
                    CandidateDelegate.VerifyFingerprint(CandidateDelegate.Unpack(PackedData, Password, _publicKey), f);
            }
                // ReSharper disable once UnusedVariable
            catch (CryptographicException ex) {
                MessageBox.Show(Resources.NOT_VERIFY_ID_ERROR);
                return;
            }
            MessageBox.Show(Resources.AUTH_LIKLEYHOOD_MESSAGE + authLikelyhood.ToString("R"));
        }

        private void AuthForm_Load(object sender, EventArgs e) {
        }
    }
}