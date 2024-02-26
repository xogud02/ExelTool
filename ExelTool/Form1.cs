using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace ExelTool
{
    public partial class Form1 : Form
    {
        public Form1() => InitializeComponent();
        private string CurrentPath { get; set; } //TODO
        private string Extension => ".csv"; // TODO?

        private void OnClickTestButton(object sender, EventArgs e)
        {
            var opd = new FolderBrowserDialog();
            opd.ShowDialog(this);
            CurrentPath = opd.SelectedPath;
            foreach (var line in ReadOOO())
            {
                Console.WriteLine(line);
            }
        }

        private T[] Read<T>(string fileName, Func<string[], T> selector)
        {
            var fullPath = Path.Combine(CurrentPath, fileName + Extension);
            try
            {
                return File.ReadAllLines(fullPath, Encoding.Default).Select(_ => _.Split(',')).Select(selector).ToArray();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return default;
        }

        private (string Alias, string Adress, DateTime Start, DateTime End)[] ReadAdress() => Read("address", SelectAdress);
        private (string Alias, string Adress, DateTime Start, DateTime End) SelectAdress(string[] _) => (_[1], _[2], DateTime.Parse(_[3]), DateTime.Parse(_[4]));

        private (DateTime Date, string Place)[] ReadCorpCard() => Read("corpcard", SelectCorpCard);
        private (DateTime Date, string Place) SelectCorpCard(string[] _) => (DateTime.Parse(_[1]), _[2]);

        private (string Place, DateTime Date)[] ReadOOO() => Read("ooo", SelectOOO);
        private (string Place, DateTime Date) SelectOOO(string[] _) => (_[0], DateTime.Parse(_[1]));
        private (DateTime Date, float Ratio)[] ReadPto() => Read("pto", SelectPto);
        private (DateTime Date, float Ratio) SelectPto(string[] _) => (DateTime.Parse(_[0]), float.Parse(_[1]));

        private (DateTime Start, DateTime End) ReadWorkFromHome(string[] _)
        {
            var start = DateTime.Parse(_[0]);

            if (DateTime.TryParse(_[1], out var end) == false)
            {
                end = start;
            }

            return (start, end);
        }
    }
}
