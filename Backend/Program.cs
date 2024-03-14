using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.Xrm.Tooling.Connector;

var builder = WebApplication.CreateBuilder(args);

// Ajoute la configuration à votre application
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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

public class CrmServiceConfiguration
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _crmUrl;
    private readonly string _username;
    private readonly string _password;

    public CrmServiceConfiguration(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _crmUrl = _configuration["CrmSettings:Url"];
        _username = _configuration["CrmSettings:Username"];
        _password = _configuration["CrmSettings:Password"];

        var connectionString = $"AuthType=Office365;Url={_crmUrl};Username={_username};Password={_password}";
        var crmServiceClient = new CrmServiceClient(connectionString);

        if (crmServiceClient.IsReady)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_username}:{_password}")));
        }
        else
        {
            throw new Exception("La connexion à Dynamics 365 a échoué.");
        }
    }

    public IOrganizationService GetOrganizationService()
    {
        var connectionString = $"AuthType=Office365;Url={_crmUrl};Username={_username};Password={_password}";
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

    public async Task CreateContactAsync(IOrganizationService service)
    {
        Entity contact = new Entity("contact");
        contact["firstname"] = "Bob";
        contact["lastname"] = "Smith";
        Guid contactId = service.Create(contact);
        Console.WriteLine("New contact id: {0}.", contactId.ToString());
    }

    public async Task RetrieveContactAsync(IOrganizationService service)
    {
        QueryExpression query = new QueryExpression("contact");
        query.ColumnSet = new ColumnSet(true);
        query.Criteria.AddCondition("firstname", ConditionOperator.Equal, "Bob");

        EntityCollection contacts = service.RetrieveMultiple(query);
        if (contacts.Entities.Count > 0)
        {
            var contact = contacts.Entities.First();
            Console.WriteLine("Retrieved contact: {0} {1}", contact["firstname"], contact["lastname"]);
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }

    public async Task UpdateContactAsync(IOrganizationService service)
    {
        QueryExpression query = new QueryExpression("contact");
        query.ColumnSet = new ColumnSet(true);
        query.Criteria.AddCondition("firstname", ConditionOperator.Equal, "Bob");

        EntityCollection contacts = service.RetrieveMultiple(query);
        if (contacts.Entities.Count > 0)
        {
            var contact = contacts.Entities.First();
            contact["jobtitle"] = "CEO";
            service.Update(contact);
            Console.WriteLine("Updated contact: {0} {1}", contact["firstname"], contact["lastname"]);
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }

    public async Task DeleteContactAsync(IOrganizationService service)
    {
        QueryExpression query = new QueryExpression("contact");
        query.ColumnSet = new ColumnSet(true);
        query.Criteria.AddCondition("firstname", ConditionOperator.Equal, "Bob");

        EntityCollection contacts = service.RetrieveMultiple(query);
        if (contacts.Entities.Count > 0)
        {
            var contact = contacts.Entities.First();
            service.Delete("contact", contact.Id);
            Console.WriteLine("Deleted contact: {0} {1}", contact["firstname"], contact["lastname"]);
        }
        else
        {
            Console.WriteLine("Contact not found.");
        }
    }
}
