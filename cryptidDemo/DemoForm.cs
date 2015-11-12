using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cryptid;
using SourceAFIS;
using SourceAFIS.Simple;

namespace cryptidDemo {
    //TODO: Auto generate template images/save
    //TODO: Save ID data
    //TODO: FP scanning 
    public partial class DemoForm : Form {
        private RSAParameters PrivateKey {
            get {
                var k = new RSACryptoServiceProvider();
                k.FromXmlString(File.ReadAllText("private.xml"));
                return k.ExportParameters(true);
            }
        }

        private Candidate _c = new Candidate();

        public DemoForm() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
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
        }

        private void fingerprintButton_Click(object sender, EventArgs e) {
            //TODO: make this automatically get your fingerprint
            _c.Fingerprint = new Fingerprint();
            _c.Fingerprint.AsBitmap = new Bitmap("fingerprint-test.bmp");
        }

        private void password_TextChanged(object sender, EventArgs e) {

        }
    }
}
