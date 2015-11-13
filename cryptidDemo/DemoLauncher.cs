using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cryptidDemo {
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
    }
}
