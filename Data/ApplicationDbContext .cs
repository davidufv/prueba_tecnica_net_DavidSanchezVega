using Microsoft.EntityFrameworkCore;
using prueba_tecnica_net.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<BalancePriceBd> BalancePricesBd { get; set; }
}

