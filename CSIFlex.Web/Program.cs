using CSIFlex.Web.Components;
using CSIFlex.Web.Services;
using CSIFlex.Application.Services;
using CSIFlex.Domain.Interfaces.Repositories;
using CSIFlex.Domain.Interfaces.Services;
using CSIFlex.Infrastructure.Data;
using CSIFlex.Infrastructure.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Serilog;
using Serilog.Events;

// Configurar Serilog antes de criar o builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("===========================================");
    Log.Information("Iniciando CSIFLEX Server Application");
    Log.Information("===========================================");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog a partir do appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId());

    // Configuração do Kestrel via appsettings.json
    builder.WebHost.UseKestrel((context, serverOptions) =>
    {
        serverOptions.Configure(context.Configuration.GetSection("Kestrel"));
    });

    Log.Information("Configurando serviços da aplicação...");

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Configuração de autenticação
    builder.Services.AddAuthorizationCore();
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<ProtectedSessionStorage>();
    builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

    // Configuração de Database Context
    builder.Services.AddScoped<DatabaseContext>();

    // Configuração de Repositórios
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    // Configuração de Serviços
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<AuthenticationService>();
    builder.Services.AddScoped<IUserService, UserService>();

    Log.Information("Serviços configurados com sucesso");

    var app = builder.Build();

    // Configurar Serilog para requisições HTTP
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : elapsed > 5000
                ? LogEventLevel.Warning
                : LogEventLevel.Information;
    });

    Log.Information("Configurando pipeline HTTP...");

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
        Log.Information("Ambiente de produção detectado - HSTS habilitado");
    }
    else
    {
        Log.Information("Ambiente de desenvolvimento detectado");
    }

    // Redirecionar HTTP para HTTPS apenas em produção
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        Log.Information("Redirecionamento HTTPS habilitado");
    }

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    Log.Information("Pipeline HTTP configurado com sucesso");
    Log.Information("===========================================");
    Log.Information("CSIFLEX Server iniciado e pronto para receber requisições");
    Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);
    Log.Information("URLs: Verifique as configurações do Kestrel no appsettings.json");
    Log.Information("===========================================");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação terminou inesperadamente devido a um erro fatal");
    throw;
}
finally
{
    Log.Information("===========================================");
    Log.Information("Encerrando CSIFLEX Server Application");
    Log.Information("===========================================");
    Log.CloseAndFlush();
}
