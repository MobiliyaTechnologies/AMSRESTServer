namespace AssetMonitoring.Components.Repository.EntityFramework
{
    using System.Data.Entity;
    using AssetMonitoring.Entities;
    using System.Linq;
    using System.Data.Entity.Infrastructure;
    using System;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.SqlClient;
    using System.Data.Entity.Core.Objects;
    using System.Collections.Generic;

    public class EFDbContext : DbContext
    {
        public EFDbContext()
            : base("name=DBConnection")
        {
        }

        public DbSet<User> User { get; set; }

        public DbSet<Asset> Asset { get; set; }

        public DbSet<Gateway> Gateway { get; set; }

        public DbSet<Role> Role { get; set; }

        public DbSet<Sensor> Sensor { get; set; }

        public DbSet<Capability> Capability { get; set; }

        public DbSet<SensorCapabilityFilter> SensorCapabilityFilter { get; set; }

        public DbSet<SensorRule> SensorRule { get; set; }

        public DbSet<SensorType> SensorType { get; set; }

        public DbSet<SensorGroup> SensorGroup { get; set; }

        public DbSet<IndoorLayout> IndoorLayout { get; set; }

        // changes related to cascade delete.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorGroup>().HasMany(e => e.Assets).WithOptional(s => s.SensorGroup).WillCascadeOnDelete();
            modelBuilder.Entity<SensorGroup>().HasMany(e => e.SensorRules).WithOptional(s => s.SensorGroup).WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationConfiguration>().HasMany(e => e.ApplicationConfigurationEntries).WithOptional(s => s.ApplicationConfiguration).WillCascadeOnDelete();

            modelBuilder.Entity<Capability>().HasMany(e => e.SensorCapabilityFilters).WithOptional(s => s.Capability).WillCascadeOnDelete();

            modelBuilder.Entity<SensorCapabilityFilter>().HasMany(e => e.SensorRules).WithOptional(s => s.CapabilityFilter).WillCascadeOnDelete();

            modelBuilder.Entity<Role>().HasMany(e => e.Users).WithOptional(s => s.Role).WillCascadeOnDelete();

            modelBuilder.Entity<SensorType>().HasMany(e => e.Sensors).WithOptional(s => s.SensorType).WillCascadeOnDelete();
        }
    }
}