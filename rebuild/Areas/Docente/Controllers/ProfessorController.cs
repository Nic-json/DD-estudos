
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
using rebuild.Data.DAL.Discente;
using rebuild.Data.DAL.Docente;
using rebuild.Migrations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace Capitulo05.Areas.Docente.Controllers
{
    [Area("Docente")]
    public class ProfessorController : Controller
    {
        private readonly IESContext _context;
        private readonly InstituicaoDAL instituicaoDAL;
        private readonly DepartamentoDAL departamentoDAL;
        private readonly CursoDAL cursoDAL;
        private readonly ProfessorDAL professorDAL;
        public ProfessorController(IESContext context)
        {
            _context = context;
            instituicaoDAL = new InstituicaoDAL(context);
            departamentoDAL = new DepartamentoDAL(context);
            cursoDAL = new CursoDAL(context);
            professorDAL = new ProfessorDAL(context);
        }
        public async Task<IActionResult> Index()
        {
            return View(await professorDAL.ObterProfessorClassificadosPorNome().ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,ProfessorID,CursosProfessores")] Professor professor)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    await professorDAL.GravarProfessorAsync(professor)
;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não	foi	possível	inserir   os  dados.");

            }
            return View(professor);
        }
        public void PrepararViewBags(List<Instituicao> instituicoes, List
        <Departamento> departamentos, List<Curso> cursos, List<Professor> professores)
        {
            instituicoes.Insert(0, new Instituicao()
            {
                InstituicaoID = 0,
                Nome = "Selecione	a	instituição"
            });
            ViewBag.Instituicoes = instituicoes;
            departamentos.Insert(0, new Departamento()
            {
                DepartamentoID =
0,
                Nome = "Selecione	o	departamento"
            });
            ViewBag.Departamentos = departamentos;
            cursos.Insert(0, new Curso()
            {
                CursoID = 0,
                Nome = "Selecione o   curso"
            });

            ViewBag.Cursos = cursos;
            professores.Insert(0, new Professor()
            {
                ProfessorID = 0,
                Nome = "Selecione	o	professor"
            });
            ViewBag.Professores = professores;
        }


        [HttpGet]
        public IActionResult AdicionarProfessor()
        {
            PrepararViewBags(instituicaoDAL.ObterInstituicoesClassificadasPorNome().ToList(),
            new List<Departamento>().ToList(), new List<Curso>().ToList(), new List<Professor>().ToList());
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarProfessor([Bind("InstituicaoID,	DepartamentoID,    CursoID,    ProfessorID")] AdicionarProfessorViewModel model)
        {
            if (model.InstituicaoID == 0 || model.DepartamentoID == 0 || model.CursoID == 0 || model.ProfessorID == 0)
            {
                ModelState.AddModelError("", "É	preciso	selecionar	todos	os  dados");

            }
            else
            {
                cursoDAL.RegistrarProfessor((long)model.CursoID, (long)model.ProfessorID);
                PrepararViewBags(instituicaoDAL.ObterInstituicoesClassificadasPorNome().ToList(),
                                departamentoDAL.ObterDepartamentosPorInstituicao((long)model.InstituicaoID).ToList(),
                                cursoDAL.ObterCursosPorDepartamento((long)model.DepartamentoID).ToList(),
                                cursoDAL.ObterProfessoresForaDoCurso((long)model.CursoID).ToList());
                RegistrarProfessorNaSessao((long)model.CursoID,(long)model.ProfessorID);
            }
            return View(model);
        }


        public JsonResult ObterDepartamentosPorInstituicao(long actionID)
        {
            var departamentos = departamentoDAL.ObterDepartamentosPorInstituicao(actionID).ToList();
            return Json(new SelectList(departamentos, "DepartamentoID", "Nome"));
        }
        public JsonResult ObterCursosPorDepartamento(long actionID)
        {
            var cursos = cursoDAL.ObterCursosPorDepartamento(actionID).ToList();
            return Json(new SelectList(cursos, "CursoID", "Nome"));
        }
        public JsonResult ObterProfessoresForaDoCurso(long actionID)
        {
            var professores = cursoDAL.ObterProfessoresForaDoCurso(actionID).ToList();
            return Json(new SelectList(professores, "ProfessorID", "Nome"));
        }
        public void RegistrarProfessorNaSessao(long cursoID, long professorID)
        {
            var cursoProfessor = new CursoProfessor()
            {
                ProfessorID = professorID,
                CursoID = cursoID
            };
            List<CursoProfessor> cursosProfessor = new List<CursoProfessor>();
            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");
            if (cursosProfessoresSession != null)
            {
                cursosProfessor = JsonConvert.DeserializeObject < List < CursoProfessor >> (cursosProfessoresSession);
            }
            cursosProfessor.Add(cursoProfessor);
            HttpContext.Session.SetString("cursosProfessores", JsonConvert.SerializeObject(cursosProfessor));
        }

        public IActionResult VerificarUltimosRegistros()
        {
            List<CursoProfessor> cursosProfessor = new List<CursoProfessor>();
            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");
            if (cursosProfessoresSession != null)
            {
                cursosProfessor = JsonConvert.DeserializeObject < List < CursoProfessor >> (cursosProfessoresSession);
            }
            return View(cursosProfessor);
        }


    }
}
