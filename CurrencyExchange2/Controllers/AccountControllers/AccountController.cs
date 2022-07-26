using CurrencyExchange2.Model.Account;
using CurrencyExchange2.Requests;
using CurrencyExchange2.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange2.Controllers.AccountControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        public readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpPost]
        [Route("CreateAccount")]
        public async Task<ActionResult<List<Response>>> CreateAccount([FromBody] CreateAccount account, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExists = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == account.UserEmail);
            if (userExists == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Invalid Mail Adress!" });

            if ( await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExists.UserId) == null){
                Account tempAccount = new Account();

                tempAccount.AccountName = account.AccountName;
                tempAccount.UserId = userExists.UserId;
                _context.Accounts.Add(tempAccount);

                await _context.SaveChangesAsync();

                return Ok(new Response { StatusCode = 201, Status = "Success", Message = "Account created successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 409, Status = "Error", Message = "Already Have An Account" });
            }

        }

        [HttpPost]
        [Route("DepositFunds")]
        public async Task<ActionResult<List<Response>>> DepositFunds([FromBody] DepositFunds depositFund, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }

            var userExists = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == depositFund.UserEmail);
            var accountExist = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExists.UserId);
            if (userExists == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account doesnt exist" }); 

            var balanceExist = await _context.Balances.SingleOrDefaultAsync(p => p.Account == accountExist && p.CoinName=="USDT");
            if (balanceExist != null)
            {
                balanceExist.TotalBalance += depositFund.TotalBalance;
                balanceExist.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Deposit Funded successfully!" });

            }
            Balance tempBalance = new Balance();
            UserBalanceHistory tempUserBalanceHistory = new UserBalanceHistory();
            tempUserBalanceHistory.Account = accountExist;
            tempUserBalanceHistory.MessageForChanging = depositFund.TotalBalance + " USDT deposit into the account";
            tempUserBalanceHistory.ChangedAmount = depositFund.TotalBalance;
            tempUserBalanceHistory.ExchangedCoinName = "USDT";
            tempBalance.CoinName = "USDT";
            tempBalance.Account = accountExist;
            tempBalance.TotalBalance = depositFund.TotalBalance;
            tempBalance.CoinId = 3;
            _context.Balances.Add(tempBalance);
            _context.UserBalanceHistories.Add(tempUserBalanceHistory);

            await _context.SaveChangesAsync();
            return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Deposit Funded successfully!" });

        }



    }
}
