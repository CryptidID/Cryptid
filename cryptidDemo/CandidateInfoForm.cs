#region

using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid;
using CryptidDemo.Properties;
using Newtonsoft.Json;
using Keys = Cryptid.Utils.Keys;

#endregion

namespace CryptidDemo {
    public partial class CandidateInfoForm : Form {
        private readonly RSAParameters _publicKey = Keys.PublicKey("public.xml");

        private Candidate _c;

        public CandidateInfoForm() {
            InitializeComponent();
        }

        private void CandidateInfoForm_Load(object sender, EventArgs e) {
        }

        public void LoadCandidateInfo(byte[] packed, string pasword, byte[] chainId = null) {
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

            if (chainId != null) {
                chainHistory.View = View.List;
                foreach (var cid in CandidateDelegate.GetCandidateChainHistory(chainId, _publicKey)) {
                    chainHistory.Items.Add(new ListViewItem(Convert.ToBase64String(cid)));
                }
            }
        }

        private void chainHistory_DoubleClick(object sender, EventArgs e) {
            if (chainHistory.SelectedItems.Count < 1) return;
            Clipboard.SetText(chainHistory.SelectedItems[0].Text);
        }
    }
}