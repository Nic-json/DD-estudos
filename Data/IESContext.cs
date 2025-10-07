using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rebuild.Models;

namespace rebuild.Data
{
    public class IESContext
    {

        public IESContext(DbContextOptions<IESContext> options) : base(options){}
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }



    }
}
