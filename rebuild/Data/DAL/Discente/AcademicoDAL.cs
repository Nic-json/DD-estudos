using Microsoft.EntityFrameworkCore;
using Modelo.Discente;
using rebuild.Data;
using rebuild.Data.DAL.Discente;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace rebuild.Data.DAL.Discente
    {
        public class AcademicoDAL
        {
            private readonly IESContext _context;
            public AcademicoDAL(IESContext context) => _context = context;

            // Consulta de listagem (somente leitura)
            public IQueryable<Academico> ObterAcademicosClassificadosPorNome()
                => _context.Academicos
                           .AsNoTracking()
                           .OrderBy(b => b.Nome);

            // Obter por Id (somente leitura)
            public async Task<Academico?> ObterAcademicoPorIdAsync(long id)
                => await _context.Academicos
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(a => a.AcademicoID == id);

            // Gravar (Add/Update) — considera PK long com identidade (0 = novo)
            public async Task<Academico> GravarAcademicoAsync(Academico academico, CancellationToken ct = default)
            {
            ArgumentNullException.ThrowIfNull(academico);   

            if (academico.AcademicoID == 0)
                await _context.Academicos.AddAsync(academico, ct);
            else
                _context.Academicos.Update(academico);

            await _context.SaveChangesAsync(ct);
            return academico;
            }

        // Remover por Id — retorna o removido ou null se não encontrado
        public async Task<Academico?> EliminarAcademicoPorIdAsync(long id)
            {
                var entity = await _context.Academicos.FindAsync(id); // aqui queremos tracking
                if (entity is null) return null;

                _context.Academicos.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
        }
    }
