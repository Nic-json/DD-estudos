using Modelo.Cadastros;
using Modelo.Docente;
using rebuild.Data;
using Microsoft.EntityFrameworkCore;

namespace rebuild.Data.DAL.Cadastros
{
    public class CursoDAL
    {
        private IESContext _ctx;
        public CursoDAL(IESContext context)
        {
            _ctx = context;
        }
        //	ObterCursosPorDepartamento	-	CursoDAL
        public IQueryable<Curso> ObterCursosPorDepartamento(long departamentoID)
    => _ctx.Cursos
               .Where(c => c.DepartamentoID == departamentoID)
               .OrderBy(c => c.Nome);


        public IQueryable<Professor> ObterProfessoresForaDoCurso(long cursoID)
        {
            var curso = _ctx.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursosProfessores).First();
            var professoresDoCurso = curso.CursosProfessores.Select(cp =>cp.ProfessorID).ToArray();
            var professoresForaDoCurso = _ctx.Professores.Where(p =>!professoresDoCurso.Contains(p.ProfessorID));
            return professoresForaDoCurso;
        }
        public void RegistrarProfessor(long cursoID, long professorID)
        {
            var curso = _ctx.Cursos.Where(c => c.CursoID == cursoID).
            Include(cp => cp.CursosProfessores).First();
            var professor = _ctx.Professores.Find(professorID);
            curso.CursosProfessores.Add(new CursoProfessor()
            {
                Curso = curso,
                Professor = professor
            });
            _ctx.SaveChanges();
        }

        public async Task<Curso> GravarCursoAsync(Curso curso)
        {
            if (curso == null) throw new ArgumentNullException(nameof(curso));

            if (curso.CursoID == 0)
                _ctx.Cursos.Add(curso);
            else
                _ctx.Entry(curso).State = EntityState.Modified;

            await _ctx.SaveChangesAsync();
            return curso;
        }

        public async Task<Curso?> EliminarCursoPorIdAsync(long id)
        {
            var curso = await ObterCursoPorIdAsync(id);
            if (curso != null)
            {
                _ctx.Cursos.Remove(curso);
                await _ctx.SaveChangesAsync();
            }
            return curso;
        }

        public async Task<Curso?> ObterCursoPorIdAsync(long id)
        {
            return await _ctx.Cursos.FirstOrDefaultAsync(m => m.CursoID == id);
        }
    }


}

