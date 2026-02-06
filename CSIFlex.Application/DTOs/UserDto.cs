using System.ComponentModel.DataAnnotations;

namespace CSIFlex.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [StringLength(50, ErrorMessage = "Nome de usuário deve ter no máximo 50 caracteres")]
    public string UserName { get; set; } = string.Empty;
    
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
    
    public bool IsActive { get; set; } = true;
    
    public bool IsAdmin => UserType?.Equals("admin", StringComparison.OrdinalIgnoreCase) ?? false;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
