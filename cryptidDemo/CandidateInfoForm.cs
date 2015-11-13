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

namespace cryptidDemo {
    public partial class CandidateInfoForm : Form {

        private readonly RSAParameters PublicKey = Cryptid.Utils.Keys.PublicKey("public.xml");

        public byte[] PackedData { get; set; }
        public string Password { get; set; }
        private Candidate _c;

        public CandidateInfoForm() {
            InitializeComponent();
        }

        private void CandidateInfoForm_Load(object sender, EventArgs e) {
            try {
                _c = CandidateDelegate.Unpack(PackedData, Password, PublicKey);
            } catch (Exception ex) {
                MessageBox.Show("Couldn't verify provided data.");
                Close();
            }

            candidateDump.Text = DumpCandidateJson();
            headshotBox.Image = _c.Image;
        }

        private string DumpCandidateJson() {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(_c.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, _c);
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}
