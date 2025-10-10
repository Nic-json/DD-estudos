using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Modelo.Cadastros;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rebuild.Data.DAL.Cadastros
{
    public class InstituicaoDAL
    {
        private readonly IESContext _context;
        public InstituicaoDAL(IESContext context) => _context = context;

        // ---------------------------
        // GETs (leitura)
        // ---------------------------

        /// <summary>Consulta base para o Index, ordenada por Nome.</summary>
        public IQueryable<Instituicao> QueryOrdenadaPorNome()
            => _context.Instituicao.AsNoTracking().OrderBy(i => i.Nome);

        /// <summary>Convenience: mesma consulta com nome semântico.</summary>
        public IQueryable<Instituicao> ObterInstituicoesClassificadasPorNome()
            => QueryOrdenadaPorNome();

        /// <summary>Lista materializada para o Index.</summary>
        public Task<List<Instituicao>> ListarAsync(CancellationToken ct = default)
            => QueryOrdenadaPorNome().ToListAsync(ct);

        /// <summary>Busca por ID (detalhes/edição). Defina incluirDepartamentos = true para carregar filhos.</summary>
        public Task<Instituicao?> ObterPorIdAsync(long id, bool incluirDepartamentos = false, CancellationToken ct = default)
        {
            IQueryable<Instituicao> q = _context.Instituicao.AsNoTracking();

            if (incluirDepartamentos)
                q = q.Include(i => i.Departamento); // 🔁 AJUSTE aqui se a navegação tiver outro nome

            return q.SingleOrDefaultAsync(i => i.InstituicaoID == id, ct);
        }

        /// <summary>Itens para dropdown (SelectList) em outras telas.</summary>
        public Task<List<SelectListItem>> ObterSelectListAsync(CancellationToken ct = default)
            => _context.Instituicao.AsNoTracking()
                .OrderBy(i => i.Nome)
                .Select(i => new SelectListItem { Value = i.InstituicaoID.ToString(), Text = i.Nome })
                .ToListAsync(ct);

        /// <summary>Existe instituição com esse ID?</summary>
        public Task<bool> ExisteAsync(long id, CancellationToken ct = default)
            => _context.Instituicao.AnyAsync(i => i.InstituicaoID == id, ct);

        /// <summary>Tem departamentos vinculados? (útil para bloquear exclusão)</summary>
        public Task<bool> PossuiDepartamentosAsync(long id, CancellationToken ct = default)
            => _context.Departamento.AnyAsync(d => d.InstituicaoID == id, ct);

        // ---------------------------
        // Escrita (Create/Update/Delete)
        // ---------------------------

        public async Task<Instituicao> SalvarAsync(Instituicao instituicao, CancellationToken ct = default)
        {
            if (instituicao.InstituicaoID == 0)
                await _context.Instituicao.AddAsync(instituicao, ct);
            else
                _context.Instituicao.Update(instituicao);

            await _context.SaveChangesAsync(ct);
            return instituicao;
        }

        public async Task<bool> ExcluirPorIdAsync(long id, CancellationToken ct = default)
        {
            var existente = await _context.Instituicao.FindAsync(new object[] { id }, ct);
            if (existente is null) return false;

            _context.Instituicao.Remove(existente);

            try
            {
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                // Provável violação de FK (departamentos vinculados).
                // Deixe propagar se quiser tratar no Controller, ou retorne false:
                // return false;
                throw;
            }
        }
    }
}