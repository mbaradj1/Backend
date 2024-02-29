using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CrmServiceConfiguration>(); // Ajout du service CRM

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Utilisez le service CRM dans votre application
var crmServiceConfiguration = app.Services.GetRequiredService<CrmServiceConfiguration>();
var crmService = crmServiceConfiguration.GetOrganizationService();

app.UseAuthorization();
app.MapControllers();

// Mettez à jour le code pour activer HTTPS
app.UseHttpsRedirection();

app.Run();

public class CrmServiceConfiguration
{
    public IOrganizationService GetOrganizationService()
    {
        // Configuration de la connexion Dynamics 365
        // Remplacez les valeurs par vos propres informations
        string crmUrl = "https://orgc82bf837.crm4.dynamics.com/api/data/v9.0/";
        string username = "Mamadou.BARADJI@TalanCloudExperts.onmicrosoft.com"; // Remplacez par votre adresse e-mail Dynamics 365
        string password = "Verrati.93100"; // Remplacez par le mot de passe de votre compte Dynamics 365

        var connectionString = $"AuthType=Office365;Url={crmUrl};Username={username};Password={password}";
        var crmServiceClient = new CrmServiceClient(connectionString);

        if (crmServiceClient.IsReady)
        {
            return (IOrganizationService)crmServiceClient.OrganizationServiceProxy;
        }
        else
        {
            throw new Exception("La connexion à Dynamics 365 a échoué.");
        }
    }
}
