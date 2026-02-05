using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace CSIFlex.Infrastructure.Data;

/// <summary>
/// Contexto de banco de dados para gerenciar conexões MySQL
/// </summary>
public class DatabaseContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseContext> _logger;

    public DatabaseContext(IConfiguration configuration, ILogger<DatabaseContext> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogDebug("DatabaseContext inicializado");
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
        _logger.LogDebug("Criando conexão assíncrona com o banco de dados");
        
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogError("Connection string 'DefaultConnection' não encontrada no appsettings.json");
            throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json");
        }

        try
        {
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            _logger.LogDebug("Conexão com o banco de dados estabelecida com sucesso");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao conectar ao banco de dados");
            throw;
        }
    }
}
