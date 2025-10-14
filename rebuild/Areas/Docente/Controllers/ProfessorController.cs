using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Docente;
using Newtonsoft.Json;
using rebuild.Areas.Docente.Models;
using rebuild.Data;
using rebuild.Data.DAL.Cadastros;
using rebuild.Data.DAL.Docente;

namespace rebuild.Areas.Docente.Controllers
{
    [Area("Docente")]
    [Authorize]
    public class ProfessorController : Controller
    {
        private readonly IESContext _context;
        private readonly InstituicaoDAL _instituicaoDAL;
        private readonly DepartamentoDAL _departamentoDAL;
        private readonly CursoDAL _cursoDAL;
        private readonly ProfessorDAL _professorDAL;

        public ProfessorController(IESContext context)
        {
            _context = context;
            _instituicaoDAL = new InstituicaoDAL(context);
            _departamentoDAL = new DepartamentoDAL(context);
            _cursoDAL = new CursoDAL(context);
            _professorDAL = new ProfessorDAL(context);
        }

        // LISTA DE PROFESSORES
        public async Task<IActionResult> Index()
            => View(await _professorDAL.ObterProfessorClassificadosPorNome().ToListAsync());

        // CADASTRO SIMPLES DE PROFESSOR (opcional)
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome")] Professor professor)
        {
            if (!ModelState.IsValid) return View(professor);

            await _professorDAL.GravarProfessorAsync(professor);
            TempData["Message"] = "Professor cadastrado.";
            return RedirectToAction(nameof(Index));
        }

        // ----- REGISTRAR PROFESSOR EM CURSO -----

        [HttpGet]
        public async Task<IActionResult> AdicionarProfessor()
        {
            await PreencherViewBagsAsync(null, null, null);
            return View(new AdicionarProfessorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarProfessor(AdicionarProfessorViewModel model)
        {
            if (model.InstituicaoID is null || model.DepartamentoID is null ||
                model.CursoID is null || model.ProfessorID is null)
            {
                ModelState.AddModelError("", "É preciso selecionar todos os dados.");
            }

            if (!ModelState.IsValid)
            {
                await PreencherViewBagsAsync(model.InstituicaoID, model.DepartamentoID, model.CursoID);
                return View(model);
            }

            // efetiva o vínculo (seu DAL é void, sem await)
            _cursoDAL.RegistrarProfessor((long)model.CursoID, (long)model.ProfessorID);

            // opcional: salva no Session para "últimos registros"
            RegistrarProfessorNaSessao((long)model.CursoID, (long)model.ProfessorID);

            TempData["Message"] = "Professor registrado no curso.";
            return RedirectToAction(nameof(AdicionarProfessor));
        }

        // ----- JSON: combos encadeados -----
        [HttpGet]
        public async Task<JsonResult> ObterDepartamentosPorInstituicao(long actionID)
        {
            var lista = await _departamentoDAL.ObterDepartamentosPorInstituicao(actionID)
                                              .Select(d => new { value = d.DepartamentoID, text = d.Nome })
                                              .ToListAsync();
            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> ObterCursosPorDepartamento(long actionID)
        {
            var lista = await _cursoDAL.ObterCursosPorDepartamento(actionID)
                                       .Select(c => new { value = c.CursoID, text = c.Nome })
                                       .ToListAsync();
            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> ObterProfessoresForaDoCurso(long actionID)
        {
            var lista = await _cursoDAL.ObterProfessoresForaDoCurso(actionID)
                                       .Select(p => new { value = p.ProfessorID, text = p.Nome })
                                       .ToListAsync();
            return Json(lista);
        }

        // ----- Utilitários -----
        private async Task PreencherViewBagsAsync(long? instituicaoId, long? departamentoId, long? cursoId)
        {
            // Instituições
            ViewBag.Instituicoes = await _instituicaoDAL
                .ObterInstituicoesClassificadasPorNome()
                .ToListAsync();

            // Departamentos (dependem de Instituição)
            if (instituicaoId.GetValueOrDefault() > 0)
                ViewBag.Departamentos = await _departamentoDAL
                    .ObterDepartamentosPorInstituicao(instituicaoId!.Value)
                    .ToListAsync();
            else
                ViewBag.Departamentos = new List<Departamento>();

            // Cursos (dependem de Departamento)  <<< CORRIGIDO
            if (departamentoId.GetValueOrDefault() > 0)
                ViewBag.Cursos = await _cursoDAL
                    .ObterCursosPorDepartamento(departamentoId!.Value)
                    .ToListAsync();
            else
                ViewBag.Cursos = new List<Curso>();

            // Professores fora do curso (dependem de Curso)  <<< CORRIGIDO (vem do CursoDAL)
            if (cursoId.GetValueOrDefault() > 0)
                ViewBag.Professores = await _cursoDAL
                    .ObterProfessoresForaDoCurso(cursoId!.Value)
                    .ToListAsync();
            else
                ViewBag.Professores = new List<Professor>();
        }

        private void RegistrarProfessorNaSessao(long cursoID, long professorID)
        {
            var cursoProfessor = new CursoProfessor { CursoID = cursoID, ProfessorID = professorID };

            var json = HttpContext.Session.GetString("cursosProfessores");
            var lista = string.IsNullOrEmpty(json)
                ? new List<CursoProfessor>()
                : JsonConvert.DeserializeObject<List<CursoProfessor>>(json)!;

            lista.Add(cursoProfessor);
            HttpContext.Session.SetString("cursosProfessores", JsonConvert.SerializeObject(lista));
        }

        public IActionResult VerificarUltimosRegistros()
        {
            var json = HttpContext.Session.GetString("cursosProfessores");
            var lista = string.IsNullOrEmpty(json)
                ? new List<CursoProfessor>()
                : JsonConvert.DeserializeObject<List<CursoProfessor>>(json)!;

            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var prof = await _context.Professores
                .AsNoTracking()
                .Include(p => p.CursosProfessores)
                    .ThenInclude(cp => cp.Curso)
                .FirstOrDefaultAsync(p => p.ProfessorID == id);

            if (prof is null) return NotFound();
            return View(prof);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var professor = await _professorDAL.ObterProfessorPorIdAsync(id);
            if (professor is null) return NotFound();

            return View(professor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ProfessorID,Nome")] Professor professor)
        {
            if (id != professor.ProfessorID) return NotFound();

            if (!ModelState.IsValid)
                return View(professor);

            try
            {
                await _professorDAL.GravarProfessorAsync(professor); // PK != 0 => Update
                TempData["Message"] = "Professor atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // se alguém removeu enquanto você editava
                var existente = await _professorDAL.ObterProfessorPorIdAsync(id);
                if (existente is null) return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações.");
                return View(professor);
            }
        }
    }
}