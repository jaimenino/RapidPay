using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RapidPay.Application.Services;
using RapidPay.Api.Model;
using RapidPay.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CardManagementController : ControllerBase
    {
        private readonly ILogger<CardManagementController> _logger;
        private readonly ICardService _cardService;

        public CardManagementController(ILogger<CardManagementController> logger, ICardService cardService)
        {
            _logger = logger;
            _cardService = cardService;
        }

        /// <summary>
        /// Creates a new card in the system
        /// </summary>
        /// <param name="card">Object with the required card information</param>
        /// <returns>Ok if the card was created or BadRequest if there was an error</returns>
        [HttpPost("/CreateCard")]
        public IActionResult CreateCard([FromBody] CreditCardModel card)
        {
            try
            {
                if(card.Number == null)
                    throw new NullReferenceException("Card number is required");
                
                if(card.InitialBalance == 0)
                    throw new Exception("Initial balance must be greater than 0");

                var newCard = _cardService.CreateCard(card.Number, card.InitialBalance);
                return Ok(newCard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            
        }
        /// <summary>
        /// Returns the available balance for a card
        /// </summary>
        /// <param name="card">Unique card id</param>
        /// <returns>Ok with the balance or BadRequest if there was an error</returns>
        [HttpGet("/GetCardBalance")]
        public IActionResult GetCardBalance([FromBody] CreditCardModel card)
        {
            try
            {
                var balance = _cardService.GetCardBalance(card.Id);
                return Ok(new { balance = balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }

        }
        /// <summary>
        /// Registers a payment in the system
        /// </summary>
        /// <param name="payment">Object with the required payment information</param>
        /// <returns>Ok if payment was registered or BadRequest if there was an error</returns>
        [HttpPost("/Pay")]
        public IActionResult MakePayment([FromBody] PaymentModel payment)
        {
            try
            {
                var newPayment = _cardService.MakePayment(payment.CardId, payment.Amount);
                return Ok(newPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }

        }
    }
}
