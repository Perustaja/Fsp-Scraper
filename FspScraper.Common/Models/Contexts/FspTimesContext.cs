using Microsoft.EntityFrameworkCore;

namespace FspScraper.Common.Models.Contexts
{
    public class FspTimesContext : DbContext
    {
        public FspTimesContext(DbContextOptions<FspTimesContext> options) : base (options) {}
        public DbSet<FspScraper.Common.Models.FspTimes> Times { get; set; }
    }
}
