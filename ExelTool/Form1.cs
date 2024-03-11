using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

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
            const int year = 2023; // TODO
            var start = new DateTime(year, 1, 1);
            var end = start.AddYears(1).AddDays(-1);
            var set = new HashSet<DateTime>();

            ForEachDay(start, end, _ => set.Add(_));

            foreach (var data in ReadOutOfOffice())
            {
                set.Remove(Extract(data.Date));
            }

            foreach (var data in ReadPaidTimeOff())
            {
                set.Remove(Extract(data.Date));
            }

            foreach (var data in ReadWorkFromHome())
            {
                ForEachDay(data.Start, data.End, _ => set.Remove(_));
            }

            ReadAdress();
            ReadCorpCard();


            foreach (var d in set.OrderBy(_ => _))
            {
                Console.Write(d.ToShortDateString() + " ");
                if (d.DayOfWeek == DayOfWeek.Saturday)
                {
                    Console.WriteLine();
                }
            }


        }

        private DateTime Extract(DateTime raw) => new DateTime(raw.Year, raw.Month, raw.Day);

        private void ForEachDay(DateTime from, DateTime to, Action<DateTime> dayAction)
        {
            var end = Extract(to);
            for (var d = Extract(from); d <= end; d = d.AddDays(1))
            {
                dayAction(d);
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

        //TODO 파일명 추상화, 리턴타입 뭔가 가공
        private (string Alias, string Adress, DateTime Start, DateTime End)[] ReadAdress() => Read("address", SelectAdress);
        private (string Alias, string Adress, DateTime Start, DateTime End) SelectAdress(string[] _) => (_[1], _[2], DateTime.Parse(_[3]), DateTime.Parse(_[4]));

        private (DateTime Date, string Place)[] ReadCorpCard() => Read("corpcard", SelectCorpCard);
        private (DateTime Date, string Place) SelectCorpCard(string[] _) => (DateTime.Parse(_[1]), _[2]);

        private (string Place, DateTime Date)[] ReadOutOfOffice() => Read("ooo", SelectOutOfOffice);
        private (string Place, DateTime Date) SelectOutOfOffice(string[] _) => (_[0], DateTime.Parse(_[1]));
        private (DateTime Date, float Ratio)[] ReadPaidTimeOff() => Read("pto", SelectPaidTimeOff);
        private (DateTime Date, float Ratio) SelectPaidTimeOff(string[] _) => (DateTime.Parse(_[0]), float.Parse(_[1]));
        private (DateTime Start, DateTime End)[] ReadWorkFromHome() => Read("wfh", SelectWorkFromHome);
        private (DateTime Start, DateTime End) SelectWorkFromHome(string[] _)
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
