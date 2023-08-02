using System.ComponentModel.DataAnnotations;

namespace GalloFlix.DataTransferObjects;

public class ForgetDto
{
    [Required(ErrorMessage = "Por favor, informe seu Email")]
    [EmailAddress(ErrorMessage = "Por favor, informe um Email Válido!")]
    [StringLength(100, ErrorMessage = "O Email deve possuir no máximo 100 caracteres")]
    [Display(Name = "Informe seu Email de Cadastro que enviaremos as instruções para recuperar sua senha")]
    public string Email { get; set; }
}