using rebuild.Models;
using rebuild.Data;
using System.Linq;

namespace rebuild.Data
{
    public class IESDbInitializer
    {
        public static void Initialize(IESContext context)
        {
            context.Database.EnsureDeleted();

            context.Database.EnsureCreated();
            if (context.Departamento.Any())
            {
                return;
            }
            var instituicao = new Instituicao[]
{
                new Instituicao {   Nome="UniParaná", Endereco="Paraná"},
                new Instituicao {   Nome="UniAcre", Endereco="Acre"}
};
            foreach (Instituicao i in instituicao)
            {
                context.Instituicao.Add(i);
            }
            context.SaveChanges();

            var departamento = new Departamento[]
{
                new Departamento    {   Nome="Ciência	da	Computação", InstituicaoID=1 },
                new Departamento    {   Nome="Ciência	de	Alimentos", InstituicaoID=2}
};
            foreach (Departamento d in departamento)
            {
                context.Departamento.Add(d);
            }
            context.SaveChanges();


        }
    }
}
