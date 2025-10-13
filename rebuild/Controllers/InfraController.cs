using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using rebuild.Models.Infra;
using System.Threading.Tasks;
namespace rebuild.Controllers
{
    [Authorize]
    public class InfraController : Controller
    {
        private readonly UserManager<UsuarioDaAplicacao> _userManager;
        private readonly SignInManager<UsuarioDaAplicacao> _signInManager;
        private readonly ILogger _logger;
        public InfraController(
                        UserManager<UsuarioDaAplicacao> userManager,
                        SignInManager<UsuarioDaAplicacao> signInManager,
                        ILogger<InfraController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Acessar(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new AcessarViewModel()); // tela em branco
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acessar(AcessarViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Senha, model.LembrarDeMim, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário autenticado.");
                // Redireciona somente se for URL local
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Falha na tentativa de login.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrarNovoUsuario(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarNovoUsuario(RegistrarNovoUsuarioViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new UsuarioDaAplicacao
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário	criou	uma	nova	conta	com senha.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("Usuário	acessou	com	a	conta	criada.");

                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description)
;
            }
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sair()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuário	realizou	logout.");
            return RedirectToAction("Index", "Home");
        }

    }
}

