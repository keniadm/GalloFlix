using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Regras de validação

namespace GalloFlix.Models;

[Table("Genre")]
public class Genre
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //Coloca automático a chave primária
    public byte Id { get; set; }
    
    [Display(Name = "Nome")]
    //Display é o que aparece na tela para o usuário e não está ligado ao banco

    [Required(ErrorMessage = "O nome do Gênero é obrigatório")]
    //Required é usado quando algo é obrigado

    [StringLength(30, ErrorMessage = "O Nome deve possuir no máximo 30 caracteres")]
    //StringLength é o máximo de caracteres
    public string Name { get; set; }    
}