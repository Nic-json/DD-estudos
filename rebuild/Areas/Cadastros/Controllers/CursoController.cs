using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;

namespace rebuild.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class CursoController : Controller
    {
        private readonly IESContext _ctx;
        private readonly DepartamentoDAL _depDAL;
        private readonly CursoDAL _curDAL;

        public CursoController(IESContext ctx, CursoDAL curDAL)
        {
            _ctx = ctx;
            _depDAL = new DepartamentoDAL(ctx);
            _curDAL = curDAL ?? throw new ArgumentNullException(nameof(curDAL));
        }

        // LISTAGEM
        public async Task<IActionResult> Index()
        {
            var cursos = await _ctx.Cursos
                                  .Include(c => c.Departamento)
                                  .OrderBy(c => c.Nome)
                                  .ToListAsync();
            return View(cursos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarDepartamentosAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DepartamentoID")] Curso? curso)
        {
            // Linha defensiva: se o model binding falhar, não deixa estourar NRE
            if (curso is null)
            {
                ModelState.AddModelError(string.Empty, "Dados inválidos.");
                await CarregarDepartamentosAsync();
                return View(new Curso());
            }

            if (!ModelState.IsValid)
            {
                await CarregarDepartamentosAsync(); // SEMPRE repopula ao voltar
                return View(curso);
            }

            // >>> Linha que estourava (agora protegida pois _curDAL foi injetado)
            await _curDAL.GravarCursoAsync(curso);

            TempData["Message"] = "Curso salvo com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarDepartamentosAsync(long? selecionado = null)
        {
            ViewBag.Departamentos = await _ctx.Departamento
            .OrderBy(d => d.Nome)
            .Select(d => new SelectListItem
            {
                Value = d.DepartamentoID.ToString(),
                Text = d.Nome
            }).ToListAsync();
        }


    }
}

