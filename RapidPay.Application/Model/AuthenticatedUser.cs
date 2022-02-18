using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Application.Model
{
    public class AuthenticatedUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
