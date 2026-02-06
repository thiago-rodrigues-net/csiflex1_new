using System.ComponentModel.DataAnnotations;

namespace CSIFlex.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Nome de usuário deve conter apenas letras, números e underscore")]
    public string UserName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Sobrenome é obrigatório")]
    [StringLength(100, ErrorMessage = "Sobrenome deve ter no máximo 100 caracteres")]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(150, ErrorMessage = "Nome de exibição deve ter no máximo 150 caracteres")]
    public string DisplayName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(150, ErrorMessage = "E-mail deve ter no máximo 150 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
    public string UserType { get; set; } = "user";
    
    [StringLength(100, ErrorMessage = "Departamento deve ter no máximo 100 caracteres")]
    public string Dept { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(20, ErrorMessage = "Ramal deve ter no máximo 20 caracteres")]
    public string PhoneExt { get; set; } = string.Empty;
    
    public string Machines { get; set; } = string.Empty;
    
    public bool EditTimeline { get; set; }
    
    public bool EditPartNumber { get; set; }
}
