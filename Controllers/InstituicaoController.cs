using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rebuild.Data;
using rebuild.Models;
using System;
using System.Collections.Generic;

namespace rebuild.Controllers
{
    public class InstituicaoController : Controller
        {
            private readonly IESContext _context;
            public InstituicaoController (IESContext context)
            {
                this._context = context;
            }
            public async Task<IActionResult> Index()
            {
                return View(await _context.Instituicao.OrderBy(c => c.Nome).ToListAsync());
            }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome", "Endereco")] Instituicao instituicao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(instituicao);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não	foi	possível	inserir	os  dados.");

            }
            return View(instituicao);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instituicao = await _context.Instituicao.SingleOrDefaultAsync(m => m.InstituicaoID == id);
            if (instituicao == null)
            {
                return NotFound();
            }
            return View(instituicao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("InstituicaoID, Nome, Endereco")] Instituicao instituicao)
        {
            if (id != instituicao.InstituicaoID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instituicao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstituicaoExists(instituicao.InstituicaoID))
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
            return View(instituicao);
        }
        private bool InstituicaoExists(long? id)
        {
            return _context.Instituicao.Any(e => e.InstituicaoID == id);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instituicao = await _context.Instituicao.SingleOrDefaultAsync(m => m.InstituicaoID == id);
            if (instituicao == null)
            {
                return NotFound();
            }
            return View(instituicao);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instituicao = await _context.Instituicao.SingleOrDefaultAsync(m => m.InstituicaoID == id);
            if (instituicao == null)
            {
                return NotFound();
            }
            return View(instituicao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var Instituicao = await _context.Instituicao.SingleOrDefaultAsync(m => m.InstituicaoID == id);
            _context.Instituicao.Remove(Instituicao);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Instituição	" + Instituicao.Nome.ToUpper() + "	foi	removida";
            return RedirectToAction(nameof(Index));
        }
    }
}