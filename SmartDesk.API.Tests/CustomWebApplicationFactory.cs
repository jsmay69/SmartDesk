using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmartDesk.Infrastructure.Persistence;

namespace SmartDesk.API.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SmartDeskDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<SmartDeskDbContext>(opts => opts.UseInMemoryDatabase("InMemoryDbForTesting"));
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartDeskDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
