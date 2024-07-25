using CommonCore.EntityFramework.Common;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Supplier.Sixt.Models.Dbs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HeyTripCarWeb.Db
{
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options)

        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(new EFLoggerProvider());
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
            optionsBuilder.ConfigureWarnings(b => b.Ignore(CoreEventId.ContextInitialized));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* modelBuilder.Entity<AxaProductCode>()
                .HasOne(x => x.AxaPlanCode)
                .WithMany(x => x.AxaProductCode)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.PlanId);*/
        }

        public DbSet<CarLocationSupplier> CarLocationSupplier { get; set; }
        public DbSet<CarCity> CarCity { get; set; }
    }
}