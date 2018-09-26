using System;
using System.Collections.Generic;
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

    }
}
