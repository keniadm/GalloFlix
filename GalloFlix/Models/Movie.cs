using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Regras de validação

namespace GalloFlix.Models;

[Table("Movie")]
public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //Coloca automático a chave primária
    public int Id { get; set; }
    
    [Display(Name = "Título Original")]
    [Required(ErrorMessage = "O Título Original é obrigatório")]
    [StringLength(100, ErrorMessage = "O Título Original deve possuir no máximo 100 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string OriginalTitle { get; set; }

    [Display(Name = "Título")]
    [Required(ErrorMessage = "O Título é obrigatório")]
    [StringLength(100, ErrorMessage = "O Título deve possuir no máximo 100 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string Title { get; set; }

    [Display(Name = "Sinopse")]
    [StringLength(5000, ErrorMessage = "A Sinopse deve possuir no máximo 5000 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string Synopsis { get; set; }

    public int MovieYear { get; set; }

    public int Duration { get; set; }

    public int AgeRating { get; set; }

    public string Image { get; set; }
}