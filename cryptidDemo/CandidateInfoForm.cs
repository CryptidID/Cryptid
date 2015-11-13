using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cryptid;
using Newtonsoft.Json;

namespace cryptidDemo {
    public partial class CandidateInfoForm : Form {

        private readonly RSAParameters PublicKey = Cryptid.Utils.Keys.PublicKey("public.xml");

        private Candidate _c;

        public CandidateInfoForm() {
            InitializeComponent();
        }

        private void CandidateInfoForm_Load(object sender, EventArgs e) {

        }

        public void LoadCandidateInfo(byte[] packed, string pasword) {
            try {
                _c = CandidateDelegate.Unpack(packed, pasword, PublicKey);
            } catch (Exception ex) {
                MessageBox.Show("Couldn't verify provided data.");
                Close();
                return;
            }

            headshotBox.Image = _c.Image;
            candidateDump.Text = JsonConvert.SerializeObject(_c, Formatting.Indented);
        }
    }
}
