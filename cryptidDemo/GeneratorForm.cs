#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid.Scanners;
using Cryptid;
using Cryptid.Utils;
using CryptidDemo.Properties;
using SourceAFIS.Simple;
using Keys = Cryptid.Utils.Keys;

#endregion

namespace CryptidDemo {
    public partial class GeneratorForm : Form {
        private readonly FpsConnectForm _connectDialog = new FpsConnectForm();
        private readonly RSAParameters _privateKey = Keys.PrivateKey("private.xml");

        private Candidate _c = new Candidate();
        private byte[] _output;

        public GeneratorForm() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Enabled = false;

            var dr = _connectDialog.ShowDialog(this);
            if (dr != DialogResult.OK) fingerprintButton.Enabled = false;
            _connectDialog.Close();
            Enabled = true;

            eye.DataSource = Enum.GetValues(typeof (Candidate.EyeColor)).Cast<Candidate.EyeColor>();
            sex.DataSource = Enum.GetValues(typeof (Candidate.Sex)).Cast<Candidate.Sex>();

            _c.Dbd = _c.Dbb = dob.Value = issued.Value = DateTime.Today;
            _c.Dbc = (Candidate.Sex) Enum.Parse(typeof (Candidate.Sex), sex.SelectedText);
            _c.Day = (Candidate.EyeColor) Enum.Parse(typeof (Candidate.EyeColor), eye.SelectedText);

            _c.Dau = new Candidate.Height(0, 0);
        }

        private void headshotButton_Click(object sender, EventArgs e) {
            headshotDialog.DefaultExt = "*.jpg";
            headshotDialog.Filter = Resources.JPG_FILTER;
            headshotDialog.FileName = "";
            if (headshotDialog.ShowDialog() == DialogResult.OK) {
                headshotBox.ImageLocation = headshotDialog.FileName;
                _c.Image = Image.FromFile(headshotBox.ImageLocation);
            }
        }

        private void firstName_TextChanged(object sender, EventArgs e) {
            _c.Dac = firstName.Text;
        }

        private void middleName_TextChanged(object sender, EventArgs e) {
            _c.Dad = middleName.Text;
        }

        private void lastName_TextChanged(object sender, EventArgs e) {
            _c.Dcs = lastName.Text;
        }

        private void issued_ValueChanged(object sender, EventArgs e) {
            _c.Dbd = issued.Value;
        }

        private void address_TextChanged(object sender, EventArgs e) {
            _c.Dag = address.Text;
        }

        private void city_TextChanged(object sender, EventArgs e) {
            _c.Dai = city.Text;
        }

        private void state_TextChanged(object sender, EventArgs e) {
            _c.Daj = state.Text.ToUpper();
        }

        private void country_TextChanged(object sender, EventArgs e) {
            _c.Dcg = country.Text.ToUpper();
        }

        private void zipcode_TextChanged(object sender, EventArgs e) {
            _c.Dak = new Candidate.PostalCode(zipcode.Text);
        }

        private void eye_SelectedIndexChanged(object sender, EventArgs e) {
            _c.Day = eye.SelectedItem is Candidate.EyeColor
                ? (Candidate.EyeColor) eye.SelectedItem
                : Candidate.EyeColor.Unknown;
        }

        private void sex_SelectedIndexChanged(object sender, EventArgs e) {
            _c.Dbc = eye.SelectedItem is Candidate.Sex ? (Candidate.Sex) sex.SelectedItem : Candidate.Sex.Male;
        }

        private void feet_ValueChanged(object sender, EventArgs e) {
            _c.Dau = new Candidate.Height((int) feet.Value, (int) inches.Value);
        }

        private void inches_ValueChanged(object sender, EventArgs e) {
            _c.Dau = new Candidate.Height((int) feet.Value, (int) inches.Value);
        }

        private void dob_ValueChanged(object sender, EventArgs e) {
            _c.Dbb = dob.Value;
        }

