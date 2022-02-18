using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Application.Services
{
    public interface IPaymentService
    {
        double GetCurrentFee();
    }
    public class PaymentService : IPaymentService
    {
        private readonly RapidPayEntities _dbContext = new();
        /// <summary>
        /// Returns the fee valid for the current hour.
        /// </summary>
        /// <returns>Fee value</returns>
        public double GetCurrentFee()
        {
            var currentFee = _dbContext.Fees.OrderByDescending(x => x.Id)?.FirstOrDefault();
            double feeValue = 0;

            if (currentFee == null)
            {
                feeValue = GetNewFee(1);
                currentFee = new Fee()
                {
                    CurrentFee = feeValue,
                    TimeFrom = DateTime.Now,
                    TimeTo = DateTime.Now.AddHours(1)
                };

                _dbContext.Fees.Add(currentFee);
                _dbContext.SaveChanges();
            }

            //If current fee is still valid
            if (currentFee.TimeTo > DateTime.Now)
            {
                feeValue = currentFee.CurrentFee;
            }
            else
            {
                //Update the fee and return the new one
                var newFeeValue = GetNewFee(currentFee.CurrentFee);
                currentFee.CurrentFee = newFeeValue;
                currentFee.TimeFrom = DateTime.Now;
                currentFee.TimeTo = currentFee.TimeFrom.AddHours(1);

                _dbContext.Entry(currentFee).State = EntityState.Modified;
                _dbContext.SaveChanges();

                feeValue = newFeeValue;
            }
            return feeValue;
        }
        /// <summary>
        /// Randomize a new fee between 0.1 and 2
        /// </summary>
        /// <param name="currentFee"></param>
        /// <returns>New fee value</returns>
        private double GetNewFee(double currentFee)
        {
            var calculatedFee = (new Random().NextDouble() * (2 - 0.1) + 0.1);
            return Math.Round(currentFee * calculatedFee, 2);
        }
    }
}
