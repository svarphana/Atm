using ATM.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATM.Api
{
    public class ATMachine : IATMachine
    {
        #region Constructors
        public ATMachine()
        {
            _manufacturer = "ATM-Manufacturer";
            _serialNumber = Guid.NewGuid().ToString();
            _money = GenerateMoney();
            _fees = new List<Fee>();
            _clients = new List<Client>();
            _feePercentage = 1;
        }
        public ATMachine(string manufacturer, string serialNumber)
        {
            _manufacturer = manufacturer;
            _serialNumber = serialNumber;
            _money = GenerateMoney();
            _fees = new List<Fee>();
            _clients = new List<Client>();
            _feePercentage = 1;
        }
        #endregion

        #region Private Properties
        [JsonProperty]
        private List<Client> _clients;
        [JsonProperty]
        private List<Fee> _fees;
        private string _manufacturer;
        private string _serialNumber;
        [JsonProperty]
        private Money _money;
        private string _cardNumber;
        private decimal _feePercentage;
        #endregion

        #region Public Properties
        [JsonProperty]
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        [JsonProperty]
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        [JsonProperty]
        public string InsertedCardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; }
        }
        #endregion

        #region Public Methods
        public Money GetTotalMoney()
        {
            return _money;
        }

        /// <summary>
        /// Retrieve the balance available on the card
        /// </summary>
        /// <returns>Card's balance</returns>
        public decimal GetCardBalance()
        {
            decimal balance = 0;

            if (_cardNumber != null && _cardNumber.Length > 0)
            {
                Client client = _clients.Count > 0 ? _clients.FirstOrDefault(t => t.CardNumber == _cardNumber) : null;
                if (client != null)
                {
                    balance = client.TotalMoney;
                }
            }

            return balance;
        }

        /// <summary>
        /// Insert bank card into ATM machine
        /// </summary>
        /// <param name="cardNumber">Card Number</param>
        public void InsertCard(string cardNumber)
        {
            _cardNumber = cardNumber;
            Client client = _clients.Count > 0 ? _clients.FirstOrDefault(t => t.CardNumber == cardNumber) : null;
            if (client == null)
            {
                client = new Client { CardNumber = cardNumber, TotalMoney = GenerateMoneyAmount() };
                _clients.Add(client);
            }
            if (client.TotalMoney <= 0)
            {
                client.TotalMoney = GenerateMoneyAmount();
            }
        }

        /// <summary>
        /// Loads money into client's account
        /// </summary>
        /// <param name="money"></param>
        public void LoadMoney(Money money)
        {
            if (_cardNumber != null && money.Amount > 0)
            {
                Client client = _clients.FirstOrDefault(t => t.CardNumber == _cardNumber);
                if (client != null)
                {
                    _money = AddMoney(_money, money);
                    client.TotalMoney += money.Amount;
                    LoadMoneyIntoATM(money);
                }
            }
        }

        /// <summary>
        /// Loads money into ATM
        /// </summary>
        /// <param name="money">Money to load</param>
        public void LoadMoneyIntoATM(Money money)
        {
            if (money.Amount > 0)
            {
                _money = AddMoney(_money, money);
            }
        }


        /// <summary>
        /// SUbtracts some amount from money
        /// </summary>
        /// <param name="money">Parameter from wich required to subtract</param>
        /// <param name="amount">Amount of money to subtract</param>
        /// <returns>Returns 2 money params: first (Item1) is a remainder, second (Item2) is subtracted amount of money</returns>
        public Tuple<Money, Money> SubtractMoney(Money money, int amount)
        {
            Money remainder = money;
            Money subtractedMoney = new Money() { Notes = new Dictionary<PaperNote, int>() };

            if (remainder.Amount < amount)
            {
                throw new Exception("Insufficient funds");
            }
            Type enumType = typeof(PaperNote);
            string[] enumName = enumType.GetEnumNames();
            //Iterating enum from highest value to lowest
            for (int i = enumName.Length - 1; i >= 0; i--)
            {
                var enumKey = Enum.Parse(typeof(PaperNote), enumName[i]);
                int denomination = (int)enumKey;//50, 20, 10, 5
                int paperNoteTotalCount = remainder.Notes[(PaperNote)enumKey];
                int paperNoteCount = amount / denomination;
                int subtractPaperNoteCount = paperNoteCount <= paperNoteTotalCount ? paperNoteCount : paperNoteTotalCount;
                remainder.Notes[(PaperNote)enumKey] -= subtractPaperNoteCount;
                subtractedMoney.Notes.Add((PaperNote)enumKey, subtractPaperNoteCount);
                amount -= denomination * subtractPaperNoteCount;

                if (amount == 0)
                {
                    i = (-1);//exit loop
                }
            }

            if (amount > 0)
            {
                throw new Exception("Can't subtract final amount '" + amount + "'. " +
                    "Please take into account that we have only such paper notes: " + string.Join(",", enumName));
            }

            money = remainder;

            return new Tuple<Money, Money>(remainder, subtractedMoney);
        }

        public IEnumerable<Fee> RetrieveChargedFees()
        {
            return _fees;
        }

        public void ReturnCard()
        {
            _cardNumber = null;
        }

        /// <summary>
        /// Withdraw money from ATM.
        /// </summary>
        /// <param name="amount">Amount of money to withdraw</param>
        /// <returns>Money withdrawn from an ATM</returns>
        public Money WithdrawMoney(int amount)
        {
            Money money = new Money();

            if (amount > 0)
            {
                if (_cardNumber != null)
                {
                    Client client = _clients.FirstOrDefault(t => t.CardNumber == _cardNumber);
                    if (client != null)
                    {
                        if (client.TotalMoney - amount < 0)
                        {
                            throw new Exception("Insufficient funds on your account.");
                        }
                        var res = SubtractMoney(_money, amount);
                        _money = res.Item1;
                        client.TotalMoney -= amount;
                        money = res.Item2;
                        _fees.Add(new Fee() { CardNumber = _cardNumber, WithdrawalDate = DateTime.Now, WithdrawalFeeAmount = (decimal)amount / 100.00m * _feePercentage });
                    }
                }
                else
                    throw new Exception("Please insert your bank card first");
            }
            else
                throw new Exception("Please specify amount > 0");

            return money;
        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates random amount of money
        /// </summary>
        /// <returns>Generated money</returns>
        private Money GenerateMoney()
        {
            Dictionary<PaperNote, int> notes = new Dictionary<PaperNote, int>();
            var rnd = new Random();
            foreach (PaperNote item in (PaperNote[])Enum.GetValues(typeof(PaperNote)))
            {
                notes.Add(item, rnd.Next(5, 20));
            }

            Money money = new Money() { Notes = notes };

            return money;
        }

        private int GenerateMoneyAmount()
        {
            var rnd = new Random();
            int res = rnd.Next(95, 1000);
            while (res % 5 != 0)
            {
                res = rnd.Next(95, 1000);
            }
            return res;
        }

        /// <summary>
        /// Adds one amount of money to another one
        /// </summary>
        /// <param name="m1">Money</param>
        /// <param name="m2">Money</param>
        /// <returns>Sum</returns>
        private Money AddMoney(Money m1, Money m2)
        {
            Money sum = m1;

            if (m2.Amount > 0)
            {
                m2.Notes.ToList().ForEach
                (
                    pair =>
                    {
                        if (sum.Notes != null && sum.Notes.Count > 0 && sum.Notes.ContainsKey(pair.Key))
                        {
                            sum.Notes[pair.Key] += pair.Value;
                        }
                        else
                        {
                            if (sum.Notes == null)
                            {
                                sum.Notes = new Dictionary<PaperNote, int>();
                            }
                            sum.Notes.Add(pair.Key, pair.Value);
                        }
                    }
                );
            }

            return sum;
        }

        #endregion

        #region Static


        /// <summary>
        /// Creates notes from array
        /// </summary>
        /// <param name="arr">each item of this array is a quantity of PaperNotes enum (5, 10, 20, 50)</param>
        /// <returns>Notes</returns>
        public static Dictionary<PaperNote, int> createNotes(int[] arr)
        {
            Dictionary<PaperNote, int> notes = new Dictionary<PaperNote, int>();
            Type enumType = typeof(PaperNote);
            string[] enumName = enumType.GetEnumNames();
            //Iterating enums
            for (int i = 0; i < enumName.Length; i++)
            {
                if (arr[i] > 0)
                {
                    PaperNote enumKey = (PaperNote)Enum.Parse(typeof(PaperNote), enumName[i]);
                    int denomination = (int)enumKey;//5, 10, 20, 50
                    notes.Add(enumKey, arr[i]);
                }
            }

            return notes;
        }
        #endregion

    }
}
