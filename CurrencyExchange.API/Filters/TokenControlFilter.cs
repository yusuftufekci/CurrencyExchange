using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CurrencyExchange.API.Filters
{
    public class TokenControlFilter<T> : IAsyncActionFilter where T : UserToken
    {
        private readonly IService<T> _service;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;

        public TokenControlFilter(IService<T> service, ITokenRepository tokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _service = service;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _UnitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string token = context.HttpContext.Request.Headers["token"];
            if (token == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(401, $"Bad Parameter"));
                return;
            }
            string userEmail = CreateToken.ParseToken(token);
            if (userEmail == null)
            {
                context.Result =
                    new NotFoundObjectResult(
                        CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
                return;
            }
            else if (userEmail == "Token expired")
            {
                var updateToken = _tokenRepository.Where(x => x.Token == token).SingleOrDefault();
                updateToken.IsActive = false;
                await _UnitOfWork.CommitAsync();
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) token expired"));
                return;

            }


            var userToken = _tokenRepository.Where(x => x.Token == token).SingleOrDefault();
            
            if (userToken != null)
            {
                //var anyEntity = await _service.AnyAsync(x => x.Token == token);
                var userExist = _userRepository.Where(x => x.Id == userToken.UserId).SingleOrDefault();

                if (userExist != null)
                {
                    if (userExist.UserEmail == userEmail)
                    {
                        await next.Invoke();
                        return;

                    }
                    context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
                    return;


                }

                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
                return;

            }
            else
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
                return;

            }
        }

    }
}

