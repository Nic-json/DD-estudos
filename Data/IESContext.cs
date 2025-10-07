using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rebuild.Models;

namespace rebuild.Data
{
    public class IESContext : DbContext
    {

        public IESContext(DbContextOptions<IESContext> options) : base(options){}
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }



    }
}