        private void generateButton_Click(object sender, EventArgs e) {
            if (!_c.IsComplete()) {
                MessageBox.Show(Resources.NOT_GENERATE_INCOMPLETE_ID_ERROR, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password.Text)) {
                MessageBox.Show(Resources.PASSWORD_EMPTY_ERROR, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var data = CandidateDelegate.Pack(_c, password.Text, _privateKey);
            var unpacked = CandidateDelegate.Unpack(data, password.Text, _privateKey);

            rawOutput.Text = BitConverter.ToString(data);
            uidOutput.Text = BitConverter.ToString(unpacked.Uid);
            sigOutput.Text = BitConverter.ToString(Arrays.CopyOfRange(data, data.Length - 512, data.Length));
            fpTemplateOutput.Text = BitConverter.ToString(unpacked.Fingerprint.AsIsoTemplate);

            _output = data;
            _c.Uid = unpacked.Uid;
        }

        private void fingerprintButton_Click(object sender, EventArgs e) {
            _c.Fingerprint = new Fingerprint();

            if (_connectDialog.IsConnected) {
                Enabled = false;

                var scanForm = new ScanFingerForm();
                var dr = scanForm.ShowDialog(this);
                if (dr == DialogResult.OK) {
                    _c.Fingerprint.AsBitmap = scanForm.Fingerprint;
                    fpBox.Image = scanForm.Fingerprint;
                }
                scanForm.Dispose();

                Enabled = true;
            }
            else {
                MessageBox.Show("You are not connected to a fingerprint scanner!", Resources.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void password_TextChanged(object sender, EventArgs e) {
        }

#pragma warning disable 108,114
        public virtual void Dispose() {
#pragma warning restore 108,114
            FPS_GT511C3.Close();
            Dispose(true);
        }

        private void save_Click(object sender, EventArgs e) {
            if (_output == null) {
                MessageBox.Show(Resources.CANNOT_SAVE_IF_NOT_GENERATED_ERROR, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            cidSaveDialog.FileName = _c.Dcs + "-" + _c.Dad + "-" + _c.Dac + ".cid";
            cidSaveDialog.Filter = Resources.CRYPTID_ID_FILTER;

            if (cidSaveDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(cidSaveDialog.FileName, _output);
            }
        }

        private void uploadBlockchain_Click(object sender, EventArgs e) {
            if (!_c.IsComplete()) {
                MessageBox.Show(Resources.NOT_GENERATE_INCOMPLETE_ID_ERROR, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password.Text)) {
                MessageBox.Show(Resources.PASSWORD_EMPTY_ERROR, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            chainId.Text = Convert.ToBase64String(CandidateDelegate.EnrollCandidate(_c, password.Text, _privateKey));
        }

        private void genCard_Click(object sender, EventArgs e) {
            if (_c == null || !_c.IsComplete()) {
                MessageBox.Show(Resources.CANNOT_SAVE_IF_NOT_GENERATED_OR_INCOMPLETE,
                    Resources.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cidSaveDialog.FileName = _c.Dcs + "-" + _c.Dad + "-" + _c.Dac + ".pdf";
            cidSaveDialog.Filter = Resources.PDF_FILTER;

            if (cidSaveDialog.ShowDialog() == DialogResult.OK) {
                var cg = new CardGenerator(_c, "cryptid-id-template.pdf");
                cg.Generate(cidSaveDialog.FileName, Convert.FromBase64String(chainId.Text));
            }
        }

        private void loadCryptidIdButton_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(password.Text)) {
                MessageBox.Show(Resources.MUST_ENTER_PASSWORD_MESSAGE, Resources.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            headshotDialog.DefaultExt = "*.cid";
            headshotDialog.Filter = Resources.CRYPTID_ID_FILTER;
            headshotDialog.FileName = "";
            if (headshotDialog.ShowDialog() == DialogResult.OK) {
                var packed = File.ReadAllBytes(headshotDialog.FileName);
                _output = packed;
                var c = CandidateDelegate.Unpack(packed, password.Text, _privateKey);

                lastName.Text = c.Dcs;
                firstName.Text = c.Dac;
                middleName.Text = c.Dad;
                issued.Value = c.Dbd;
                dob.Value = c.Dbb;
                sex.SelectedIndex = (int) c.Dbc - 1;
                eye.SelectedIndex = (int) c.Day;
                inches.Value = c.Dau.Inches;
                feet.Value = c.Dau.Feet;
                address.Text = c.Dag;
                city.Text = c.Dai;
                state.Text = c.Daj;
                zipcode.Text = c.Dak.AnsiFormat;
                country.Text = c.Dcg;
                headshotBox.Image = c.Image;

                _c = c;
            }
        }
    }
}