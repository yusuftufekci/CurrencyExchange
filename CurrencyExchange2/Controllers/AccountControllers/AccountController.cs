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
        public static Account tempAccount = new Account();
        public static Balance balance = new Balance();

        public readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;

        }
        // GET: AccountController
        [HttpGet]
        public async Task<ActionResult<List<Account>>> Get()
        {
            return Ok(await _context.Accounts.ToListAsync());

        }

        [HttpPost]
        [Route("CreateAccount")]
        public async Task<ActionResult<List<Account>>> CreateAccount([FromBody] CreateAccount account, [FromHeader] string token)
        {
            var tokenExist = await _context.UserTokens.SingleOrDefaultAsync(p => p.Token == token);
            if (tokenExist == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Invalid Token!" });
            }
            var userExists = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == account.UserEmail);
            if (userExists == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Invalid Mail Adress!" });

            if ( await _context.Accounts.SingleOrDefaultAsync(p => p.UserId == userExists.UserId) == null){
                tempAccount.AccountName = account.AccountName;
                tempAccount.UserId = userExists.UserId;
                balance.UserId = userExists.UserId;
                tempAccount.BalanceId = balance.UserId;

                _context.Accounts.Add(tempAccount);
                _context.Balances.Add(balance);

                await _context.SaveChangesAsync();

                return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Account created successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Already Have An Account" });
            }

        }
    }
}
