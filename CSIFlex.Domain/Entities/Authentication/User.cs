namespace CSIFlex.Domain.Entities.Authentication;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = "user";
    public string RefId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Dept { get; set; } = string.Empty;
    public string Machines { get; set; } = string.Empty;
    public string PhoneExt { get; set; } = string.Empty;
    public bool EditTimeline { get; set; }
    public bool EditPartNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsAdmin => UserType?.Equals("admin", StringComparison.OrdinalIgnoreCase) ?? false;

    public User()
    {
    }

    public User(
        string userName,
        string firstName,
        string lastName,
        string displayName,
        string email,
        string userType,
        string dept = "",
        string title = "",
        string machines = "",
        string phoneExt = "",
        bool editTimeline = false,
        bool editPartNumber = false)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Nome de usuário não pode ser vazio", nameof(userName));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Nome não pode ser vazio", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Sobrenome não pode ser vazio", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail não pode ser vazio", nameof(email));

        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? $"{firstName} {lastName}" : displayName;
        Email = email;
        UserType = userType;
        Dept = dept;
        Title = title;
        Machines = machines;
        PhoneExt = phoneExt;
        EditTimeline = editTimeline;
        EditPartNumber = editPartNumber;
        RefId = Guid.NewGuid().ToString();
        CreatedAt = DateTime.Now;
        IsActive = true;
    }

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
