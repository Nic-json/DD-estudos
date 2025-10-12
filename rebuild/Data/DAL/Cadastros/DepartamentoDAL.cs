using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using rebuild.Data;
using rebuild.Migrations;
using System.Linq;
using System.Threading.Tasks;
namespace rebuild.Data.DAL.Cadastros
{
    public class DepartamentoDAL
    {
        private IESContext _context;
        public DepartamentoDAL(IESContext context)
        {
            _context = context;
        }
        public IQueryable<Departamento> ObterDepartamentosClassificadosPorNome()
        {
            return _context.Departamento.Include(i => i.Instituicao).OrderBy(b => b.Nome);
        }
        public async Task<Departamento> ObterDepartamentoPorId(long id)
        {
            var departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Instituicao.Where(i => departamento.InstituicaoID == i.InstituicaoID).Load(); ;
            return departamento;
        }
        public async Task<Departamento> GravarDepartamento(Departamento departamento)
        {
            if (departamento.DepartamentoID == null)
            {
                _context.Departamento.Add(departamento);
            }
            else
            {
                _context.Update(departamento);
            }
            await _context.SaveChangesAsync();
            return departamento;
        }
        public async Task<Departamento> EliminarDepartamentoPorId(long id)
        {
            Departamento departamento = await ObterDepartamentoPorId(id);
            _context.Departamento.Remove(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }

        public IQueryable<Departamento> ObterDepartamentosPorInstituicao(long instituicaoID)
        {
            var departamentos = _context.Departamento.Where(d => d.InstituicaoID == instituicaoID).OrderBy(d => d.Nome);
            return departamentos;
        }

    }
}
