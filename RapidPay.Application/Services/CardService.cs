using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.Application.Services
{
    public interface ICardService
    {
        CreditCard CreateCard(string cardNumber, float initailBalance);
        double GetCardBalance(int cardId);
        CardPayment MakePayment(int cardId, double amount);
    }
    public class CardService : ICardService
    {
        private readonly IPaymentService _paymentService;
        private readonly RapidPayEntities _dbContext = new();
        public CardService(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        /// <summary>
        /// Creates a new card in the database
        /// </summary>
        /// <param name="cardNumber">15 numbers of the card</param>
        /// <param name="initialBalance">Initial balance for the card</param>
        /// <returns>Card with all its information</returns>
        /// <exception cref="InvalidOperationException">Throws exception if the card number exists in the database</exception>
        public CreditCard CreateCard(string cardNumber, float initialBalance)
        {
            var existsCard = _dbContext.CreditCards.Where(x => x.CardNumber == cardNumber)?.Count() > 0;
            if (existsCard)
            {
                throw new InvalidOperationException("Card already exists.");
            }
            else
            {
                var newCard = new CreditCard()
                {
                    CardNumber = cardNumber,
                    InitalBalance = initialBalance,
                    CurrentBalance = initialBalance
                };
                _dbContext.CreditCards.Add(newCard);
                _dbContext.SaveChanges();

                return newCard;
            }
        }
        /// <summary>
        /// Returns the available balance for the card
        /// </summary>
        /// <param name="cardId">Unique card id</param>
        /// <returns>Available balance value</returns>
        public double GetCardBalance(int cardId)
        {
            return Math.Round(_dbContext.CreditCards.Where(x => x.Id == cardId)?.FirstOrDefault()?.CurrentBalance ?? 0, 2);
        }
        /// <summary>
        /// Creates a payment for the card and makes the discount to the available balance. For each payment a fee is charged and discounted from the available balance.
        /// </summary>
        /// <param name="cardId">Unique card id</param>
        /// <param name="amount">Amount of the payment</param>
        /// <returns>Payment with all its information</returns>
        /// <exception cref="Exception">Throws exception if there is not enough balance to discount the payment</exception>
        public CardPayment MakePayment(int cardId, double amount)
        {
            //Gets Fee
            double fee = _paymentService.GetCurrentFee();

            //Validates card and balance
            double currentBalance = GetCardBalance(cardId);
            double paymentWithFee = Math.Round(amount * (1 + (fee / 100)), 2);
            if ((currentBalance - paymentWithFee) >= 0)
            {
                //Create payment
                return RegisterPayment(cardId, paymentWithFee);
            }
            else
            {
                throw new Exception("There is not enough balance for this payment");
            }
        }
        /// <summary>
        /// Store the payment in the database
        /// </summary>
        /// <param name="cardId">Unique card id</param>
        /// <param name="amount">Amount of the payment with fee added</param>
        /// <returns>Payment with all its information</returns>
        private CardPayment RegisterPayment(int cardId, double amount)
        {
            var transaction = _dbContext.Database.BeginTransaction();
            var newPayment = new CardPayment()
            {
                CardId = cardId,
                Amount = amount,
                IsCompleted = false,
                PaymentDate = DateTime.Now
            };

            try
            {
                var cardDb = _dbContext.CreditCards.Where(x => x.Id == cardId)?.FirstOrDefault();
                if (cardDb == null)
                {
                    throw new Exception("Card not found");
                }

                //Create payment record
                newPayment.IsCompleted = true;
                _dbContext.CardPayments.Add(newPayment);

                //Update balance
                cardDb.CurrentBalance = cardDb.CurrentBalance - amount;
                _dbContext.Entry(cardDb).State = EntityState.Modified;
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return newPayment;
        }
    }
}
