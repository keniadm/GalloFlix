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

    [Display(Name = "Título")]
    [Required(ErrorMessage = "O Título é obrigatório")]
    [StringLength(100, ErrorMessage = "O Título deve possuir no máximo 100 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string Title { get; set; }

    [Display(Name = "Título Original")]
    [Required(ErrorMessage = "O Título Original é obrigatório")]
    [StringLength(100, ErrorMessage = "O Título Original deve possuir no máximo 100 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string OriginalTitle { get; set; }

    [Display(Name = "Sinopse")]
    [Required(ErrorMessage = "A Sinopse é obrigatória")]
    [StringLength(8000, ErrorMessage = "A Sinopse deve possuir no máximo 5000 caracteres")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco
    public string Synopsis { get; set; }

    [Column(TypeName = "Year")]
    [Display(Name = "Ano de Estreia")]
    [Required(ErrorMessage = "O Ano de Estreia é obrigatório")]
    public Int16 MovieYear { get; set; }

    [Display(Name = "Duração (em minutos)")]
    [Required(ErrorMessage = "A Duração é obrigatória")]
    public Int16 Duration { get; set; }

    [Display(Name = "Classificação Etária")]
    [Required(ErrorMessage = "A Classificação Etária é obrigatória")]
    public byte AgeRating { get; set; }

    [StringLength(200)]
    [Display(Name = "Foto")]
    public string Image { get; set; }

    //não existe no banco de dados
    [NotMapped]
    [Display(Name = "Duração")]
    public string HourDuration { get {
        return TimeSpan.FromMinutes(Duration) .ToString(@"%h'h 'mm'min'");
    }}

    public ICollection<MovieComment> Comments { get; set; }
    public ICollection<MovieGenre> Genres { get; set; }
    public ICollection<MovieRating> Ratings { get; set; }

}