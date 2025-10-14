using System.ComponentModel.DataAnnotations;

namespace rebuild.Models.Infra { 
public class AcessarViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "Senha inválida.")]
    public string Senha { get; set; }

    [Display(Name = "Lembrar de mim?")]
    public bool LembrarDeMim { get; set; }
    }
}