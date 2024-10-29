using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Booking>()
    //        .HasOne(b => b.Event) // Define the relationship
    //        .WithMany() // Assuming an event can have multiple bookings
    //        .HasForeignKey(b => b.EventId);
    //}
}
