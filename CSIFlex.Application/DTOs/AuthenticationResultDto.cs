namespace CSIFlex.Application.DTOs;

/// <summary>
/// DTO para resultado de autenticação
/// </summary>
public class AuthenticationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}

/// <summary>
/// DTO de dados do usuário
/// </summary>
public class UserDto
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string Machines { get; set; } = string.Empty;
}
