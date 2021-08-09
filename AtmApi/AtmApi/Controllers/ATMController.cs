using ATM.Api;
using AtmApp.Helpers;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.SessionState;

namespace AtmApp.Controllers
{
    [RoutePrefix("api/atm")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class ATMController : ApiController
    {
        private ATMachine atm;

        public ATMController()
        {
            HttpSessionState session = HttpContext.Current.Session;

            //atm = ObjectSerializer.DeSerializeObject<ATMachine>(@"C:\Windows\Temp\atm.xml");//(ATMachine)session["ATM"];
            atm = JSONSerializer.Load();

            if (atm == null)
            {
                atm = new ATMachine("ATM-Manufacturer", Guid.NewGuid().ToString());
                //session.Add("ATM", atm);
                //ObjectSerializer.SerializeObject<ATMachine>(atm, @"C:\Windows\Temp\.xml");
                JSONSerializer.Save(atm, true);
            }
        }

        [HttpGet]
        [Route("getAtMachine")] //   /api/atm/getAtMachine
        public ATMachine GetAtm()
        {         
            return atm;
        }

        [HttpGet]
        [Route("getTotalMoney")] //   /api/atm/getTotalMoney
        public IHttpActionResult GetTotalMoney()
        {
            return Ok(atm.GetTotalMoney());
        }

        [HttpGet]
        [Route("getSupportedPaperNoteTypes")] //   /api/atm/getSupportedPaperNoteTypes
        public IHttpActionResult GetSupportedPaperNoteTypes()
        {
            return Ok(Enum.GetValues(typeof(PaperNote)));
        }

        [HttpGet]
        [Route("insertCard/{cardNumber}")] //   /api/atm/insertCard/12345
        public IHttpActionResult InsertCard(string cardNumber)
        {
            atm.InsertCard(cardNumber);
            JSONSerializer.Save(atm, true);
            return Ok("Card '" + cardNumber + "' inserted...");
        }

        [HttpGet]
        [Route("getCardBalance")] //   /api/atm/getCardBalance
        public IHttpActionResult GetCardBalance()
        {
            return Ok(atm.GetCardBalance());
        }

        [HttpGet]
        [Route("returnCard")] //   /api/atm/returnCard
        public IHttpActionResult ReturnCard()
        {
            string insertedCardNumber = atm.InsertedCardNumber;
            atm.ReturnCard();
            JSONSerializer.Save(atm, true);
            return Ok("Card '" + insertedCardNumber + "' is returned...");
        }

        [HttpGet]
        [Route("loadMoney/{param1}/{param2}/{param3}/{param4}")]
        public IHttpActionResult LoadMoney(int param1, int param2, int param3, int param4)
        {
            int[] arr = new int[] { param1, param2, param3, param4 };
            Money money = new Money() { Notes = ATMachine.createNotes(arr) };

            atm.LoadMoney(money);
            JSONSerializer.Save(atm, true);
            return Ok(money.Amount + "€ successfully added to your account.");
        }

        [HttpGet]
        [Route("withdrawMoney/{amount}")]
        public IHttpActionResult WithdrawMoney(int amount)
        {
            try
            {
                Money money = atm.WithdrawMoney(amount);
                JSONSerializer.Save(atm, true);
                return Ok(money);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet]
        [Route("loadMoneyIntoATM/{param1}/{param2}/{param3}/{param4}")]
        public IHttpActionResult LoadMoneyIntoATM(int param1, int param2, int param3, int param4)
        {
            int[] arr = new int[] { param1, param2, param3, param4 };
            Money money = new Money() { Notes = ATMachine.createNotes(arr) };

            atm.LoadMoneyIntoATM(money);
            JSONSerializer.Save(atm, true);
            return Ok(atm.GetTotalMoney());
        }

        [HttpGet]
        [Route("retrieveChargedFees")] //   /api/atm/retrieveChargedFees
        public IHttpActionResult RetrieveChargedFees()
        {
            return Ok(atm.RetrieveChargedFees());
        }

        
    }
}
