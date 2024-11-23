using Microsoft.EntityFrameworkCore;
using MSLogistics.Domain;

namespace MSLogistics.Persistence
{
    public class DomainContext : DbContext
    {
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<DispatchGroup> DispatchGroups { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }

        public DomainContext(DbContextOptions<DomainContext> options) : base(options) { }

        public DomainContext() : base() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships
            modelBuilder.Entity<Stop>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Stops)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seeding logic
            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Define IDs for seed data
            var vehicle1Id = Guid.NewGuid();
            var route1Id = Guid.NewGuid();
            var stop1Id = Guid.NewGuid();
            var stop2Id = Guid.NewGuid();

            // Seed Vehicles
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle
                {
                    Id = vehicle1Id,
                    RegistrationNumber = "ABC123",
                    LoadCapacity = 1500,
                    VehicleModel = "Model X",
                    VehicleMake = "Tesla"
                }
            );

            // Seed Route
            modelBuilder.Entity<Route>().HasData(
                new Route
                {
                    Id = route1Id,
                    Name = "Route A",
                    VehicleId = vehicle1Id  // Associate with Vehicle
                }
            );

            // Seed Stops and associate them with the Route
            modelBuilder.Entity<Stop>().HasData(
                new Stop
                {
                    Id = stop1Id,
                    Name = "Stop 1",
                    CustomerId = Guid.NewGuid(),
                    RouteId = route1Id  // Associate with Route A
                },
                new Stop
                {
                    Id = stop2Id,
                    Name = "Stop 2",
                    CustomerId = Guid.NewGuid(),
                    RouteId = route1Id  // Associate with Route A
                }
            );
        }
    }
}


