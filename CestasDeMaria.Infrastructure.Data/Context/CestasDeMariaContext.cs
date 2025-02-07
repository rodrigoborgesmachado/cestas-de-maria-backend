using CestasDeMaria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace CestasDeMaria.Infrastructure.Data.Context
{
    public class CestasDeMariaContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        private CestasDeMariaContext()
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public CestasDeMariaContext(IConfiguration configuration)
            : base()
        {
            _configuration = configuration;
        }

        public CestasDeMariaContext(string connectionString)
            : base()
        {
            this.connectionString = connectionString;
        }

        #region DbSet

        public DbSet<Logger> Logger { get; set; }
        public DbSet<Mailmessage> MailMessage { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Basketdeliveries> Basketdeliveries { get; set; }
        public DbSet<Basketdeliverystatus> Basketdeliverystatus { get; set; }
        public DbSet<Families> Families { get; set; }
        public DbSet<Familyfamilystatushistory> Familyfamilystatushistory { get; set; }
        public DbSet<Familystatus> Familystatus { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = string.IsNullOrEmpty(connectionString)
                ? _configuration["connectionstring"]
                : connectionString;

            optionsBuilder
                .UseSqlServer(connString, options =>
                {
                    options.CommandTimeout(180); // Set the timeout to 180 seconds
                })
#if DEBUG
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information); // Log SQL queries in debug mode
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logger>().HasKey(e => new { e.Id });
            modelBuilder.Entity<Mailmessage>().HasKey(e => new { e.Id });

            modelBuilder.Entity<Families>()
            .HasOne(sc => sc.Familystatus)
            .WithMany()
            .HasForeignKey(sc => new { sc.Familystatusid });
            modelBuilder.Entity<Families>()
            .HasOne(sc => sc.Admins)
            .WithMany()
            .HasForeignKey(sc => new { sc.Createdby });
            modelBuilder.Entity<Families>()
            .HasMany(sc => sc.Familyfamilystatushistory)
            .WithOne(sc => sc.Families)
            .HasForeignKey(sc => new { sc.Familyid });
            modelBuilder.Entity<Families>()
            .HasMany(sc => sc.Basketdeliveries)
            .WithOne(sc => sc.Families)
            .HasForeignKey(sc => new { sc.Familyid });

            modelBuilder.Entity<Logger>()
            .HasOne(sc => sc.Admins)
            .WithMany()
            .HasForeignKey(sc => new { sc.Adminid });

            modelBuilder.Entity<Familyfamilystatushistory>()
            .HasOne(sc => sc.NewFamilystatus)
            .WithMany()
            .HasForeignKey(sc => new { sc.Newfamilystatusid });
            modelBuilder.Entity<Familyfamilystatushistory>()
            .HasOne(sc => sc.OldFamilystatus)
            .WithMany()
            .HasForeignKey(sc => new { sc.Oldfamilystatusid });

            modelBuilder.Entity<Basketdeliveries>()
            .HasOne(sc => sc.Basketdeliverystatus)
            .WithMany()
            .HasForeignKey(sc => new { sc.Deliverystatusid });

            base.OnModelCreating(modelBuilder);
        }

        public void CreateDefaultData(object model)
        {
            var dateNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            // Recursively traverse the model and its properties
            SetPropertiesRecursively(model, dateNow);
        }

        private void SetPropertiesRecursively(object obj, DateTime dateNow)
        {
            if (obj == null)
                return;

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var propertyType = property.PropertyType;

                // Check if the property is a collection (IEnumerable but not a string)
                if (propertyType.FullName.Contains("System.Collections.Generic.IEnumerable"))
                {
                    var list = property.GetValue(obj) as IEnumerable<object>;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            SetPropertiesRecursively(item, dateNow);
                        }
                    }
                }
                // For complex objects (classes) that are not string, recursively set properties
                else if (propertyType.IsClass && propertyType != typeof(string))
                {
                    var nestedObject = property.GetValue(obj);
                    if (nestedObject != null)
                    {
                        SetPropertiesRecursively(nestedObject, dateNow);
                    }
                }
                else
                {
                    // Set specific properties (Code, Created, Updated) as required
                    if (property.Name == "Code" && string.IsNullOrEmpty((string)property.GetValue(obj)))
                    {
                        property.SetValue(obj, Guid.NewGuid().ToString());
                    }
                    else if (property.Name == "Created" && (!HasValue(property.GetValue(obj)) || (DateTime)property.GetValue(obj) == DateTime.MinValue))
                    {
                        property.SetValue(obj, dateNow);
                    }
                    else if (property.Name == "Updated" && (!HasValue(property.GetValue(obj)) || (DateTime)property.GetValue(obj) == DateTime.MinValue))
                    {
                        property.SetValue(obj, dateNow);
                    }
                }
            }
        }

        private bool HasValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            // If the value is a string, check if it's null or empty
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }

            // If the value is a DateTime, check if it's DateTime.MinValue
            if (value is DateTime dateTime)
            {
                return dateTime != DateTime.MinValue;
            }

            // If the value is nullable, check if it has a value
            var type = value.GetType();
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return ((dynamic)value).HasValue;
            }

            // For other types, assume they are valid if not null
            return true;
        }

        private void ConfigSaveUpdate()
        {
            var dateNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("Created") != null && entry.Entity.GetType().GetProperty("Updated") != null))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if(entry.Property("Created").CurrentValue == null || (DateTime)entry.Property("Created").CurrentValue == DateTime.MinValue)
                            entry.Property("Created").CurrentValue = dateNow;

                        entry.Property("Updated").CurrentValue = dateNow;
                        entry.Property("IsActive").CurrentValue = (byte)1;
                        entry.Property("IsDeleted").CurrentValue = (byte)0;

                        var t = entry.Entity.GetType();

                        if (t.GetProperty("Code") == null)
                        {
                            continue;
                        }

                        string pKey = (string)t.GetProperty("Code").GetValue(entry.Entity, null);

                        if (string.IsNullOrEmpty(pKey))
                        {
                            pKey = Guid.NewGuid().ToString();
                            t.GetProperty("Code").SetValue(entry.Entity, pKey, null);
                        }

                        break;

                    case EntityState.Modified:
                        entry.Property("Created").IsModified = false;
                        entry.Property("Updated").CurrentValue = dateNow;
                        break;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            ConfigSaveUpdate();
            return await base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            ConfigSaveUpdate();
            return base.SaveChanges();
        }
    }
}
