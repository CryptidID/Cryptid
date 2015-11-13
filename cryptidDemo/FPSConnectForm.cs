using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using cryptid.Scanners;

namespace cryptidDemo {
    public partial class FPSConnectForm : Form {

        private static readonly int[] _baudrates = { 9600, 19200, 38400, 57600, 115200 };

        public int Port {get; private set;} = 1;
        public int Baud { get; private set; } = _baudrates[0];
        public bool IsConnected { get; private set; } = false;

        public FPSConnectForm() {
            InitializeComponent();
        }

        private enum SerialPorts {
            COM1 = 1,
            COM2 = 2,
            COM3 = 3,
            COM4 = 4,
            COM5 = 5,
            COM6 = 6,
            COM7 = 7,
            COM8 = 8,
            COM9 = 9,
            COM10 = 10,
        }

        private void FPSConnectForm_Load(object sender, EventArgs e) {
            DialogResult = DialogResult.None;
            baudrates.DataSource = _baudrates;
            ports.DataSource = Enum.GetValues(typeof(SerialPorts)).Cast<SerialPorts>();

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
            FPS_GT511C3.Close();
            int status = FPS_GT511C3.Open(Port, Baud);
            if (status == 1) {
                DialogResult = DialogResult.OK;
                IsConnected = true;
                connectBg.BackColor = Color.Green;
                connectText.Text = "Connected";
            } else {
                DialogResult = DialogResult.None;
                IsConnected = false;
                connectBg.BackColor = Color.Red;
                connectText.Text = "Disconnected";
                FPS_GT511C3.Close();

                MessageBox.Show("Got connect error #" + status + ".\nYou may need to manually reset the device and try again.");
            }
            connectButton.Enabled = true;
            connectProgress.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
