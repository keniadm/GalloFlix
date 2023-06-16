using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using GalloFlix.DataTransferObjects;
using GalloFlix.Models;
using GalloFlix.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GalloFlix.Controllers;

[Authorize(Roles = "Administrador")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;
    private readonly IEmailSender _emailSender;

    public AccountController(ILogger<AccountController> logger,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore,
        IEmailSender emailSender
    )
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<AppUser>)_userStore;
        _emailSender = emailSender;
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
            string userName = login.Email;
            if (IsValidEmail(login.Email))
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null)
                    userName = user.UserName;
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName, login.Password, login.RememberMe, true
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

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation($"Usuário {ClaimTypes.Email} fez logoff");
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto register)
    {
        if (ModelState.IsValid) 
        {
            var user = Activator.CreateInstance<AppUser>();

            user.Name = register.Name;
            user.DateOfBirth = register.DateOfBirth;
            user.Email = register.Email;

            await _userStore.SetUserNameAsync(
                user, register.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(
                user, register.Email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, register.Password);

            if(result.Succeeded)
            {
                _logger.LogInformation($"Novo usuário registrado com o email {user.Email}");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ConfirmEmail", "Account", new { userId = userId, code = code},
                    protocol: Request.Scheme
                );

                await _userManager.AddToRoleAsync(user, "Usuário");

                await _emailSender.SendEmailAsync(
                    email: register.Email,
                    subject: "GalloFlix - Criação de Conta",
                    htmlMessage: $"Por favor, confirme a criação de sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Clicando Aqui</a>"
                );

                return RedirectToAction("RegisterConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(register);
    }

    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }
    
    private bool IsValidEmail(string email) 
    {
        try
        {
            MailAddress m = new(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

}
