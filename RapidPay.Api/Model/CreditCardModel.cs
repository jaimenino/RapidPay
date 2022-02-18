using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Api.Model
{
    public class CreditCardModel
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public float InitialBalance { get; set; }
        public float Balance { get; set; }
    }
}
