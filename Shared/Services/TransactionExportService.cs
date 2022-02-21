using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lexplorer.Services
{
    public class TransactionExportService
    {
        public async Task<Stream> GenerateCSV(string format, string accountId, DateTime? startDate, DateTime? endDate)
        {
            //just simulating for now, waiting 3s with Task.Run

            var theString = "TestContent";
            var fileStream = new MemoryStream();
            byte[] data = Encoding.Unicode.GetBytes(theString);
            fileStream.Write(data, 0, data.Length);
            fileStream.Position = 0;

            await Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < 3000)
                {
                    Thread.Sleep(50);
                }
            });

            return fileStream;
        }
    }
}
