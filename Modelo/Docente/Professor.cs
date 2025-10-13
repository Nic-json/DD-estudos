using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Docente
{
    public class Professor
    {
        public long ProfessorID { get; set; }  // <- NÃO usar long?
        [Required, StringLength(100)]
        public string Nome { get; set; } = "";
        public ICollection<CursoProfessor> CursosProfessores { get; set; } = new List<CursoProfessor>();
    }
}

