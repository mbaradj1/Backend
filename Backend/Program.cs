using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Assuming you have AppDbConnectionString defined in your configuration
var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CrmServiceConfiguration>();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();

app.Run();

app.Run();

public class CrmServiceConfiguration
{
    private readonly HttpClient _httpClient;

    public CrmServiceConfiguration(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IOrganizationService GetOrganizationService()
    {
        // Configuration de la connexion Dynamics 365
        // Remplacez les valeurs par vos propres informations
        string crmUrl = "https://orgc82bf837.crm4.dynamics.com/api/data/v9.0/";
        string username = "Mamadou.BARADJI@TalanCloudExperts.onmicrosoft.com"; // Remplacez par votre adresse e-mail Dynamics 365
        string password = "Verrati.93100"; // Remplacez par le mot de passe de votre compte Dynamics 365

        var authenticationHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}"));

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationHeader);

        // Use _httpClient for making HTTP requests to Dynamics 365
        // Example: var response = await _httpClient.GetAsync($"{crmUrl}/someEndpoint");

        // Modify the code as needed based on your specific Dynamics 365 API requests

        // Return a mock IOrganizationService for demonstration purposes
        return new MockOrganizationService();
    }

    private class MockOrganizationService : IOrganizationService
    {
        // Implement the IOrganizationService interface methods as needed
        // Example: public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet) { }
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            throw new NotImplementedException();
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }

        // ... other IOrganizationService methods
    }
}
