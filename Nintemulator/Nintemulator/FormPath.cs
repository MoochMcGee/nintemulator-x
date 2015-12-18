using System;
using System.Windows.Forms;

namespace Nintemulator
{
    public partial class FormPath : Form
    {
        public FormPath()
        {
            InitializeComponent();
        }

        private void buttonFmc_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.FmcPath = textBoxFmc.Text = folderBrowserDialog.SelectedPath;
        }
        private void buttonSfc_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.SfcPath = textBoxSfc.Text = folderBrowserDialog.SelectedPath;
        }
        private void buttonGmb_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.GmbPath = textBoxGmb.Text = folderBrowserDialog.SelectedPath;
        }
        private void buttonGbc_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.GbcPath = textBoxGbc.Text = folderBrowserDialog.SelectedPath;
        }
        private void buttonGba_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.GbaPath = textBoxGba.Text = folderBrowserDialog.SelectedPath;
        }
        private void buttonNds_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                Program.Settings.NdsPath = textBoxNds.Text = folderBrowserDialog.SelectedPath;
        }

        private void FormPath_Load(object sender, EventArgs e)
        {
            textBoxFmc.Text = Program.Settings.FmcPath;
            textBoxSfc.Text = Program.Settings.SfcPath;
            textBoxGmb.Text = Program.Settings.GmbPath;
            textBoxGbc.Text = Program.Settings.GbcPath;
            textBoxGba.Text = Program.Settings.GbaPath;
            textBoxNds.Text = Program.Settings.NdsPath;
        }
    }
}