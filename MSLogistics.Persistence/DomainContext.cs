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
               .HasOne<Route>()
               .WithMany(r => r.Stops)
               .HasForeignKey(s => s.RouteId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Route>()
               .HasOne<DispatchGroup>()
               .WithMany(d => d.Routes) 
               .HasForeignKey(r => r.DispatchGroupId) 
               .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}


