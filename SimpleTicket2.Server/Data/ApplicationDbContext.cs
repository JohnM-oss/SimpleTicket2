using Microsoft.EntityFrameworkCore;

namespace SimpleTicket2.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { Id = 1, Title = "Test Ticket 1", Description = "First test ticket", Status = TicketStatus.Open, CreatedAt = new DateTime(2025, 6, 22) },
                new Ticket { Id = 2, Title = "Test Ticket 2", Description = "Second test ticket", Status = TicketStatus.Closed, CreatedAt = new DateTime(2025, 6, 22) }
            // Add more test tickets as needed
            );
        }
    }
}
