using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modelo.Cadastros;
using Modelo.Discente;


namespace rebuild.Data
{
    public class IESContext(DbContextOptions<IESContext> options) : IdentityDbContext<UsuarioDaAplicacao>(options)
    {
        public DbSet<Academico> Academicos { get; set;}
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<CursoDisciplina> CursoDisciplinas { get; set; }
            public DbSet<Departamento> Departamento { get; set; }
            public DbSet<Instituicao> Instituicao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            
            modelBuilder.Entity<CursoDisciplina>(e =>
            {
                e.ToTable("CursoDisciplina");

                // PK composta
                e.HasKey(x => new { x.CursoID, x.DisciplinaID });

                // Curso 1..N CursoDisciplinas
                e.HasOne(x => x.Curso)
                 .WithMany()
                 .HasForeignKey(x => x.CursoID)
                 .OnDelete(DeleteBehavior.NoAction); // ou NoAction/Restrict se houver "multiple cascade paths"

                // Disciplina 1..N CursoDisciplinas
                e.HasOne(x => x.Disciplina)
                 .WithMany()
                 .HasForeignKey(x => x.DisciplinaID)
                 .OnDelete(DeleteBehavior.NoAction); // idem observação acima
            });
        }

    }
}
