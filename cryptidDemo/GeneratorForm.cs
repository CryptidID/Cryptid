using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using cryptid.Scanners;
using Cryptid;
using SourceAFIS;
using SourceAFIS.Simple;

namespace cryptidDemo {
    //TODO: Auto generate template images/save
    public partial class GeneratorForm : Form {

        private readonly RSAParameters PrivateKey = Cryptid.Utils.Keys.PrivateKey("private.xml");

        private Candidate _c = new Candidate();
        private FPSConnectForm connectDialog = new FPSConnectForm();
        private byte[] output = null;

        public GeneratorForm() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Enabled = false;

            DialogResult dr = connectDialog.ShowDialog(this);
            if (dr != DialogResult.OK) fingerprintButton.Enabled = false;
            connectDialog.Close();
            Enabled = true;

            eye.DataSource = Enum.GetValues(typeof (Candidate.EyeColor)).Cast<Candidate.EyeColor>();
            sex.DataSource = Enum.GetValues(typeof(Candidate.Sex)).Cast<Candidate.Sex>();

            _c.Dbd = _c.Dbb = dob.Value = issued.Value = DateTime.Today;
            _c.Dbc = (Candidate.Sex) Enum.Parse(typeof(Candidate.Sex), sex.SelectedText);
            _c.Day = (Candidate.EyeColor)Enum.Parse(typeof(Candidate.EyeColor), eye.SelectedText);

            _c.Dau = new Candidate.Height(0, 0);
        }

        private void headshotButton_Click(object sender, EventArgs e) {
            headshotDialog.DefaultExt = "*.jpg";
            headshotDialog.DefaultExt = "Image Files|*.jpg";
            if (headshotDialog.ShowDialog() == DialogResult.OK) {
                headshotBox.ImageLocation = headshotDialog.FileName;
            }
            _c.Image = Image.FromFile(headshotBox.ImageLocation);
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
            _c.Day = eye.SelectedItem is Candidate.EyeColor ? (Candidate.EyeColor) eye.SelectedItem : Candidate.EyeColor.Unknown;
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
            _c.Dbb = issued.Value;
        }

        private void generateButton_Click(object sender, EventArgs e) {
            byte[] data = CandidateDelegate.Pack(_c, password.Text, PrivateKey);
            Candidate unpacked = CandidateDelegate.Unpack(data, password.Text, PrivateKey);

            rawOutput.Text = BitConverter.ToString(data);
            uidOutput.Text = BitConverter.ToString(unpacked.Uid);
            sigOutput.Text = BitConverter.ToString(Cryptid.Utils.Arrays.CopyOfRange(data, data.Length - 512, data.Length));
            fpTemplateOutput.Text = BitConverter.ToString(unpacked.Fingerprint.AsIsoTemplate);

            output = data;
        }

        private void fingerprintButton_Click(object sender, EventArgs e) {
            _c.Fingerprint = new Fingerprint();
            
            if (connectDialog.IsConnected) {
                Enabled = false;

                ScanFingerForm scanForm = new ScanFingerForm();
                DialogResult dr = scanForm.ShowDialog(this);
                if (dr == DialogResult.OK) {
                    _c.Fingerprint.AsBitmap = scanForm.Fingerprint;
                    fpBox.Image = scanForm.Fingerprint;
                }
                scanForm.Dispose();

                Enabled = true;
            } else {
                //TODO: Allow to choose fingerprint image?
                MessageBox.Show("You are not connected to a fingerprint scanner!");
            }
        }

        private void password_TextChanged(object sender, EventArgs e) {

        }

        public virtual void Dispose() {
            FPS_GT511C3.Close();
            Dispose(true);
        }

        private void save_Click(object sender, EventArgs e) {
            if (output == null) return;

            cidSaveDialog.FileName = _c.Dcs + "-" + _c.Dad + "-" + _c.Dac + ".cid";
            cidSaveDialog.Filter = "Cryptid ID File (*.cid)|*.cid";

            if (cidSaveDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(cidSaveDialog.FileName, output);
            }
        }
    }
}
