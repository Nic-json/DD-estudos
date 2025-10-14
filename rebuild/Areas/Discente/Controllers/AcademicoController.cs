
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Modelo.Discente;
using rebuild.Data;
using rebuild.Data.DAL.Discente;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace rebuild.Areas.Discente.Controllers
{
    [Area("Discente")]
    [Authorize]
    public class AcademicoController : Controller
    {
        private readonly IESContext _context;
        private IWebHostEnvironment _env;
        private readonly AcademicoDAL _dal;

        public AcademicoController(IESContext context, IWebHostEnvironment env)
        {
            _context = context;
            _dal = new AcademicoDAL(context);
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dal.ObterAcademicosClassificadosPorNome().ToListAsync());
        }
        private async Task<IActionResult> ObterVisaoAcademicoPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var academico = await _dal.ObterAcademicoPorIdAsync((long)id);
            if (academico == null)
            {
                return NotFound();
            }
            return View(academico);
        }
        public async Task<IActionResult> Details(long? id)
        {
            return await ObterVisaoAcademicoPorId(id);
        }
        public async Task<IActionResult> Edit(long? id)
        {
            return await ObterVisaoAcademicoPorId(id);
        }
        public async Task<IActionResult> Delete(long? id)
        {
            return await ObterVisaoAcademicoPorId(id);
        }
        //	GET:	Academico/Create
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,RegistroAcademico,Nascimento")] Academico academico, IFormFile? foto)

        {
            if (!ModelState.IsValid)
                return View(academico);

            if (foto is not null && foto.Length > 0)
            {
                // validação simples (opcional)
                if (!new[] { "image/jpeg", "image/png", "image/gif" }.Contains(foto.ContentType))
                    ModelState.AddModelError("foto", "Formato inválido. Envie JPG, PNG ou GIF.");
                if (foto.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError("foto", "Arquivo maior que 2MB.");

                if (!ModelState.IsValid) return View(academico);

                using var ms = new MemoryStream();
                await foto.CopyToAsync(ms);
                academico.Foto = ms.ToArray();
                academico.FotoMimeType = foto.ContentType;
            }
            try
            {
                await _dal.GravarAcademicoAsync(academico);
                TempData["Message"] = "Acadêmico registrado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível inserir os dados.");
                return View(academico);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,[Bind("AcademicoID,Nome,RegistroAcademico,Nascimento")] Academico academico,IFormFile? foto, string? chkRemoverFoto)
        {
            if (id != academico.AcademicoID) return NotFound();
            if (!ModelState.IsValid) return View(academico);

            // carrega do banco
            var atual = await _dal.ObterAcademicoPorIdAsync(id);
            if (atual is null) return NotFound();

            // atualiza campos básicos
            atual.Nome = academico.Nome;
            atual.RegistroAcademico = academico.RegistroAcademico;
            atual.Nascimento = academico.Nascimento;

            // foto: remover / trocar / manter
            if (!string.IsNullOrEmpty(chkRemoverFoto))
            {
                atual.Foto = null;
                atual.FotoMimeType = null;
            }
            else if (foto is not null && foto.Length > 0)
            {
                // validações simples
                if (foto.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError("foto", "Arquivo maior que 2MB.");
                var okContent = new[] { "image/jpeg", "image/png", "image/gif" }.Contains(foto.ContentType);
                if (!okContent)
                    ModelState.AddModelError("foto", "Formato inválido. Envie JPG, PNG ou GIF.");

                if (!ModelState.IsValid) return View(academico);

                using var ms = new MemoryStream();
                await foto.CopyToAsync(ms);
                atual.Foto = ms.ToArray();
                atual.FotoMimeType = foto.ContentType;
            }
            // senão: mantém a foto atual

            await _dal.GravarAcademicoAsync(atual);
            TempData["Message"] = "Acadêmico atualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id
)
        {
            var academico = await _dal.EliminarAcademicoPorIdAsync((long)id);
            TempData["Message"] = "Acadêmico	" + academico.Nome.ToUpper() + "	foi	removida";
            return RedirectToAction(nameof(Index));
        }
        private async Task<bool> AcademicoExists(long? id)
        {
            return await _dal.ObterAcademicoPorIdAsync((long)id) != null;
        }
        [HttpGet]
        public async Task<IActionResult> Foto(long id)
        {
            // chame o SEU método correto do DAL/Contexto
            var academico = await _dal.ObterAcademicoPorIdAsync(id);
            // ou, se usa DbContext direto:
            // var academico = await _ctx.Academicos
            //     .AsNoTracking()
            //     .Select(a => new { a.Foto, a.FotoMimeType })
            //     .FirstOrDefaultAsync(a => a.AcademicoID == id);

            if (academico is null)
                return NotFound();

            if (academico.Foto is null || string.IsNullOrEmpty(academico.FotoMimeType))
                return NotFound(); // ou retorne uma imagem padrão

            return File(academico.Foto, academico.FotoMimeType); // FileContentResult
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFoto(long id)
        {
            var a = await _dal.ObterAcademicoPorIdAsync(id);
            if (a is null || a.Foto is null || string.IsNullOrEmpty(a.FotoMimeType))
                return NotFound();

            var ext = a.FotoMimeType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                _ => "bin"
            };

            var fileName = $"Foto_{id}.{ext}";
            // devolve o conteúdo em memória (NÃO usa stream/arquivo físico)
            return File(a.Foto, a.FotoMimeType, fileName);
        }
    }
}

