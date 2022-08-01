using CurrencyExchange.Repository;
using CurrencyExchange2.Model.Account;
using CurrencyExchange2.Requests;
using CurrencyExchange2.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange2.Controllers.UserController
{
    public class UserInformationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserInformationController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpPost]
        [Route("UserInformation")]
        public async Task<ActionResult<List<UserInformationResponse>>> GetAllUserInformation([FromBody] GetUserInformation userInfos, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == userInfos.UserEmail);
            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });
            int userId = userExist.Id;
            var userAccount = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userId);
            if (userAccount == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });
            var balances = await _context.Balances.Where(p=> p.Account== userAccount).ToListAsync();
            string userEmailAddress = userExist.UserEmail;
            string userAccountName = userAccount.AccountName;
            int accountId = userAccount.Id;
            List<UserBalances> userBalancesInfos = new List<UserBalances>();

            foreach (var item in balances)
            {
                UserBalances userBalancesInfo = new UserBalances();
                userBalancesInfo.TotalBalance = item.TotalBalance;
                userBalancesInfo.CoinName = item.CryptoCoin.CoinName;
                userBalancesInfos.Add(userBalancesInfo);

            }

            return Ok(new UserInformationResponse { StatusCode = 200, Status = "Success", Message = "Succesfull", UserEmail = userEmailAddress, UserAccountName=userAccountName, UserBalances= userBalancesInfos });

        }

        [HttpPost]
        [Route("UserTransactionHistory")]
        public async Task<ActionResult<List<UserInformationResponse>>> GetUserTransactionHistory([FromBody] GetUserInformation userInfos, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == userInfos.UserEmail);
            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });
            int userId = userExist.Id;
            var userAccount = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userId);
            if (userAccount == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });
            string userAccountName = userAccount.AccountName;
            int accountId = userAccount.Id;
            List<UserTransactionHistory> userTransactionHistories = new List<UserTransactionHistory>();
            var transactions = await _context.UserBalanceHistories.Where(p => p.Account == userAccount).ToListAsync();
            foreach (var item in transactions)
            {
                UserTransactionHistory userTransactions = new UserTransactionHistory();
                userTransactions.MessageForChanging = item.MessageForChanging;
                userTransactions.AccountName = userAccountName;
                userTransactions.ChangedAmount = item.ChangedAmount;
                userTransactions.ExchangedCoinName = item.ExchangedCoinName;

                userTransactionHistories.Add(userTransactions);
            }



            return Ok(new UserTransactionResponse{ StatusCode = 200, Status = "Success", Message = "Succesfull", UserTransactions=userTransactionHistories });

        }

    }
}
