

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ParkingMonitor
{
    public static class Program
    {

        public static async Task Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<Service.MonthlyInvoiceGenerator>();

                    //Simulator stuff
                    services.AddTransient<Simulator.TransactionGenerator>();
                    services.AddTransient<IInvoiceService, Simulator.InvoiceService.ToLogger>();


                    services.AddSingleton<Simulator.InvoiceService.ToJson>();



                    services.AddTransient<IInvoiceService>(sp => sp.GetRequiredService<Simulator.InvoiceService.ToJson>());
                    //services.AddTransient<IInvoiceService, Simulator.InvoiceService.ToLogger>();



                    services.AddDbContextFactory<Db.Context>(opt =>
                    {
                        opt.UseInMemoryDatabase("ParkingMonitor", imopt =>
                        {

                        });
                    });
                })

                .Build();


            await host.StartAsync();


            var sim_transactionGenerator = host.Services.GetRequiredService<Simulator.TransactionGenerator>();
            await sim_transactionGenerator.Generate(2022, 8, 5, 23423);

            var generator = host.Services.GetRequiredService<Service.MonthlyInvoiceGenerator>();



            await generator.GenerateInvoices(2022, 10);

            var jsonwriter = host.Services.GetRequiredService<Simulator.InvoiceService.ToJson>();
            await jsonwriter.WriteAsync("output.json");

            await host.StopAsync();
        }
    }
}

