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
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("/GetCardBalance")]
        public IActionResult GetCardBalance([FromBody] CreditCardModel card)
        {
            try
            {
                var balance = _cardService.GetCardBalance(card.Id);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

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
                return BadRequest(ex.Message);
            }

        }
    }
}
