using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
//using Excel = Microsoft.Office.Interop.Excel;

namespace ExelTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnClickTestButton(object sender, EventArgs e)
        {
            var opd = new OpenFileDialog();
            opd.ShowDialog(this);
            Console.WriteLine(opd.FileName);
            var ext = Path.GetExtension(opd.FileName);
            if (ext == ".csv")
            {
                using (var reader = new StreamReader(opd.FileName, Encoding.Default, true))
                {
                    while (reader.EndOfStream == false)
                    {
                        var line = reader.ReadLine();
                        Console.WriteLine(line);
                    }
                }
            }
            else
            {
                Console.WriteLine(ext);
            }

            //var app = new Excel.Application();
            //var opened = app.Workbooks.Open(opd.FileName);
            //Console.WriteLine(opened);
        }
    }
}
