using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange2.Model.Crypto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CurrencyExchange2.Responses;
using CurrencyExchange.Repository;

namespace CurrencyExchange2.Controllers.CoinControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinController : ControllerBase
    {
  

        private readonly ApplicationDbContext _context;

        public CoinController(ApplicationDbContext context)
        {
            _context = context;

        }

        //[HttpGet]
        //[Route("CoinPrices")]
        //public async Task<ActionResult<List<Response>>> CoinPrices()
        //{
        //    var coinPrice = new CoinPrice();

        //    using (var client = new HttpClient())
        //    {
        //        client.Timeout = TimeSpan.FromMinutes(1); 
        //        HttpResponseMessage response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var ResponceString = await response.Content.ReadAsStringAsync();


        //            var ResponceObject = JsonConvert.DeserializeObject<List<CoinPrice>>(ResponceString);
        //            if (_context.CryptoCoinPrices.Count() == 0)
        //            {
        //                foreach (var item in ResponceObject)
        //                {
        //                   // _context.CryptoCoinPrices.Add(item);
        //                }
        //                await _context.SaveChangesAsync();
        //                return Ok(new Response { StatusCode = 201, Status = "Success", Message = "Coin List successfully added!" });
        //            }
        //            else
        //            {
        //                foreach (var item in ResponceObject)
        //                {
                            
        //                        coinPrice = await _context.CryptoCoinPrices.SingleOrDefaultAsync(p => p.symbol == item.symbol);
        //                        if (coinPrice == null)
        //                        {
        //                            coinPrice.price = item.price;
        //                            coinPrice.ModifiedDate = DateTime.UtcNow;
        //                        }     
                                
        //                }
        //                await _context.SaveChangesAsync();
        //                return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Coin List successfully added!" });

        //            }      
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 500 , Status = "Error", Message = "Can't Reach api" });

        //        }
        //    }
            

        //}
        //[HttpGet]
        //[Route("CoinList")]

        //public async Task<ActionResult<List<Response>>> LoadCoin()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.Timeout = TimeSpan.FromMinutes(1);
        //        HttpResponseMessage response = await client.GetAsync("https://api.nomics.com/v1/currencies/ticker?key=ab9543f3c307afc219e1b55e9527559447536691");
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var ResponceString = await response.Content.ReadAsStringAsync();
        //            var root = (JContainer)JToken.Parse(ResponceString);
        //            var list = root.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "id").Select(p => p.Value.Value<string>());
        //            if (_context.CoinTypes.Count() == 0)
        //            {
        //                foreach (var item in list)
        //                {
        //                    var tempCoinType = new CoinType();

        //                    tempCoinType.CoinName = item;
        //                    _context.CoinTypes.Add(tempCoinType);
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in list)
        //                {

        //                    var coinExist = await _context.CoinTypes.SingleOrDefaultAsync(p => p.CoinName == item);
        //                    if (coinExist == null)
        //                    {
        //                        var tempCoinType = new CoinType();
                                
        //                        tempCoinType.CoinName = item;
        //                        _context.CoinTypes.Add(tempCoinType);
        //                    }
        //                }
        //            }
                    
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 500, Status = "Error", Message = "Can't Reach api" });

        //        }
               
        //    }
        //    await _context.SaveChangesAsync();
        //    return Ok(new Response { StatusCode = 200, Status = "Success", Message = "Coin List successfully added!" });
        //}

    }
}
