using CurrencyExchange2.Model.Account;
using CurrencyExchange2.Requests;
using CurrencyExchange2.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange2.Controllers.UserController
{
    public class BalanceInquiryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BalanceInquiryController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpPost]
        [Route("UserBalanceInformation")]
        public async Task<ActionResult<List<UserBalanceInformationResponse>>> GetUserBalances([FromBody] GetUserInformation userInfos, [FromHeader] string token)
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
            var balances = await _context.Balances.Where(p => p.Account == userAccount).ToListAsync();
           
            List<UserBalances> userBalancesInfos = new List<UserBalances>();

            foreach (var item in balances)
            {
                UserBalances userBalancesInfo = new UserBalances();
                userBalancesInfo.TotalBalance = item.TotalBalance;
                userBalancesInfo.CoinName = item.CoinName;
                userBalancesInfos.Add(userBalancesInfo);

            }

            return Ok(new UserBalanceInformationResponse { StatusCode = 200, Status = "Success", Message = "Succesfull", UserBalances = userBalancesInfos });

        }
    }
}
