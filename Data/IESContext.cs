using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rebuild.Models;

namespace rebuild.Data
{
    public class IESContext(DbContextOptions<IESContext> options) : DbContext(options)
    {
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Instituicao> Instituicao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Departamento>(e =>
            {
                e.ToTable("Departamento");     // nome da TABELA em singular
                e.HasKey(x => x.DepartamentoID);
                e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Instituicao>(e =>
            {
                e.ToTable("Instituicao");     // nome da TABELA em singular
                e.HasKey(x => x.InstituicaoID);
                e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            });
        }

    }
}
