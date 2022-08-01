using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CurrencyExchange.API.Filters
{
    public class NotFoundFilter<T> : IAsyncActionFilter where T : BaseEntity
    {
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext contex, ActionExecutionDelegate next)
        {
            var idValue = contex.ActionArguments.Values.FirstOrDefault();

            if (idValue==null)
            {
                await next.Invoke();
            }

            var id = (int)idValue;
            var anyEntity = await _service.AnyAsync(x=>x.Id==id);

            if (anyEntity)
            {
                await next.Invoke();
            }

            contex.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({id}) not found"));
        }

    }
}
