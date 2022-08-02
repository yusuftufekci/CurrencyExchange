using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CurrencyExchange.API.Filters
{
    public class NotFoundFilter<T> : IAsyncActionFilter where T : User
    {
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string userEmail = context.HttpContext.Request.Query["UserEmail"];

            if (userEmail == null)
            {
            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(401, $"Bad Parameter"));
                return;
            }
           

            var anyEntity = await _service.AnyAsync(x=>x.UserEmail==userEmail);

            if (anyEntity)
            {
                await next.Invoke();
            }

            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({userEmail}) not sssssfound"));
        }

    }
}
