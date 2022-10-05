using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingMonitor.Simulator.InvoiceService
{
    internal class ToJson : IInvoiceService
    {
        public Task GenerateInvoice(int year, int month, string regNr, params (DateTime Start, DateTime End)[] periods)
        {
            foreach (var p in periods)
                _entries.Add(new Entry(year, month, regNr, p.Start, p.End));
            return Task.CompletedTask;
        }

        public async Task WriteAsync(string path)
        {
            var payload = _entries.GroupBy(x => new { x.Year, x.Month })
                .Select(period => new
                {
                    Year = period.Key.Year,
                    Month = period.Key.Month,
                    Vehicles = period.GroupBy(x => x.RegNr)
                    .Select(v => new
                    {
                        RegNr = v.Key,
                        Periods = v.Select(x => new { x.Start, x.End }).ToList()
                    }).ToList()
                }).ToList();

            using var stream = File.Open(path, FileMode.Create);
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }


        private List<Entry> _entries = new();

        private record Entry(int Year, int Month, string RegNr, DateTime Start, DateTime End);

    }
}
