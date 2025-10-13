using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;
using Modelo.Cadastros;

namespace rebuild.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    public class CursoController : Controller
    {
        private readonly IESContext _ctx;
        private readonly DepartamentoDAL _depDAL;

        public CursoController(IESContext ctx)
        {
            _ctx = ctx;
            _depDAL = new DepartamentoDAL(ctx);
        }

        // LISTAGEM
        public async Task<IActionResult> Index()
        {
            var lista = await _ctx.Cursos
                                  .Include(c => c.Departamento)
                                  .OrderBy(c => c.Nome)
                                  .ToListAsync();
            return View(lista);
        }

        // CREATE GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopularDepartamentosAsync();
            return View(new Curso());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DepartamentoID")] Curso curso)
        {
            if (!ModelState.IsValid)
            {
                await PopularDepartamentosAsync();
                return View(curso);
            }

            _ctx.Cursos.Add(curso);
            await _ctx.SaveChangesAsync();
            TempData["Message"] = "Curso cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopularDepartamentosAsync()
        {
            var deps = await _depDAL.ObterDepartamentosClassificadosPorNome().ToListAsync();
            ViewBag.Departamentos = new SelectList(deps, "DepartamentoID", "Nome");
        }
    }
}

