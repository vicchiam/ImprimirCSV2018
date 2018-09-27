using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImprimirCSV
{
    class ProccessTask
    {

        private readonly static int SECONDS = 30;

        private Thread thread;
        private bool stop;

        public ProccessTask()
        {
            this.stop = false;
            this.thread = new Thread(new ThreadStart(Loop));
        }

        public void startTask()
        {
            thread.Start();            
        }

        public void stopTask()
        {
            this.stop = true;            
        }

        private void Loop()
        {
            Settings.writeLog("Procesando Iniciado");
            while (!this.stop)
            {
                processTask();
                Thread.Sleep(SECONDS * 1000);
            }
            Settings.writeLog("Procesando Detenido");
        }


        private void processTask()
        {            
            List<string[]> rows = Settings.loadSettings();
            foreach (string[] row in rows)
            {
                string folder = row[0];
                string printer = row[1];
                string prefix = row[2];

                this.checkFiles(folder, printer, prefix);
            }
        }

        private void checkFiles(string folder, string printer, string prefix)
        {
            Settings.writeLog("Procesando "+folder);            
            if (Directory.Exists(folder))
            {
                string[] pathFiles = Directory.GetFiles(folder);
                foreach (string filePath in pathFiles)
                {
                    string file = Path.GetFileName(filePath);
                    if (file.StartsWith(prefix))
                    {
                        Settings.writeLog("Procesando " + file);
                        PDF pdf = new PDF(filePath, "out.pdf", "logo.jpg");
                        Settings.writeLog("Imprimiendo " + file+" por "+printer);
                        SendToPrinter(printer);
                    }
                }
            }
        }

        private void SendToPrinter(string printer)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = "\"" + printer + "\"";                        
            info.Verb = "printto";
            info.FileName = "out.pdf";
            
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }



    }
}
