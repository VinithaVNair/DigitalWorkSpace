using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AutoMapper;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogManaging.Controllers
{
    [Route("catalogs/{catalogId}/cards")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CardsController : ControllerBase
    {
        private readonly ICardEventHandler _cardEventHandler;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CardsController> _logger;

        public CardsController(ICatalogRepository catalogRepository, ICardEventHandler cardEventHandler, IMapper mapper, ILogger<CardsController> logger)
        {
            _catalogRepository = catalogRepository;
            _cardEventHandler = cardEventHandler;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Add cards to Catalog
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="cardCreationDtos">Cards detail</param>
        /// <param name="catalogId">Catalog id to which the cards needs to be added</param>
        /// <returns>An action result with a list of cards added</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<Card>> AddCards([FromBody] IEnumerable<CardDto> cardCreationDtos, int catalogId)
        {
            _logger.LogInformation("Adding new Cards {ids} in catalog {catalogId} initiated", string.Join(",", cardCreationDtos.Select(a => a.CardId)), catalogId);
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if(catalog==null)
            {
                return NotFound();
            }

            var cards = _mapper.Map<IList<Card>>(cardCreationDtos);

            var cardAdded=catalog.AddCards(cards, cardCreationDtos.First().UserId);
            if(cardAdded== null)
            {
                return Forbid();
            }

            return Ok(cardAdded);
        }

        /// <summary>
        /// Get all cards which belong give catalog
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns>An action result with a list of cards</returns>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<Card>> GetCards(int catalogId)
        {
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var cards = catalog.GetCards();
            return Ok(cards);
        }

        /// <summary>
        /// Delete card from the given catalog
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="cardDeletionDto"></param>
        /// <param name="catalogId">Catalog id from which cards needs to be deleted</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<bool> DeleteCard([FromBody] IEnumerable<CardDto> cardDeletionDto, int catalogId)
        {
            _logger.LogInformation("Deletiong of  Cards {ids} from catalog {catalogId} initiated", string.Join(",", cardDeletionDto.Select(a => a.CardId)), catalogId);
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var cards = _mapper.Map<IList<Card>>(cardDeletionDto);

            var isDeleted=catalog.RemoveCard(cards, cardDeletionDto.FirstOrDefault().UserId);
            if(!isDeleted)
            {
                return Forbid();
            }
            return Ok(isDeleted);
        }

        /// <summary>
        /// Edit the card by replacing older version of card with the newer vesion
        /// Only admin of the catalog can perform this action
        /// </summary>
        /// <param name="cardEditDto">Details of the Edited card</param>
        /// <param name="catalogId">Catalog whose card needs to be edited</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<IList<Card>> EditCard([FromBody] IEnumerable<CardDto> cardEditDto, int catalogId)
        {
            _logger.LogInformation("Editing of  Cards {ids} in catalog {catalogId} initiated", string.Join(",", cardEditDto.Select(a => a.CardId)), catalogId);

            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var pendingCards = _mapper.Map<IList<PendingCard>>(cardEditDto);
            var editedCards=catalog.EditCard(pendingCards, cardEditDto.First().UserId);
            if(editedCards==null)
            {
                return Forbid();
            }
            return Ok(editedCards);
        }

        /// <summary>
        /// Get cards from catalog based on its id
        /// </summary>
        /// <param name="catalogId">Catalog id from which the card's details are required</param>
        /// <param name="id">card id</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetCard")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<Card> GetCard(int id, int catalogId)
        {
            var catalog = Catalog.GetExistingCatalog(catalogId, _catalogRepository, _cardEventHandler);
            if (catalog == null)
            {
                return NotFound();
            }
            var card = catalog.GetCard(id);
            return Ok(card);
        }
    }
}
