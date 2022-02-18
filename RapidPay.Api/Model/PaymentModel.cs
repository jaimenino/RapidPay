using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Api.Model
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public double Amount { get; set; }
    }
}
