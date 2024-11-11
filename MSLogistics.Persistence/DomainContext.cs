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

        public DomainContext() : base()
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Backup>()
        //      .HasOne<Device>()
        //      .WithOne(d => d.Backup)
        //      .HasForeignKey<Backup>(b => b.DeviceId)
        //      .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}

