using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Core.Model;
using Microsoft.AspNetCore.Mvc;
using CatalogManaging.Model;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using AutoMapper;
using System.Net;
using Microsoft.Extensions.Logging;

namespace CatalogManaging.Controllers
{
    [Route("catalogs")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CatalogsController : ControllerBase
    {
        private readonly ICardEventHandler _cardEventHandler;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CatalogsController> _logger;

        public CatalogsController(ICatalogRepository catalogRepository, ICardEventHandler cardEventHandler, IMapper mapper, ILogger<CatalogsController> logger)
        {
            _catalogRepository = catalogRepository;
            _cardEventHandler = cardEventHandler;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all the existing Catalogs
        /// </summary>
        /// <returns>An Action result containing list of catalog</returns>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IList<Catalog>> GetCatalogs()
        {
            var catalogs = _catalogRepository.GetCatalogs();
            return Ok(catalogs);
        }

        /// <summary>
        /// Creates a Catalog
        /// creater will be the admin
        /// </summary>
        /// <param name="catalogCreationDto">Details of the Catalog to be created along with creater information </param>
        /// <returns>An action result of type Catalog</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public ActionResult<Catalog> CreateCatalog([FromBody] CatalogCreationDto catalogCreationDto)
        {
            _logger.LogInformation("Creation of catalog with name -- {catalogName} -- initiated", catalogCreationDto.CatalogName);
            var catalog = new Catalog( catalogCreationDto.CatalogName, catalogCreationDto.UserId, _catalogRepository, _cardEventHandler);

            return CreatedAtRoute("GetCatalog",new { id = catalog.Id },catalog);
        }


        /// <summary>
        /// Gets the edited cards awaiting approval from admin
        /// </summary>
        /// <param name="id">Catalog Id</param>
        /// <returns>An Action Result of list of Pending Card</returns>
        [Route("{id}/pendingapproval")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IList<PendingCard>> GetApprovalPendingCards(int id)
        {
            var catalog = Catalog.GetExistingCatalog(id, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Retrieving all cards awaiting approval for catalog {id}", id);
            var cards = catalog.GetAllUnApprovedCards();
            return Ok(cards);
        }

        /// <summary>
        /// Get catalog by its ID
        /// </summary>
        /// <param name="id">Catlog Id</param>
        /// <returns>An Action Result of type Catalog</returns>
        [HttpGet("{id}",Name ="GetCatalog")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<Catalog> GetCatalog(int id)
        {
            _logger.LogInformation("Retrieving details for catalog {id}", id);
            var catalog = Catalog.GetExistingCatalog(id, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            return Ok(catalog);
        }

        /// <summary>
        /// Reject the edit on the card for the catalog
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="cardEditDto">Edited card's details</param>
        /// <param name="id">Catalog Id</param>
        /// <returns>Returns true on success</returns>
        [Route("{id}/reject")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<bool> RejectEditedCard([FromBody] IEnumerable<CardDto> cardEditDto, int id)
        {
            _logger.LogInformation("Rejection of  edit request on Cards {ids} for catalog {id} ", string.Join(",", cardEditDto.Select(a => a.CardId)), id);
            var pendingCards = _mapper.Map<IList<PendingCard>>(cardEditDto);
            var catalog = Catalog.GetExistingCatalog(id, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }

            bool result=catalog.RejectEditOnCard(pendingCards, cardEditDto.First().UserId);
            if(!result)
            {
                return Forbid();
            }
            return Ok(result);
        }
    }
}
