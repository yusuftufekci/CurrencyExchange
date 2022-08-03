using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CurrencyExchange.API.Filters
{
    public class TokenControlFilter<T> : IAsyncActionFilter where T : UserToken
    {
        private readonly IService<T> _service;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenControlFilter(IService<T> service, ITokenRepository tokenRepository, IUserRepository userRepository)
        {
            _service = service;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string token = context.HttpContext.Request.Headers["token"];
            var userEmail = context.HttpContext.Request.Query.ToList();


            if (token == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(401, $"Bad Parameter"));
                return;
            }
            var userToken = _tokenRepository.Where(x => x.Token == token).SingleOrDefault();

            if (userToken != null)
            {
                //var anyEntity = await _service.AnyAsync(x => x.Token == token);
                var userExist = _userRepository.Where(x => x.Id == userToken.UserId).SingleOrDefault();

                if (userExist != null)
                {
                    var value = userEmail.Find(item => item.Key == "UserEmail").Value;
                    if (userExist.UserEmail == value)
                    {
                        await next.Invoke();

                    }
                    context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));

                }

                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
            }
        }

    }
}

