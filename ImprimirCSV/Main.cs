using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImprimirCSV
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Icon = new System.Drawing.Icon("document.ico");
            this.ShowInTaskbar = false;
            this.ShowIcon = false;

            notifyIcon.Icon = new System.Drawing.Icon("document.ico");

            this.loadPrintersList();
        }

        private void loadPrintersList()
        {
            List<string> printers = Settings.getListPrinters();
            printersCombo.Items.AddRange(printers.ToArray());
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState=FormWindowState.Normal;
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void directoryTextBox_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult result = fd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
            {
                directoryTextBox.Text = fd.SelectedPath;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string path = directoryTextBox.Text;
            string printer = printersCombo.Text;
            string prefix = prefixTextBox.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Debes seleccionar un directorio","Advertencia");
            }
            else if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show("Debes seleccionar una impresora","Advertencia");
            }
            else if (string.IsNullOrWhiteSpace(prefix))
            {
                MessageBox.Show("Debes indicar un prefijo","Advertencia");
            }

        }
    }
}
