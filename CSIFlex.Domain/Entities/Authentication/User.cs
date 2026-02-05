namespace CSIFlex.Domain.Entities.Authentication;

/// <summary>
/// Entidade de domínio que representa um usuário do sistema CSIFLEX
/// </summary>
public class User
{
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string DisplayName { get; private set; }
    public string Email { get; private set; }
    public string UserType { get; private set; }
    public string RefId { get; private set; }
    public string Title { get; private set; }
    public string Dept { get; private set; }
    public string Machines { get; private set; }
    public string PhoneExt { get; private set; }
    public bool EditTimeline { get; private set; }
    public bool EditPartNumber { get; private set; }

    /// <summary>
    /// Verifica se o usuário é administrador
    /// </summary>
    public bool IsAdmin => UserType?.ToLower() == "admin";

    /// <summary>
    /// Construtor privado para garantir que a entidade seja criada apenas através de métodos factory
    /// </summary>
    private User() { }

    /// <summary>
    /// Factory method para criar um novo usuário
    /// </summary>
    public static User Create(
        string userName,
        string passwordHash,
        string salt,
        string firstName,
        string lastName,
        string displayName,
        string email,
        string userType,
        string refId = "",
        string title = "",
        string dept = "",
        string machines = "",
        string phoneExt = "",
        bool editTimeline = false,
        bool editPartNumber = false)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Nome de usuário não pode ser vazio", nameof(userName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash da senha não pode ser vazio", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt não pode ser vazio", nameof(salt));

        return new User
        {
            UserName = userName,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            DisplayName = displayName ?? string.Empty,
            Email = email ?? string.Empty,
            UserType = userType ?? "user",
            RefId = refId ?? string.Empty,
            Title = title ?? string.Empty,
            Dept = dept ?? string.Empty,
            Machines = machines ?? string.Empty,
            PhoneExt = phoneExt ?? string.Empty,
            EditTimeline = editTimeline,
            EditPartNumber = editPartNumber
        };
    }

    /// <summary>
    /// Factory method para reconstruir um usuário a partir do banco de dados
    /// </summary>
    public static User Reconstruct(
        string userName,
        string passwordHash,
        string salt,
        string firstName,
        string lastName,
        string displayName,
        string email,
        string userType,
        string refId,
        string title,
        string dept,
        string machines,
        string phoneExt,
        bool editTimeline,
        bool editPartNumber)
    {
        return new User
        {
            UserName = userName,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = firstName,
            LastName = lastName,
            DisplayName = displayName,
            Email = email,
            UserType = userType,
            RefId = refId,
            Title = title,
            Dept = dept,
            Machines = machines,
            PhoneExt = phoneExt,
            EditTimeline = editTimeline,
            EditPartNumber = editPartNumber
        };
    }
}
