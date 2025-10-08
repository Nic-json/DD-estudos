using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Migrations;
using rebuild.Models;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace rebuild.Controllers
{

    public class DepartamentoController : Controller
    {

        private readonly IESContext _context;
        public DepartamentoController(IESContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departamento.Include(i=>i.Instituicao).OrderBy(c => c.Nome).ToListAsync());

        }

        public IActionResult Create()
        {
            var Instituicao = _context.Instituicao.OrderBy(i => i.Nome).ToList();
            Instituicao.Insert(0, new Instituicao()
            {
                InstituicaoID = 0,
                Nome = "Selecione	a	instituição"
            });
            ViewBag.Instituicao = Instituicao;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome, InstituicaoID")] Departamento Departamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(Departamento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não	foi	possível	inserir	os  dados.");

                }
            return View(Departamento);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            if (Departamento == null)
            {
                return NotFound();
            }
            ViewBag.Instituicao = new SelectList(_context.Instituicao.OrderBy(b => b.Nome), "InstituicaoID", "Nome", Departamento.InstituicaoID);

            return View(Departamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("DepartamentoID,Nome,InstituicaoID")]	Departamento	Departamento)
        {
            if (id != Departamento.DepartamentoID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Departamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(Departamento.DepartamentoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(Departamento);
        }
        private bool DepartamentoExists(long? id)
        {
            return _context.Departamento.Any(e => e.DepartamentoID == id);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Instituicao.Where(i => Departamento.InstituicaoID == i.InstituicaoID).Load();
            if (Departamento == null)
            {
                return NotFound();
            }
            return View(Departamento);
        }
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Instituicao.Where(i => Departamento.InstituicaoID == i.InstituicaoID).Load();
            if (Departamento == null)
            {
                return NotFound();
            }
            return View(Departamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var Departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.DepartamentoID == id);
            _context.Departamento.Remove(Departamento);
            TempData["Message"] = "Departamento" + Departamento.Nome.ToUpper() + "	foi	removido";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




    }
}

