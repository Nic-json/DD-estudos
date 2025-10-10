using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Cadastros
{
    public class Instituicao
    {
        public long InstituicaoID { get; set; }
        public string Nome { get; set; }
        public String Endereco { get; set; }
        public virtual ICollection<Departamento>? Departamento { get; set; }
    }
}
