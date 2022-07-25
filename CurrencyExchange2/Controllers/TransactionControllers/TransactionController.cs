using CurrencyExchange2.Model.Account;
using CurrencyExchange2.Requests;
using CurrencyExchange2.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange2.Controllers.TransactionControllers
{
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpPost]
        [Route("BuyCoin")]
        public async Task<ActionResult<List<Response>>> BuyCoin([FromBody] BuyCoin BuyCoins, [FromHeader] string token)
        {

            var tokenExist = await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token);
            if (tokenExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == BuyCoins.UserEmail);

            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });

            if(BuyCoins.Amount<=0)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Amount should be greater than 0" });

            string symbol = BuyCoins.CoinToBuy + BuyCoins.BuyWİthThisCoin;
            var symbolExist = await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbol);
            if (symbolExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You cant buy " +BuyCoins.CoinToBuy +" with " + BuyCoins.BuyWİthThisCoin });
            }
            var accountExist = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExist.UserId);
            if (accountExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });


            var balanceExist = await _context.Balances.SingleOrDefaultAsync(p => p.CoinName == BuyCoins.BuyWİthThisCoin && p.Account==accountExist);
            if (balanceExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "You dont have any" + BuyCoins.BuyWİthThisCoin });
            }
            var coinTypeToBuy = await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbol);
            double coinPrices = Convert.ToDouble(coinTypeToBuy.price);
            double totalAmount = coinPrices * BuyCoins.Amount;

            if (totalAmount > balanceExist.TotalBalance)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You dont have enough" + BuyCoins.BuyWİthThisCoin });
            }
            var balanceExistForBuyCoin = await _context.Balances.SingleOrDefaultAsync(p => p.Account == accountExist && p.CoinName == BuyCoins.CoinToBuy);
            var boughtCoinId = await _context.CoinTypes.SingleOrDefaultAsync(p => p.CoinName == BuyCoins.CoinToBuy);

            if (balanceExistForBuyCoin == null)
            {
                
                balanceExist.TotalBalance -= totalAmount;
                Balance tempBalance = new Balance();
                tempBalance.CoinName = BuyCoins.CoinToBuy;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = BuyCoins.Amount;
                tempBalance.CoinId = boughtCoinId.CoinId;
                _context.Balances.Add(tempBalance);

            }
            else
            {
                balanceExistForBuyCoin.TotalBalance += BuyCoins.Amount;
                balanceExist.TotalBalance -= totalAmount;
                balanceExist.ModifiedDate = DateTime.UtcNow;
            }



           
            _context.SaveChanges();
            return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Successfully Bought !" + BuyCoins.CoinToBuy });

        }
    }
}
