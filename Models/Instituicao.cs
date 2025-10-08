using System.Collections.Generic;
namespace rebuild.Models
{
    public class Instituicao
    {
        public long? InstituicaoID { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public virtual ICollection<Departamento>? Departamento { get; set; }

    }

    public class Departamento
    {
        public long? DepartamentoID { get; set; }
        public string Nome { get; set; }
        public long? InstituicaoID { get; set; }
        public Instituicao? Instituicao { get; set; }
    }
}