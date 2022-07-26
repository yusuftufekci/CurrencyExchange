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
        public async Task<ActionResult<List<Response>>> GetAllUserInformation([FromBody] GetUserInformation userInfos, [FromHeader] string token)
        {
            if (await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 401, Status = "Error", Message = "Invalid Token!" });
            }
            var userExist = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == userInfos.UserEmail);
            if (userExist == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "User doesnt exist" });
            int userId = userExist.UserId;
            var userAccount = await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userId);
            if (userAccount == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 404, Status = "Error", Message = "Account Doesnt Exist" });
            var balances = await _context.Balances.Where(p=> p.Account== userAccount).ToListAsync();
            string userEmailAddress = userExist.UserEmail;
            string userAccountName = userAccount.AccountName;
            int accountId = userAccount.AccountId;
            List<UserBalancesInfo> userBalancesInfos = new List<UserBalancesInfo>();

            foreach (var item in balances)
            {
                UserBalancesInfo userBalancesInfo = new UserBalancesInfo();
                userBalancesInfo.TotalBalance = item.TotalBalance;
                userBalancesInfo.CoinName = item.CoinName;
                userBalancesInfos.Add(userBalancesInfo);

            }

            return Ok(new UserInformation { StatusCode = 200, Status = "Success", Message = "Succesfull", UserEmail = userEmailAddress, UserAccountName=userAccountName, UserBalances= userBalancesInfos });

        }
    }
}
