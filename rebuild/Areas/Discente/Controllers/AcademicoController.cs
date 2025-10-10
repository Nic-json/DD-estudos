
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Discente;
using rebuild.Data;
using rebuild.Data.DAL.Discente;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;


namespace rebuild.Areas.Discente.Controllers
{
    [Area("Discente")]
    public class AcademicoController : Controller
    {
        private readonly IESContext _context;
        private readonly AcademicoDAL academicoDAL;
        private IWebHostEnvironment _env;

        public AcademicoController(IESContext context, IWebHostEnvironment env)
        {
            _context = context;
            academicoDAL = new AcademicoDAL(context);
            _env = env; 
        }
        public async Task<IActionResult> Index()
        {
            return View(await academicoDAL.ObterAcademicosClassificadosPorNome().ToListAsync());
        }
        private async Task<IActionResult> ObterVisaoAcademicoPorId(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var academico = await academicoDAL.ObterAcademicoPorIdAsync((long)id);
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,RegistroAcademico,Nascimento")]	Academico	academico)

                                {
            try
            {
                if (ModelState.IsValid)
                {
                    await academicoDAL.GravarAcademicoAsync(academico)
;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não	foi	possível	inserir   os  dados.");

                                                }
            return View(academico);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("AcademicoID,Nome,RegistroAcademico,Nascimento")]	Academico academico,	IFormFile    foto, string? chkRemoverFoto)
        {
            if (id != academico.AcademicoID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var stream = new MemoryStream();
                    if (chkRemoverFoto != null)
                    {
                        academico.Foto = null;
                    }
                    else { 
                    await foto.CopyToAsync(stream);
                    academico.Foto = stream.ToArray();
                    academico.FotoMimeType = foto.ContentType;
                    }
                    await academicoDAL.GravarAcademicoAsync(academico);
                
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AcademicoExists(academico.AcademicoID))
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
            return View(academico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id
)
        {
            var academico = await academicoDAL.EliminarAcademicoPorIdAsync((long)id);
            TempData["Message"] = "Acadêmico	" + academico.Nome.ToUpper() + "	foi	removida";
            return RedirectToAction(nameof(Index));
        }
        private async Task<bool> AcademicoExists(long? id)
        {
            return await academicoDAL.ObterAcademicoPorIdAsync((long)id) != null;
        }
        [HttpGet]
        public async Task<IActionResult> Foto(long id)
        {
            // chame o SEU método correto do DAL/Contexto
            var academico = await academicoDAL.ObterAcademicoPorIdAsync(id);
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
            var a = await academicoDAL.ObterAcademicoPorIdAsync(id);
            if (a is null) return NotFound();
            if (a.Foto is null || string.IsNullOrEmpty(a.FotoMimeType)) return NotFound();

            var ext = a.FotoMimeType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                _ => "bin"
            };

            var fileName = $"Foto_{id}.{ext}";
            var folderPath = Path.Combine(_env.WebRootPath, "downloads");
            Directory.CreateDirectory(folderPath); // idempotente
            var fullPath = Path.Combine(folderPath, fileName);

            // grava o arquivo físico
            await System.IO.File.WriteAllBytesAsync(fullPath, a.Foto);

            // serve via provider
            IFileProvider provider = new PhysicalFileProvider(folderPath);
            IFileInfo fileInfo = provider.GetFileInfo(fileName);
            using var readStream = fileInfo.CreateReadStream(); // stream é descartado ao final da resposta

            return File(readStream, a.FotoMimeType, fileName);
        }
    }
}

