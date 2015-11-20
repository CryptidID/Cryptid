#region

using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid;
using CryptidDemo.Properties;
using Newtonsoft.Json;
using Keys = Cryptid.Utils.Keys;

#endregion

namespace cryptidDemo {
    public partial class CandidateInfoForm : Form {
        private readonly RSAParameters _publicKey = Keys.PublicKey("public.xml");

        private Candidate _c;

        public CandidateInfoForm() {
            InitializeComponent();
        }

        private void CandidateInfoForm_Load(object sender, EventArgs e) {
        }

        public void LoadCandidateInfo(byte[] packed, string pasword) {
            try {
                _c = CandidateDelegate.Unpack(packed, pasword, _publicKey);
            }
                // ReSharper disable once UnusedVariable
            catch (Exception ex) {
                MessageBox.Show(Resources.NOT_VERIFY_ID_ERROR);
                Close();
                return;
            }

            headshotBox.Image = _c.Image;
            candidateDump.Text = JsonConvert.SerializeObject(_c, Formatting.Indented);
        }
    }
}