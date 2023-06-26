using Microsoft.EntityFrameworkCore;

namespace bambamWS.Models
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Usurol> usurol { get; set; } = null!;
        public DbSet<Categoria> categorias { get; set; } = null!;
        public DbSet<Unidad> unidades { get; set; } = null!;
        public DbSet<Producto> productos { get; set; } = null!;
        public DbSet<Ineg> inegs { get; set; } = null!;
    }
}
