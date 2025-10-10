using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Cadastros
{
    public class Disciplina
    {
        public long DisciplinaID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public virtual ICollection<CursoDisciplina>? CursosDisciplinas	{	get;	set;	}
}
}
