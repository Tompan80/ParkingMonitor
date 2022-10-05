using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMonitor
{
    internal interface IInvoiceService
    {
        Task GenerateInvoice(int year, int month, string regNr, params (DateTime Start, DateTime End)[] periods);
    }
}
