using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
namespace barcodesgui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public record PrinterInfo(int room, int printer, [Optional] int? pages)
        {
            public override string ToString()
            {
                if (pages is null)
                {
                    return $"Room {room} | Printer {printer} | Pages not yet entered";
                }
                else
                {
                    return $"Room {room} | Printer {printer} | {pages} pages printed";
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            txtPageCount.Focus();
        }
        bool written = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtPageCount.Text != "")
            {
                PrinterInfo pi = new PrinterInfo(int.Parse(txtRoomNum.Text), int.Parse(txtPrinterNum.Text), int.Parse(txtPageCount.Text));
                Export(pi);
            }
            else
            {
                MessageBox.Show("Please enter a page count!");
            }
            txtPageCount.Focus();
        }

        void Export(PrinterInfo info)
        {
            string path = getPath();
            string header = "room_num,printer_id,page_count";
            if (!Directory.Exists(@"H:/printercount/"))
            {
                Directory.CreateDirectory(@"H:/printercount/");
            }
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            List<string> lines = File.ReadAllLines(path).ToList();
            if (lines.Count == 0)
            {
                lines.Add(header);
            }
            lines.Add($"{info.room},{info.printer},{info.pages}");
            File.WriteAllLines(path, lines);
            written = false;
            txtPrinterNum.Text = "";
            txtRoomNum.Text = "";
            txtPageCount.Text = "";
        }

        private void txtPageCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string text = txtPageCount.Text;
            if (text.Length > 7)
            {
                txtRoomNum.Text = text[..3];
                txtPrinterNum.Text = text[3..];
                txtPageCount.Text = "";
                written = true;
            }
        }

        string getPath()
        {
            string path = @"H:/printercount/";
            string semester = DateTime.Now.Month switch
            {
                <= 6 => "Spring",
                _ => "Fall"
            };
            semester += DateTime.Now.Year.ToString();
            semester += ".csv";
            path = Path.Combine(path, semester);
            return path;
        }
    }
}
