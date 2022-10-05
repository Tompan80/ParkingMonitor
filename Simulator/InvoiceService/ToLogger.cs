using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMonitor.Simulator.InvoiceService
{
    internal class ToLogger : IInvoiceService
    {
        private readonly ILogger<IInvoiceService> _logger;

        public ToLogger(ILogger<IInvoiceService> logger)
        {
            _logger = logger;
        }

        public Task GenerateInvoice(int year, int month, string regNr, params (DateTime Start, DateTime End)[] periods)
        {
            var builder = new StringBuilder();

            _logger.LogInformation(
$@"Invoice for {regNr}, period {year}-{month}
-------------------------------------------------

    Periods:
{periods.Select(p => $"\t{p.Start}-{p.End}").Aggregate((src, next) => $"{src}\n{next}")}
");

            return Task.CompletedTask;
        }
    }
}
