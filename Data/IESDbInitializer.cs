using rebuild.Models;
using rebuild.Data;
using System.Linq;

namespace rebuild.Data
{
    public class IESDbInitializer
    {
        public static void Initialize(IESContext context)
        {
            context.Database.EnsureCreated();
            if (context.Departamento.Any())
            {
                return;
            }
            var departamentos = new Departamento[]
            {
                                                                new Departamento    {   Nome="Ciência	da	Computação"},
                                                                new Departamento    {   Nome="Ciência	de	Alimentos"}
            };
            foreach (Departamento d in departamentos)
            {
                context.Departamento.Add(d);
            }
            context.SaveChanges();
        }
    }
}
