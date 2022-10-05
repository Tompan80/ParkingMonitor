using Microsoft.EntityFrameworkCore;
using ParkingMonitor.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMonitor.Simulator
{
    internal class TransactionGenerator
    {
        private readonly IDbContextFactory<Context> _dbContextFactory;

        public TransactionGenerator(IDbContextFactory<Db.Context> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }


        private static readonly char[] _letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

        char RandomLetter(Random r) => _letters[r.Next(0, _letters.Length)];

        char RandomDigit(Random r) => (char)((int)'0' + r.Next(10));


        public async Task Generate(int year, int month, int months, int seed)
        {
            var r = new Random(seed);

            var startPeriod = new DateTime(year, month, 1);
            var endPeriod = new DateTime(year, month, 1).AddMonths(months);


            var vehicles = Enumerable.Range(0, 100).Select(i => $"{RandomLetter(r)}{RandomLetter(r)}{RandomLetter(r)}-{RandomDigit(r)}{RandomDigit(r)}{RandomDigit(r)}").ToList();

            var entries = Enumerable.Range(0, 1000).SelectMany(i =>
            {
                var regNr = vehicles[r.Next(vehicles.Count)];

                var offset = r.Next((int)endPeriod.Subtract(startPeriod).TotalSeconds);
                var timestamp = startPeriod.Add(TimeSpan.FromSeconds(offset));

                var ret = new List<Db.Transaction>();

                if (r.NextDouble() < 0.995)
                {
                    ret.Add(new Db.Transaction(timestamp, regNr, Direction.Incoming));
                }
                if (r.NextDouble() < 0.995)
                {
                    ret.Add(new Db.Transaction(timestamp.AddSeconds(120 + r.Next(60 * 60 * 72)), regNr, Direction.Outgoing));
                }
                return ret;

            })
            .OrderBy(x => x.Timestamp)
            .ToList();

            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Transactions.AddRange(entries);

            await dbContext.SaveChangesAsync();
        }

    }
}
