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

        private List<string[]> listeners;
        private ProccessTask proccessTask;

        public MainForm()
        {
            InitializeComponent();

            this.init();
            this.loadSettings();            
            this.loadPrintersList();
            this.loadLog();

            listeners = new List<string[]>();
            proccessTask = new ProccessTask();
        }

        private void init()
        {
            Settings.initLog();
            this.Icon = new System.Drawing.Icon("document.ico");
            this.MaximizeBox = false;
            notifyIcon.Icon = new System.Drawing.Icon("document.ico");
            listView.View = View.Details;            
        }

        private void loadLog()
        {
            logTextBox.Text = Settings.readLog();
        }

        private void loadSettings()
        {
            this.listeners = Settings.loadSettings();
            foreach (string[] row in this.listeners)
            {
                ListViewItem item = new ListViewItem(row);
                listView.Items.Add(item);
            }
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
            proccessTask.stopTask();
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
                MessageBox.Show("Debes seleccionar un directorio", "Advertencia");
            }
            else if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show("Debes seleccionar una impresora", "Advertencia");
            }
            else if (string.IsNullOrWhiteSpace(prefix))
            {
                MessageBox.Show("Debes indicar un prefijo", "Advertencia");
            }
            else
            {
                string[] aux = { path, printer, prefix };
                if (this.checkRepeatDirectory(aux))
                {
                    MessageBox.Show("El directorio ya esta en uso", "Advertencia");
                }
                else
                {
                    this.listeners.Add(aux);
                    ListViewItem item = new ListViewItem(aux);
                    listView.Items.Add(item);
                    Settings.saveSettings(aux);
                }                
            }
        }

        private bool checkRepeatDirectory(string[] newItem)
        {            
            foreach (string[] item in this.listeners)
            {
                if (item[0] == newItem[0]) return true;
            }
            return false;
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView.FocusedItem.Bounds.Contains(e.Location))
                {                    
                    contextMenuStripListView.Show(Cursor.Position);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            ListViewItem item=listView.SelectedItems[0];
            string path = item.SubItems[0].Text;
            if (Settings.deleteSettings(path))
            {
                item.Remove();
            }
        }

        private void startStopMenuItem_Click(object sender, EventArgs e)
        {
            if (notifyIcon.ContextMenuStrip.Items[0].Text == "Iniciar Escucha")
            {                
                proccessTask.startTask();
                notifyIcon.ContextMenuStrip.Items[0].Text = "Detener Escucha";
            }
            else
            {
                proccessTask.stopTask();
                notifyIcon.ContextMenuStrip.Items[0].Text = "Iniciar Escucha";
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            this.loadLog();
        }
    }
}
