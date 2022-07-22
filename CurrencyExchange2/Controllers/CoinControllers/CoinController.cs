using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange2.Data;
using CurrencyExchange2.Model.Crypto;
using Newtonsoft.Json;

namespace CurrencyExchange2.Controllers.CoinControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinController : ControllerBase
    {
        CryptoCoin val = new CryptoCoin();
       
        private readonly ApplicationDbContext _context;

        public CoinController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<List<CryptoCoin>>> Get()
        {
            var coinPrice = new CryptoCoin();

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(1);
                HttpResponseMessage response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ResponceString = await response.Content.ReadAsStringAsync();


                    var ResponceObject = JsonConvert.DeserializeObject<List<CryptoCoin>>(ResponceString);
                    if (_context.CryptoCoins.Count() == 0)
                    {
                        foreach (var item in ResponceObject)
                        {
                            if (item.symbol == "DOGEUSDT" || item.symbol == "ARPAUSDT" || item.symbol == "ICPUSDT" || item.symbol == "ETHUSDT" || item.symbol == "BTCUSDT")
                            _context.CryptoCoins.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in ResponceObject)
                        {
                            if (item.symbol == "DOGEUSDT" || item.symbol == "ARPAUSDT" || item.symbol == "ICPUSDT" || item.symbol == "ETHUSDT" || item.symbol == "BTCUSDT")
                            {
                                coinPrice = await _context.CryptoCoins.SingleOrDefaultAsync(p => p.symbol == item.symbol);
                                if (coinPrice == null)
                                {
                                    coinPrice.price = item.price;
                                    coinPrice.ModifiedDate = DateTime.UtcNow;
                                }



                                }
                                
                        }

                    }

                    
                }
            }
            await _context.SaveChangesAsync();
            return Ok();

        }






        //// GET: Coin/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.CoinPrice == null)
        //    {
        //        return NotFound();
        //    }

        //    var coinPrice = await _context.CoinPrice
        //        .FirstOrDefaultAsync(m => m.CoinId == id);
        //    if (coinPrice == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(coinPrice);
        //}

        //// GET: Coin/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

    }
}
