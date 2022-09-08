using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.HelperFunctions;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CurrencyExchange.API.Filters
{
    public class TokenControlFilter<T> : IAsyncActionFilter where T : UserToken
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TokenControlFilter(ITokenRepository tokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string token = context.HttpContext.Request.Headers["token"];
            if (token == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized,TokenMessages.MissingToken));
                return;
            }

            string userEmail = CreateToken.ParseToken(token);
            if (userEmail == null)
            {
                context.Result =
                    new NotFoundObjectResult(
                        CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized, $"{typeof(T).Name} " + TokenMessages.TokenInvalid ));
                return;
            }

            if (userEmail == TokenMessages.TokenExpired)
            {
                var updateToken = _tokenRepository.Where(x => x.Token == token).SingleOrDefault();
                updateToken.IsActive = false;
                await _unitOfWork.CommitAsync();
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized, $"{typeof(T).Name} " + TokenMessages.TokenExpired));
                return;
            }


            var userToken = _tokenRepository.Where(x => x.Token == token).SingleOrDefault();

            if (userToken == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized, $"{typeof(T).Name} " + TokenMessages.TokenInvalid));
                return;

            }


            var userExist = _userRepository.Where(x => x.Id == userToken.UserId).SingleOrDefault();

            if (userExist == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized, $"{typeof(T).Name} " + TokenMessages.TokenInvalid));
                return;
            }

            if (userExist.UserEmail == userEmail)
            {
                await next.Invoke();
                return;

            }

            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.Unauthorized, $"{typeof(T).Name} " + TokenMessages.TokenInvalid));
            return;






        }

    }

}

