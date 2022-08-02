using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CurrencyExchange.API.Filters
{
    public class TokenControlFilter<T> : IAsyncActionFilter where T : UserToken
    {
        private readonly IService<T> _service;

        public TokenControlFilter(IService<T> service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string token = context.HttpContext.Request.Headers["token"];

            if (token == null)
            {
                context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(401, $"Bad Parameter"));
                return;
            }


            var anyEntity = await _service.AnyAsync(x => x.Token == token);

            if (anyEntity)
            {
                await next.Invoke();
            }

            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({token}) unaccapted token"));
        }

    }
}

