using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace cryptidDemo {
    public partial class AuthForm : Form {
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
            CandidateInfoForm info = new CandidateInfoForm();
            info.PackedData = PackedData;
            info.Password = Password;
            info.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e) {

        }
    }
}
