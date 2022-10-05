using Microsoft.EntityFrameworkCore;
using ParkingMonitor.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMonitor.Service
{
    internal class MonthlyInvoiceGenerator
    {
        private readonly IDbContextFactory<Context> _contextFactory;
        private readonly IInvoiceService _invoiceGenerator;

        public MonthlyInvoiceGenerator(IDbContextFactory<Db.Context> contextFactory, IInvoiceService invoiceGenerator)
        {
            _contextFactory = contextFactory;
            _invoiceGenerator = invoiceGenerator;
        }

        public async Task GenerateInvoices(int year, int month)
        {


            using var dbContext = _contextFactory.CreateDbContext();

            var periodStart = new DateTime(year, month, 1);
            var periodEnd = new DateTime(year, month + 1, 1).AddDays(-1);

            var outgoingTransactions = await dbContext.Transactions
                .Where(x => x.Timestamp >= periodStart && x.Timestamp <= periodEnd)
                .Where(x => x.Direction == Direction.Outgoing) //Only catch outgoing transactions, as they mark an entry in the invoice.
                .ToListAsync();


            var transactionsPerVehicle = outgoingTransactions.ToLookup(x => x.RegNr);

            foreach(var vehicleInfo in transactionsPerVehicle)
            {
                var endTransactions = vehicleInfo.OrderBy(x => x.Timestamp).ToList();

                var periods = new List<(DateTime Start, DateTime End)>();


                foreach(var endTransaction in endTransactions)
                {
                    var startTransaction = await dbContext.Transactions //Find the latest incoming transaction, as it marks the beginning of the entry.
                        .Where(x => x.Timestamp < endTransaction.Timestamp)
                        .Where(x => x.Direction == Direction.Incoming)
                        .OrderByDescending(x => x.Timestamp)
                        .FirstAsync();

                    periods.Add((startTransaction.Timestamp, endTransaction.Timestamp));
                }

                await _invoiceGenerator.GenerateInvoice(year, month, vehicleInfo.Key, periods.ToArray());
            }


        }
    }
}
