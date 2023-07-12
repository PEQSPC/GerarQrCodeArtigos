using OfficeOpenXml;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GerarQrCodeArquivos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Label
        {
            private string artigo { get; set; }


            public Bitmap QRCodeImage { get; set; }
            public Bitmap QRCodeBitmap { get; set; }
        }


        private static void GerarQrode()
        {
            // Show a file dialog box to let the user choose an Excel file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;";
            openFileDialog.Title = "Selecionar Ficheiro Excel com os artigos";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Show a folder browser dialog to let the user choose the folder to save the QR code PNG files
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Selecione a pasta para salvar os QR Code para cada Artigo";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;

                    using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.First();

                        // Read the data from the Excel file and create a label for each row
                        List<Label> labels = new List<Label>();

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            // Se startRow ultrapassar a última linha do arquivo existente, sair do loop
                            if (row > worksheet.Dimension.End.Row)
                            {
                                break;
                            }

                            string artigo = worksheet.Cells[row, 1].Value.ToString();

                            Label label = new Label();

                            // Generate QR code for the label
                            string qrText = $"{artigo}";
                            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                            {
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                                using (QRCode qrCode = new QRCode(qrCodeData))
                                {
                                    Bitmap qrImage = qrCode.GetGraphic(5);
                                    label.QRCodeImage = qrImage;
                                }
                            }
                            labels.Add(label);

                            // Save the QR code image as a PNG file
                            string fileName = $"{artigo}.png";
                            string filePath = Path.Combine(folderPath, fileName);
                            label.QRCodeImage.Save(filePath, ImageFormat.Png);
                        }
                    }
                }
            }
        }




        private void btn_abrirExcel_Click(object sender, EventArgs e)
        {
            GerarQrode();


        }

    }
}
