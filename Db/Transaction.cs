using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMonitor.Db
{
    public class Transaction
    {
        public Transaction(DateTime timestamp, string regNr, Direction direction)
        {
            Id = Guid.NewGuid();
            Timestamp = timestamp;
            RegNr = regNr;
            Direction = direction;
        }

        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string RegNr { get; private set; }
        public Direction Direction { get; private set; }
    }
}
