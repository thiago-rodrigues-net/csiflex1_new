using System.ComponentModel.DataAnnotations;

namespace CSIFlex.Application.DTOs;

/// <summary>
/// DTO para requisição de login
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [MinLength(3, ErrorMessage = "Nome de usuário deve ter no mínimo 3 caracteres")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(4, ErrorMessage = "Senha deve ter no mínimo 4 caracteres")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
