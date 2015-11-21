#region

using System;
using System.Windows.Forms;
using cryptidDemo;

#endregion

namespace CryptidDemo {
    public partial class DemoLauncher : Form {
        public DemoLauncher() {
            InitializeComponent();
        }

        private void launchGenButton_Click(object sender, EventArgs e) {
            new GeneratorForm().Show();
        }

        private void launchAuthButton_Click(object sender, EventArgs e) {
            new AuthForm().Show();
        }

        private void DemoLauncher_Load(object sender, EventArgs e) {
        }
    }
}