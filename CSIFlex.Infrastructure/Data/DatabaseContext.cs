using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CSIFlex.Infrastructure.Data;

/// <summary>
/// Contexto de banco de dados para gerenciar conexões MySQL
/// </summary>
public class DatabaseContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Cria uma nova conexão com o banco de dados
    /// </summary>
    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json");
        }

        return new MySqlConnection(connectionString);
    }

    /// <summary>
    /// Cria uma nova conexão assíncrona com o banco de dados
    /// </summary>
    public async Task<MySqlConnection> CreateConnectionAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json");
        }

        var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
