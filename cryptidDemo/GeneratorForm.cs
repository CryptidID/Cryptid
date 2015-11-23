using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid;
using Cryptid.Scanners;
using Cryptid.Utils;
using CryptidDemo;
using SourceAFIS.Simple;
using Keys = Cryptid.Utils.Keys;

namespace cryptidDemo {
    //TODO: Auto generate template images/save
    public partial class GeneratorForm : Form {
        private readonly RSAParameters PrivateKey = Keys.PrivateKey("private.xml");

        private Candidate _c = new Candidate();
        private readonly FpsConnectForm connectDialog = new FpsConnectForm();
        private byte[] output;

        public GeneratorForm() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Enabled = false;

            var dr = connectDialog.ShowDialog(this);
            if (dr != DialogResult.OK) fingerprintButton.Enabled = false;
            connectDialog.Close();
            Enabled = true;

            eye.DataSource = Enum.GetValues(typeof(Candidate.EyeColor)).Cast<Candidate.EyeColor>();
            sex.DataSource = Enum.GetValues(typeof(Candidate.Sex)).Cast<Candidate.Sex>();

            _c.Dbd = _c.Dbb = dob.Value = issued.Value = DateTime.Today;
            _c.Dbc = (Candidate.Sex)Enum.Parse(typeof(Candidate.Sex), sex.SelectedText);
            _c.Day = (Candidate.EyeColor)Enum.Parse(typeof(Candidate.EyeColor), eye.SelectedText);

            _c.Dau = new Candidate.Height(0, 0);
        }

        private void headshotButton_Click(object sender, EventArgs e) {
            headshotDialog.DefaultExt = "*.jpg";
            headshotDialog.Filter = "Image Files|*.jpg";
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
                ? (Candidate.EyeColor)eye.SelectedItem
                : Candidate.EyeColor.Unknown;
        }

        private void sex_SelectedIndexChanged(object sender, EventArgs e) {
            _c.Dbc = eye.SelectedItem is Candidate.Sex ? (Candidate.Sex)sex.SelectedItem : Candidate.Sex.Male;
        }

        private void feet_ValueChanged(object sender, EventArgs e) {
            _c.Dau = new Candidate.Height((int)feet.Value, (int)inches.Value);
        }

        private void inches_ValueChanged(object sender, EventArgs e) {
            _c.Dau = new Candidate.Height((int)feet.Value, (int)inches.Value);
        }

        private void dob_ValueChanged(object sender, EventArgs e) {
            _c.Dbb = dob.Value;
        }

        private void generateButton_Click(object sender, EventArgs e) {
            if (!_c.IsComplete()) {
                MessageBox.Show("Cannot generate an and ID for an incomplete candidate.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password.Text)) {
                MessageBox.Show("Password cannot be empty!.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = CandidateDelegate.Pack(_c, password.Text, PrivateKey);
            var unpacked = CandidateDelegate.Unpack(data, password.Text, PrivateKey);

            rawOutput.Text = BitConverter.ToString(data);
            uidOutput.Text = BitConverter.ToString(unpacked.Uid);
            sigOutput.Text = BitConverter.ToString(Arrays.CopyOfRange(data, data.Length - 512, data.Length));
            fpTemplateOutput.Text = BitConverter.ToString(unpacked.Fingerprint.AsIsoTemplate);

            output = data;
            _c.Uid = unpacked.Uid;
        }

        private void fingerprintButton_Click(object sender, EventArgs e) {
            _c.Fingerprint = new Fingerprint();

            if (connectDialog.IsConnected) {
                Enabled = false;

                var scanForm = new ScanFingerForm();
                var dr = scanForm.ShowDialog(this);
                if (dr == DialogResult.OK) {
                    _c.Fingerprint.AsBitmap = scanForm.Fingerprint;
                    fpBox.Image = scanForm.Fingerprint;
                }
                scanForm.Dispose();

                Enabled = true;
            } else {
                //TODO: Allow to choose fingerprint image?
                MessageBox.Show("You are not connected to a fingerprint scanner!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void password_TextChanged(object sender, EventArgs e) {
        }

        public virtual void Dispose() {
            FPS_GT511C3.Close();
            Dispose(true);
        }

        private void save_Click(object sender, EventArgs e) {
            if (output == null) {
                MessageBox.Show("Cannot save ID if none has been generated!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            cidSaveDialog.FileName = _c.Dcs + "-" + _c.Dad + "-" + _c.Dac + ".cid";
            cidSaveDialog.Filter = "Cryptid ID File (*.cid)|*.cid";

            if (cidSaveDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(cidSaveDialog.FileName, output);
            }
        }

        private void uploadBlockchain_Click(object sender, EventArgs e) {
            if (!_c.IsComplete()) {
                MessageBox.Show("Cannot generate an and ID for an incomplete candidate.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password.Text)) {
                MessageBox.Show("Password cannot be empty!.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            chainId.Text = Convert.ToBase64String(CandidateDelegate.EnrollCandidate(_c, password.Text, PrivateKey));
        }

        private void genCard_Click(object sender, EventArgs e) {
            if (_c == null || !_c.IsComplete()) {
                MessageBox.Show("Cannot generate card if no ID has been generated or the candidate is incomplete!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cidSaveDialog.FileName = _c.Dcs + "-" + _c.Dad + "-" + _c.Dac + ".pdf";
            cidSaveDialog.Filter = "PDF File (*.pdf)|*.pdf";

            if (cidSaveDialog.ShowDialog() == DialogResult.OK) {
                var cg = new CardGenerator(_c, cidSaveDialog.FileName);
                cg.Generate(cidSaveDialog.FileName, Convert.FromBase64String(chainId.Text));
            }
        }

        private void loadCryptidIdButton_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(password.Text)) {
                MessageBox.Show("You must enter the password associated with this ID.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            headshotDialog.DefaultExt = "*.cid";
            headshotDialog.Filter = "Cryptid ID File (*.cid)|*.cid";
            headshotDialog.FileName = "";
            if (headshotDialog.ShowDialog() == DialogResult.OK) {
                var packed = File.ReadAllBytes(headshotDialog.FileName);
                output = packed;
                var c = CandidateDelegate.Unpack(packed, password.Text, PrivateKey);

                lastName.Text = c.Dcs;
                firstName.Text = c.Dac;
                middleName.Text = c.Dad;
                issued.Value = c.Dbd;
                dob.Value = c.Dbb;
                sex.SelectedIndex = (int)c.Dbc - 1;
                eye.SelectedIndex = (int)c.Day;
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

        private void loadBlockChain_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(password.Text)) {
                MessageBox.Show("You must enter the password associated with this ID.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var packed = CandidateDelegate.GetPackedCandidate(Convert.FromBase64String(chainId.Text));
            output = packed;
            var c = CandidateDelegate.Unpack(packed, password.Text, PrivateKey);

            lastName.Text = c.Dcs;
            firstName.Text = c.Dac;
            middleName.Text = c.Dad;
            issued.Value = c.Dbd;
            dob.Value = c.Dbb;
            sex.SelectedIndex = (int)c.Dbc - 1;
            eye.SelectedIndex = (int)c.Day;
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