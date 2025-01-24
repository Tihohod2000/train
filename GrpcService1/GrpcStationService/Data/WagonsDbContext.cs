using Microsoft.EntityFrameworkCore;

namespace GrpcStationService.Data
{
    public class Wagon
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime DepartureDate { get; set; }
    }

    public class WagonsDbContext : DbContext
    {
        public WagonsDbContext(DbContextOptions<WagonsDbContext> options) : base(options)
        {
        }

        public DbSet<Wagon> Wagons { get; set; }
    }
}