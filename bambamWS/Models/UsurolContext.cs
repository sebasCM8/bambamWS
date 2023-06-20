using Microsoft.EntityFrameworkCore;

namespace bambamWS.Models
{
    public class UsurolContext : DbContext
    {

        public UsurolContext(DbContextOptions<UsurolContext> options) : base(options)
        {

        }

        public DbSet<Usurol> Usurols { get; set; }

    }
}
