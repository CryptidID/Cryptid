#region

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cryptid.Scanners;
using CryptidDemo.Properties;

#endregion

namespace CryptidDemo {
    public partial class FpsConnectForm : Form {
        private static readonly int[] Baudrates = {9600, 19200, 38400, 57600, 115200};

        public FpsConnectForm() {
            InitializeComponent();
        }

        public int Port { get; private set; } = 1;
        public int Baud { get; private set; } = Baudrates[0];
        public bool IsConnected { get; private set; }

        private void FPSConnectForm_Load(object sender, EventArgs e) {
            DialogResult = DialogResult.None;
            baudrates.DataSource = Baudrates;
            ports.DataSource = Enum.GetValues(typeof (SerialPorts)).Cast<SerialPorts>();

            ports.SelectedIndex = baudrates.SelectedIndex = 0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {
        }

        private void ports_SelectedIndexChanged(object sender, EventArgs e) {
            Port = (int) ports.SelectedItem;
        }

        private void baudrates_SelectedIndexChanged(object sender, EventArgs e) {
            Baud = (int) baudrates.SelectedItem;
        }

        private void connectButton_Click(object sender, EventArgs e) {
            connectProgress.Visible = true;
            connectButton.Enabled = false;
            FpsGt511C1R.Close();
            var status = FpsGt511C1R.Open(Port, Baud);
            if (status == 1) {
                DialogResult = DialogResult.OK;
                IsConnected = true;
                connectBg.BackColor = Color.Green;
                connectText.Text = Resources.FPS_CONNECTED;
            }
            else {
                DialogResult = DialogResult.None;
                IsConnected = false;
                connectBg.BackColor = Color.Red;
                connectText.Text = Resources.FPS_DISCONNECTED;
                FpsGt511C1R.Close();

                MessageBox.Show(
                    $"Got connect error #{status}.\nYou may need to manually reset the device and try again.");
            }
            connectButton.Enabled = true;
            connectProgress.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private enum SerialPorts {
            Com1 = 1,
            Com2 = 2,
            Com3 = 3,
            Com4 = 4,
            Com5 = 5,
            Com6 = 6,
            Com7 = 7,
            Com8 = 8,
            Com9 = 9,
            Com10 = 10
        }
    }
}