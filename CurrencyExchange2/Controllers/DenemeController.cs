//using CurrencyExchange.Core.Entities.Authentication;
//using CurrencyExchange.Core.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using CurrencyExchange.Core.DTOs;
//namespace CurrencyExchange.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DenemeController : CustomBaseController
//    {
//        private readonly IService<User> _service;

//        public DenemeController(IService<User> service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> All()
//        {
//            var users = await _service.GetAllAsync();
//            // return Ok(CustomResponseDto<List<User>>.Succes(200, users.ToList()));
//            return CreateActionResult(CustomResponseDto<List<User>>.Succes(200,users.ToList()));
//        }
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var users = await _service.GetByIdAsync(id);
//            // return Ok(CustomResponseDto<List<User>>.Succes(200, users.ToList()));
//            return CreateActionResult(CustomResponseDto<User>.Succes(200, users));
//        }

//        [HttpPut]
//        public async Task<IActionResult> Update(User user)
//        {
//            await _service.UpdateAsync(user);
//            // return Ok(CustomResponseDto<List<User>>.Succes(200, users.ToList()));
//            return CreateActionResult(CustomResponseDto<NoContentDto>.Succes(204));
//        }

//    }
//}
