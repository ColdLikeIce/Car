using CommonCore.EntityFramework.Common;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using HeyTripCarWeb.Supplier.ACE.Models.Dbs;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dbs;
using HeyTripCarWeb.Supplier.NZ.Model.Dbs;
using HeyTripCarWeb.Supplier.Sixt.Models.Dbs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HeyTripCarWeb.Db
{
    public class CarSupplierDbContext : DbContext
    {
        public CarSupplierDbContext(DbContextOptions<CarSupplierDbContext> options) : base(options)

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

        //public DbSet<ABG_CreditCardPolicy> ABG_CreditCardPolicy { get; set; }
        public DbSet<ABGCarProReservation> ABGCarProReservation { get; set; }

        public DbSet<ABGLocation> ABGLocation { get; set; }

        //public DbSet<ABGRateCache> ABGRateCache { get; set; }
        public DbSet<AbgRqLogInfo> AbgRqLogInfo { get; set; }

        public DbSet<AbgYoungDriver> AbgYoungDriver { get; set; }

        /*        public DbSet<AceLocation> AceLocation { get; set; }
                public DbSet<ACERateCache> ACERateCache { get; set; }
                public DbSet<AceReservation> AceReservation { get; set; }*/

        public DbSet<BargainLocation> BargainLocation { get; set; }
        public DbSet<BarginCarOrder> BarginCarOrder { get; set; }
        public DbSet<BarginCategoryTypes> BarginCategoryTypes { get; set; }
        public DbSet<BarginDriverAge> BarginDriverAge { get; set; }

        public DbSet<BarginRqLogInfo> BarginRqLogInfo { get; set; }

        public DbSet<SixtCarProReservation> SixtCarProReservation { get; set; }
        public DbSet<SixtCountry> SixtCountry { get; set; }
        public DbSet<SixtLocation> SixtLocation { get; set; }
        public DbSet<SixtRqLogInfo> SixtRqLogInfo { get; set; }

        public DbSet<NZCategoryTypes> NZCategoryTypes { get; set; }
        public DbSet<NZDriverAge> NZDriverAge { get; set; }
        public DbSet<NZLocation> NZLocation { get; set; }
        public DbSet<NZUOrder> NZUOrder { get; set; }
        public DbSet<NZRqLogInfo> NZRqLogInfo { get; set; }
    }
}