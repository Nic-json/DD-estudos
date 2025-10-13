using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using rebuild.Data.DAL.Cadastros;
using System.Threading.Tasks;

[Area("Cadastros")]
[Authorize]

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
    [HttpGet]
    public async Task<IActionResult> Delete(long? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var model = await _dal.ObterPorIdAsync(id.Value, ct: ct);
        return model is null ? NotFound() : View(model);
    }
    // POST: Instituicoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id, CancellationToken ct)
    {
        // tem dependentes?
        if (await _dal.PossuiDepartamentosAsync(id, ct))
        {
            TempData["Message"] = "Não foi possível excluir: a instituição possui departamentos vinculados.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        var ok = await _dal.ExcluirPorIdAsync(id, ct);
        if (!ok)
        {
            TempData["Message"] = "Não foi possível excluir: existem registros dependentes.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Message"] = "Instituição removida com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}