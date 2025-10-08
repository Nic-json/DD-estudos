    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using rebuild.Data.DAL.Cadastros;
using System.Threading.Tasks;


public class InstituicaoController : Controller
{
    private readonly InstituicaoDAL _dal;
    public InstituicaoController(InstituicaoDAL dal) => _dal = dal;

    // GET: Instituicoes
    public IActionResult Index()
    {
        var consulta = _dal.QueryOrdenadaPorNome(); // ou .ToList() se preferir materializar aqui
        return View(consulta);
    }

    // GET: Instituicoes/Details/5
    public async Task<IActionResult> Details(long id)
    {
        var inst = await _dal.ObterPorIdAsync(id, incluirDepartamentos: true);
        if (inst is null) return NotFound();
        return View(inst);
    }
    public IActionResult Create() => View();
    // POST: Instituicoes/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Instituicao instituicao)
    {
        if (!ModelState.IsValid) return View(instituicao);
        await _dal.SalvarAsync(instituicao);
        TempData["Message"] = "Instituição criada com sucesso.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(long id)
    {
        var inst = await _dal.ObterPorIdAsync(id);
        if (inst is null) return NotFound();
        return View(inst);
    }
    // POST: Instituicoes/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Instituicao instituicao)
    {
        if (id != instituicao.InstituicaoID) return BadRequest();
        if (!ModelState.IsValid) return View(instituicao);

        await _dal.SalvarAsync(instituicao);
        TempData["Message"] = "Instituição atualizada.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(long id)
    {
        var inst = await _dal.ObterPorIdAsync(id, incluirDepartamentos: true);
        if (inst is null) return NotFound();
        return View(inst);
    }
    // POST: Instituicoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            var ok = await _dal.ExcluirPorIdAsync(id);
            if (!ok) return NotFound();

            TempData["Message"] = "Instituição excluída.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            // provavelmente há Departamentos vinculados (FK)
            TempData["Message"] = "Não é possível excluir: existem departamentos vinculados.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}