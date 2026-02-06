using System.ComponentModel.DataAnnotations;

namespace CSIFlex.Application.DTOs;

public class ChangePasswordDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter no mínimo 6 caracteres")]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
