using GalloFlix.DataTransferObjects;
using GalloFlix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GalloFlix.Controllers;

[Authorize(Roles = "Administrador")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(ILogger<AccountController> logger,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        LoginDto loginDto = new();
        loginDto.ReturnUrl = returnUrl ?? Url.Content("~/");
        return View(loginDto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto login)
    {
        //se o model é válido, faz login
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                login.Email, login.Password, login.RememberMe, true
            );
            if (result.Succeeded)
            {
                _logger.LogInformation($"Usuário { login.Email } acessou o sistema");
                return LocalRedirect(login.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning($"Usuário { login.Email } está bloqueado");
                return RedirectToAction("Lockout");
            }
            ModelState.AddModelError("login", "Usuário e/ou Senha Inválidos!!!");
        }
        return View(login);
    }
}
