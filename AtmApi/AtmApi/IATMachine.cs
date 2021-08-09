using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ATM.Api
{
    #region Structs

    public enum PaperNote
    {
        Five = 5,
        Ten = 10,
        Twenty = 20,
        Fifty = 50
    }

    public struct Money
    {
        public int Amount
        {
            get
            {
                int total = 0;
                if (Notes != null && Notes.Count > 0)
                {
                    Notes.ToList().ForEach
                    (
                        pair =>
                        {
                            total += Convert.ToInt32(pair.Key, CultureInfo.InvariantCulture) * pair.Value;
                        }
                    );
                }
                else
                    Notes = new Dictionary<PaperNote, int>();

                return total;
            }
        }

        public Dictionary<PaperNote, int> Notes { get; set; }
    }

    public struct Fee
    {
        public string CardNumber { get; set; }

        public decimal WithdrawalFeeAmount { get; set; }

        public DateTime WithdrawalDate { get; set; }
    }

    public struct CardNumber
    {
        public string Number { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int CVC { get; set; }
    } 
    #endregion

    public interface IATMachine
    {
        /// <summary>
        /// ATM Manufacturer
        /// </summary>
        string Manufacturer { get; }


        /// <summary>
        /// Serial Number
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Insert bank card into ATM machine
        /// </summary>
        /// <param name="cardNumber">Card Number</param>
        void InsertCard(string cardNumber);

        /// <summary>
        /// Retrieve the balance available on the card
        /// </summary>
        /// <returns>Card's balance</returns>
        decimal GetCardBalance();

        /// <summary>
        /// Withdraw money from ATM.
        /// </summary>
        /// <param name="amount">Amount of money to withdraw</param>
        /// <returns>Money withdrawn from an ATM</returns>
        Money WithdrawMoney(int amount);

        /// <summary>
        /// Return card back to client
        /// </summary>
        void ReturnCard();

        /// <summary>
        /// Load the money into ATM machine
        /// </summary>
        /// <param name="money">Money loaded into ATM machine</param>
        void LoadMoney(Money money);

        /// <summary>
        /// Retrieves charged fees
        /// </summary>
        /// <returns>List of charged fees</returns>
        IEnumerable<Fee> RetrieveChargedFees();
    }
}
