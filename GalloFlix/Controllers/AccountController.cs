using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using GalloFlix.DataTransferObjects;
using GalloFlix.Helpers;
using GalloFlix.Models;
using GalloFlix.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GalloFlix.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;
    private readonly IEmailSender _emailSender;

    public AccountController(
        ILogger<AccountController> logger,
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
        _emailStore = GetEmailStore();
        _emailSender = emailSender;
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        LoginDto login = new();
        login.ReturnUrl = returnUrl ?? Url.Content("~/");
        return View(login);
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginDto login)
    {
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
                userName, login.Password, login.RememberMe, lockoutOnFailure: true
            );
            if (result.Succeeded)
            {
                _logger.LogInformation($"Usuário {login.Email} acessou o sistema");
                return LocalRedirect(login.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning($"Usuário {login.Email} está bloqueado");
                return RedirectToAction("Lockout");
            }
            ModelState.AddModelError("login", "Usuário e/ou Senha Inválidos!!!");
        }
        return View(login);
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation($"Usuário {ClaimTypes.Email} fez logoff");
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto register)
    {
        if (ModelState.IsValid)
        {
            var user = Activator.CreateInstance<AppUser>();

            user.Name = register.Name;
            user.DateOfBirth = register.DateOfBirth;
            user.Email = register.Email;

            await _userStore.SetUserNameAsync(user, register.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, register.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Novo usuário registrado com o email {user.Email}.");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ConfirmEmail", "Account",
                    new { userId, code },
                    protocol: Request.Scheme);

                await _userManager.AddToRoleAsync(user, "Usuário");

                await _emailSender.SendEmailAsync(register.Email, "EtecBook - Criação de Conta",
                    $"Por favor, confirme a criação de sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                return RedirectToAction("RegisterConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TranslateIdentityErrors.TranslateErrorMessage(error.Code));
            }
        }
        return View(register);
    }


    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Não foi possível localizar o usuário.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded);
    }


    [HttpGet]
    public IActionResult Forget()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Forget(ForgetDto forget)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(forget.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Não revelar que o usuário não existe ou que não está confirmado
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                action: "ResetPassword",
                controller: "Account",
                values: new { email = forget.Email, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                email: forget.Email,
                subject: "Recuperar Senha",
                htmlMessage: $"Para definir uma nova senha <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clique aqui</a>.");

            return RedirectToAction("ForgotPasswordConfirmation");
        }
        return View();
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ResetPassword(string email, string code)
    {
        if (code == null || string.IsNullOrEmpty(email))
        {
            return BadRequest("Solicitação Inválida!!!");
        }
        ResetPasswordDto reset = new
        (
            email: email,
            code: Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        );
        return View(reset);
    }


    private IUserEmailStore<AppUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<AppUser>)_userStore;
    }


    private static bool IsValidEmail(string email)
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