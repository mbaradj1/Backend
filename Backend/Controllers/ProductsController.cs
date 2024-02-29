using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IOrganizationService _organizationService;

        public ProductsController(AppDbContext appDbContext, IOrganizationService organizationService)
        {
            _appDbContext = appDbContext;
            _organizationService = organizationService;
        }

        [HttpGet]
        public IActionResult GetContactsFromDynamics()
        {
            try
            {
                // Exemple : Récupérer des contacts depuis Dynamics 365
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet("contactid", "firstname", "lastname", "emailaddress1");

                var contacts = _organizationService.RetrieveMultiple(query).Entities
                    .Select(e => new
                    {
                        ContactId = e.Id,
                        FirstName = e.GetAttributeValue<string>("firstname"),
                        LastName = e.GetAttributeValue<string>("lastname"),
                        Email = e.GetAttributeValue<string>("emailaddress1")
                    })
                    .ToList();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        // Autres actions pour les produits...
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                // Vérifiez si le modèle est valide
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ajouter la logique pour ajouter le produit à la base de données
                _appDbContext.Products.Add(product);
                await _appDbContext.SaveChangesAsync();

                // Retournez une réponse réussie
                return Ok(product);
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retournez une réponse d'erreur
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
    }

}
