//	Professor	DAL
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Docente;

namespace rebuild.Data.DAL.Docente
{
    public class ProfessorDAL
    {
        private IESContext _context;
        public ProfessorDAL(IESContext context)
        {
            _context = context;
        }
        // Consulta de listagem (somente leitura)
        public IQueryable<Professor> ObterProfessorClassificadosPorNome()
            => _context.Professores
                       .AsNoTracking()
                       .OrderBy(b => b.Nome);

        // Obter por Id (somente leitura)
        public async Task<Professor?> ObterProfessorPorIdAsync(long id)
            => await _context.Professores
                             .AsNoTracking()
                             .FirstOrDefaultAsync(a => a.ProfessorID == id);

        // Gravar (Add/Update) — considera PK long com identidade (0 = novo)
        public async Task<Professor> GravarProfessorAsync(Professor professor)
        {
            if (professor is null) throw new System.ArgumentNullException(nameof(professor));

            if (professor.ProfessorID == 0)               // novo
            {
                _context.Professores.Add(professor);
            }
            else                                           // atualização
            {
                // Garante rastreamento correto mesmo vindo desacoplado (ex.: de um form)
                _context.Entry(professor).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return professor;
        }

        // Remover por Id — retorna o removido ou null se não encontrado
        public async Task<Professor?> EliminarProfessorPorIdAsync(long id)
        {
            var entity = await _context.Professores.FindAsync(id); // aqui queremos tracking
            if (entity is null) return null;

            _context.Professores.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        



    }
}


