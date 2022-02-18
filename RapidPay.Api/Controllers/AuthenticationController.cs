using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RapidPay.Application.Services;
using RapidPay.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _authenticationService.Authenticate(model.Username, model.Password);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
    }
}
