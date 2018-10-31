using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprimirCSV
{
    class Settings
    {

        public static List<String> getListPrinters()
        {
            List<string> list = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                list.Add(printer);
            }
            return list;
        }

        public static void saveSettings(string[] row)
        {
            string line = string.Join(";", row) + "" + Environment.NewLine;
            File.AppendAllText("data.dat", line);
        }

        public static List<string[]> loadSettings()
        {
            List<string[]> res = new List<string[]>();
            if (!File.Exists("data.dat"))
            {
                File.Create("data.dat").Close();                
            }
            foreach (string line in File.ReadAllLines("data.dat"))
            {
                string[] aux = line.Split(';');
                res.Add(aux);
            }         
            return res;
        }

        public static bool deleteSettings(string path)
        {
            bool res = false;
            string text = "";
            foreach (string line in File.ReadAllLines("data.dat"))
            {
                string[] aux = line.Split(';');
                if (aux[0]==path)
                    res = true;                
                else
                    text += line + Environment.NewLine;
            }
            File.WriteAllText("data.dat", text);
            return res;
        }

        public static void initLog()
        {
            if (File.Exists("log.dat"))
            {
                File.Delete("log.dat");
            }
            File.Create("log.dat").Close();            
        }

        public static string readLog()
        {            
            return File.ReadAllText("log.dat");
        }

        public static void writeLog(string text)
        {
            string[] lines = File.ReadAllLines("log.dat");
            if (lines.Length > 1000) {                               
                File.WriteAllLines("log.dat", lines.Skip(100).ToArray());
            }
            DateTime now = DateTime.Now;
            string timestamp = now.Year + "/" + now.Month.ToString("D2") + "/" + now.Day.ToString("D2") + "  " + now.Hour.ToString("D2") + ":" + now.Minute.ToString("D2");
            File.AppendAllText("log.dat",timestamp+" "+text+Environment.NewLine);
        }       

    }
}
