using System.ComponentModel.DataAnnotations;

namespace GalloFlix.DataTransferObjects;

public class ResetPasswordDto
{
    public string Email { get; private set; }

    [DataType(DataType.Password)]
    [Display(Name = "Senha de Acesso")]
    [Required(ErrorMessage = "Por favor, informe sua Senha de Acesso")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "A Senha deve possuir no minimo 6 e no máximo 20 caracteres")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Senha de Acesso")]
    [Compare("Password", ErrorMessage = "As Senhas não Conferem.")]
    public string ConfirmPassword { get; set; }

    public string Code { get; private set; }

    public ResetPasswordDto(string email, string code)
    {
      Email = email;
      Code = code;
      Password = string.Empty;
      ConfirmPassword = string.Empty;
    }
}