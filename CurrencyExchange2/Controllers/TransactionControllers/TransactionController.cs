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
        public async Task<ActionResult<List<Response>>> BuyCoin([FromBody] BuyCoin buyCoins, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == buyCoins.UserEmail);

            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });
            if (buyCoins.Amount < 0.001)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You cannot trade with this amount " });

            }
            if (buyCoins.Amount<=0)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Amount should be greater than 0" });

            string symbolOfCoins = buyCoins.CoinToBuy + buyCoins.BuyWİthThisCoin;
            if (await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbolOfCoins) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You cant buy " +buyCoins.CoinToBuy +" with " + buyCoins.BuyWİthThisCoin });
            }
            var accountExist = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExist.UserId);
            if (accountExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });


            var balanceExist = await _context.Balances.SingleOrDefaultAsync(p => p.CoinName == buyCoins.BuyWİthThisCoin && p.Account==accountExist);
            if (balanceExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "You dont have any" + buyCoins.BuyWİthThisCoin });
            }
            var coinTypeToBuy = await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbolOfCoins);
            double coinPrice = Convert.ToDouble(coinTypeToBuy.price);
            double totalAmount = coinPrice * buyCoins.Amount;
            if (totalAmount <= 0.001)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You can't buy this amount of coin" });
            if (totalAmount > balanceExist.TotalBalance)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You dont have enough" + buyCoins.BuyWİthThisCoin });
            }
            var balanceExistForBuyCoin = await _context.Balances.SingleOrDefaultAsync(p => p.Account == accountExist && p.CoinName == buyCoins.CoinToBuy);
            var coinToBuy = await _context.CoinTypes.SingleOrDefaultAsync(p => p.CoinName == buyCoins.CoinToBuy);

            if (balanceExistForBuyCoin == null)
            {
                Balance tempBalance = new Balance();


                balanceExist.TotalBalance -= totalAmount;
                tempBalance.CoinName = buyCoins.CoinToBuy;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = buyCoins.Amount;
                tempBalance.CoinId = coinToBuy.CoinId;
                _context.Balances.Add(tempBalance);

            }
            else
            {
                balanceExistForBuyCoin.TotalBalance += buyCoins.Amount;
                balanceExist.TotalBalance -= totalAmount;
                balanceExist.ModifiedDate = DateTime.UtcNow;
            }



           
            _context.SaveChanges();
            return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Successfully Bought !" + buyCoins.CoinToBuy });

        }


        [HttpPost]
        [Route("BuyCoinv2")]
        public async Task<ActionResult<List<Response>>> BuyCoinv2([FromBody] BuyCoin BuyCoinsv2, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == BuyCoinsv2.UserEmail);

            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });
            if (BuyCoinsv2.Amount < 0.001)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You cannot trade with this amount " });

            }

            if (BuyCoinsv2.Amount <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Amount should be greater than 0" });

            string symbolOfCoins = BuyCoinsv2.CoinToBuy + BuyCoinsv2.BuyWİthThisCoin;
            if (await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbolOfCoins) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You cant buy " + BuyCoinsv2.CoinToBuy + " with " + BuyCoinsv2.BuyWİthThisCoin });
            }
            var accountExist = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExist.UserId);
            if (accountExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });


            var balanceExist = await _context.Balances.SingleOrDefaultAsync(p => p.CoinName == BuyCoinsv2.BuyWİthThisCoin && p.Account == accountExist);
            if (balanceExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "You dont have any" + BuyCoinsv2.BuyWİthThisCoin });
            }
            var coinTypeToBuy = await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == symbolOfCoins);

            double coinPrice = Convert.ToDouble(coinTypeToBuy.price);
            double totalAmount = BuyCoinsv2.Amount / coinPrice ;
            if (totalAmount <= 0.001)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You can't buy this amount of coin" });

            if (totalAmount > balanceExist.TotalBalance)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "You dont have enough" + BuyCoinsv2.BuyWİthThisCoin });
            }
            var balanceExistForBuyCoin = await _context.Balances.SingleOrDefaultAsync(p => p.Account == accountExist && p.CoinName == BuyCoinsv2.CoinToBuy);
            var coinToBuy = await _context.CoinTypes.SingleOrDefaultAsync(p => p.CoinName == BuyCoinsv2.CoinToBuy);

            if (balanceExistForBuyCoin == null)
            {
                Balance tempBalance = new Balance();


                balanceExist.TotalBalance -= BuyCoinsv2.Amount;
                tempBalance.CoinName = BuyCoinsv2.CoinToBuy;
                tempBalance.Account = accountExist;
                tempBalance.TotalBalance = totalAmount;
                tempBalance.CoinId = coinToBuy.CoinId;
                _context.Balances.Add(tempBalance);

            }
            else
            {
                balanceExistForBuyCoin.TotalBalance += totalAmount;
                balanceExist.TotalBalance -= BuyCoinsv2.Amount;
                balanceExist.ModifiedDate = DateTime.UtcNow;
            }




            _context.SaveChanges();
            return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Successfully Bought !" + BuyCoinsv2.CoinToBuy });

        }
    
}
}
