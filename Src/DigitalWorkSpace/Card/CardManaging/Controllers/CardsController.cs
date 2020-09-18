using System;
using System.Collections.Generic;
using System.Net;
using CardManaging.Core.Contracts;
using CardManaging.Core.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CardManaging.Controllers
{
    [Route("cards")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CardsController : ControllerBase
    {
        private readonly ICardOperations _cardOperations;
        private readonly ILogger<CardsController> _logger;

        public CardsController(ICardOperations cardOperations, ILogger<CardsController> logger)
        {
            _cardOperations = cardOperations;
            _logger = logger;
        }

        /// <summary>
        /// Gets all the created cards
        /// </summary>
        /// <returns>An Action Result of list of Cards</returns>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<Card>> GetCards()
        {
            var cards = _cardOperations.GetAllCards();
            return Ok(cards);
        }

        /// <summary>
        /// Gets cards by its id
        /// will result all the version of the card
        /// can filter with version as well
        /// </summary>
        /// <param name="id">id of the card</param>
        /// <param name="version">version of the card</param>
        /// <returns>An Action Result of list of Cards</returns>
        [HttpGet("{id}", Name ="GetCard")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<IList<Card>> GetCard(int id,[FromQuery] int version)
        {
            var card = _cardOperations.GetCards(id,version);
            if(card==null)
            {
                return NotFound();
            }
            return Ok(card);
        }

        /// <summary>
        /// Create card
        /// </summary>
        /// <param name="card"></param>
        /// <returns>An Action Result of type</returns>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<Card> CreateCard([FromBody] Card card)
        {
            _logger.LogInformation("Creation of card {id} initiated", card.Id);

            var createdCard = _cardOperations.CreateCard(card);
            if (createdCard == null)
            {
                return NotFound();
            }
            return CreatedAtRoute("GetCard",new { id = createdCard.Id,version=createdCard.Version }, createdCard);
        }

        /// <summary>
        /// Edit the card
        /// </summary>
        /// <param name="card">Card which needs to be edited</param>
        /// <returns>An Action Result of type card</returns>
        [HttpPatch]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<Card> EditCard([FromBody] EditedCard card)
        {
            _logger.LogInformation("Edit of card {id} initiated", card.Id);

            var editedCard=_cardOperations.EditCard(card);
            if (editedCard == null)
            {
                return NotFound();
            }
            return Ok(editedCard);
        }
    }
}
