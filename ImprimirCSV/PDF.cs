using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Barcode;

namespace ImprimirCSV
{
    class PDF
    {

        private DataTable dataTable;
        private float tNeto;
        private float tCajas;

        public PDF(string pathCSV, string pathPDF, string pathLogo)
        {
            this.dataTable = loadData(pathCSV, ';');
            this.makePDF(pathPDF, pathLogo);
        }

        private DataTable loadData(string path, char separator)
        {

            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(path))
            {
                string[] headers = sr.ReadLine().Split(separator);
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(separator);
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }

        private void makePDF(string pathPDF, string pathLogo)
        {
            File.Delete(pathPDF);

            FileStream fs = new FileStream(pathPDF, FileMode.Create, FileAccess.Write, FileShare.None);

            Document doc = new Document(PageSize.A4, 36, 36, 36, 36);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            this.setLogo(doc, pathLogo);

            DataRow row = dataTable.Rows[0];

            string sscc = row["SSCC"].ToString();
            this.setBarcode(doc, writer, sscc);

            string agencia = row["AGENCIA"].ToString();
            string nom_agencia = row["NOMBRE_AGENCIA"].ToString();
            this.setAgencia(doc, writer, agencia, nom_agencia);

            string cliente = row["SUCURSAL_CLIENTE"].ToString();
            string nom_cliente = row["CLTE_RAZON_SOCIAL"].ToString();
            string direccion = row["SUC_DIRECCION_ENVIO"].ToString();
            string cp = row["SUC_D_POSTAL_ENVIO"].ToString();
            string poblacion = row["SUC_POBLACION_ENVIO"].ToString();
            this.setCliente(doc, writer, cliente, nom_cliente, direccion, cp, poblacion);

            string pedido = row["NUMERO_PEDIDO"].ToString();
            string referencia = row["REFERENCIA_PEDIDO"].ToString();
            this.setPedido(doc, writer, pedido, referencia);

            this.setTable(doc, writer);

            this.setTotal(doc, writer, "90", "150");

            doc.Close();
        }

        private void setLine(Document doc, PdfWriter writer)
        {
            float y = writer.GetVerticalPosition(false);
            PdfContentByte cb = writer.DirectContent;
            cb.MoveTo(doc.LeftMargin, y - 5);
            cb.LineTo(doc.PageSize.Width - doc.RightMargin, y - 5);
            cb.Stroke();
        }

        private void setLogo(Document doc, string path)
        {
            System.Drawing.Image myImg = System.Drawing.Image.FromFile(path);
            Image im = Image.GetInstance(myImg, System.Drawing.Imaging.ImageFormat.Jpeg);
            im.ScalePercent(20);
            doc.Add(im);
        }

        private void setBarcode(Document doc, PdfWriter writer, string barcode)
        {
            Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;
            System.Drawing.Image img = barcode39.Draw(barcode, 50);
            Image im = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Png);
            float y = writer.GetVerticalPosition(false);
            im.SetAbsolutePosition(175, y + 28);
            doc.Add(im);

            PdfContentByte cb = writer.DirectContent;
            ColumnText ct = new ColumnText(cb);
            Phrase myText = new Phrase(barcode);
            ct.SetSimpleColumn(myText, 300, y + 28, 580, 317, 15, Element.ALIGN_LEFT);
            ct.Go();
        }

        private void setAgencia(Document doc, PdfWriter writer, string codigo, string nombre)
        {
            this.setLine(doc, writer);
            Font f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            Paragraph textLine = new Paragraph("Agencia de Transportes", f);
            doc.Add(textLine);

            f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 26);
            textLine = new Paragraph(codigo+"-"+nombre, f);
            textLine = new Paragraph(nombre, f);
            textLine.SetLeading(25, 0);
            doc.Add(textLine);

            this.setLine(doc, writer);
        }

        private void setCliente(Document doc, PdfWriter writer, string codigo, string nombre, string direccion, string cp, string poblacion)
        {
            Font f = FontFactory.GetFont(FontFactory.HELVETICA, 24);

            //Paragraph textLine = new Paragraph(codigo+"-"+nombre, f);
            Paragraph textLine = new Paragraph(nombre, f);
            doc.Add(textLine);

            f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            textLine = new Paragraph(direccion, f);
            doc.Add(textLine);

            textLine = new Paragraph(cp + "-" + poblacion, f);
            doc.Add(textLine);
        }

        private void setPedido(Document doc, PdfWriter writer, string pedido, string referencia)
        {
            Font f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            Paragraph textLine = new Paragraph("PEDIDO: " + pedido + "            REF.PEDIDO:" + referencia, f);
            textLine.SpacingAfter = 10;
            doc.Add(textLine);
        }

        private void setTable(Document doc, PdfWriter writer)
        {

            PdfPTable table = new PdfPTable(5);
            table.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
            table.LockedWidth = true;
            table.SpacingBefore = 2;

            float[] widths = new float[] { 1, 7, 1, 1, 1 };
            table.SetWidths(widths);

            Font f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);

            PdfPCell cell = new PdfPCell(new Phrase("CODIGO", f));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DESCRIPCIÓN", f));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("LOTE", f));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("PESO", f));
            cell.HorizontalAlignment = 2;
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CAJAS", f));
            cell.HorizontalAlignment = 2;
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);

            this.tNeto = 0;
            this.tCajas = 0;

            this.setRows(doc, writer, table);

            doc.Add(table);
        }

        private void setRows(Document doc, PdfWriter writer, PdfPTable table)
        {

            Font f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            foreach (DataRow row in this.dataTable.Rows)
            {
                string articulo = row["CODIGO_PRODUCTO"].ToString();
                string descripcion = row["DESCRI_LARGA"].ToString();
                string lote = row["LOTE_EXPERT"].ToString();
                string peso = row["CANTIDAD"].ToString();
                string cantidad = row["CANTIDAD_CAJAS"].ToString();

                float aux = 0;
                if (float.TryParse(peso,out aux))
                {
                    this.tNeto += aux;
                }
                if (float.TryParse(cantidad, out aux))
                {
                    this.tCajas += aux;
                }

                PdfPCell cell = new PdfPCell(new Phrase(articulo, f));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(descripcion, f));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(lote, f));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(peso, f));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(cantidad, f));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);
            }

        }

        private void setTotal(Document doc, PdfWriter writer, string neto, string cajas)
        {
            Font f = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

            PdfContentByte cb = writer.DirectContent;

            cb.MoveTo(36, 30);
            cb.LineTo(doc.PageSize.Width - doc.RightMargin, 30);
            cb.Stroke();

            ColumnText ct = new ColumnText(cb);
            Phrase myText = new Phrase("TOTAL P.NETO(KG):" + this.tNeto + "                       TOTAL CAJAS:" + this.tCajas, f);
            ct.SetSimpleColumn(myText, 36, 7, 500, 25, 15, Element.ALIGN_LEFT);
            ct.Go();
        }

    }
}
