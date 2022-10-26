using IDistributedCacheRedisApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace IDistributedCacheRedisApp.Controllers
{
    public class ProductsController : Controller
    {

        private readonly IDistributedCache _distributedCache;

        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEnry = new DistributedCacheEntryOptions();

            cacheEnry.AbsoluteExpiration = DateTime.Now.AddMinutes(30);
            Product product = new Product { Id = 3, Name = "kalem3", Price = 200 };

            string jsonproduct = JsonConvert.SerializeObject(product); //json çevirme

            Byte[] byteproduct = Encoding.UTF8.GetBytes(jsonproduct); //byte çevirme

            _distributedCache.Set("product:3", byteproduct);


            //await _distributedCache.SetStringAsync("product:2",jsonproduct,cacheEnry);

            //_distributedCache.SetString("name", "caner");
            //await _distributedCache.SetStringAsync("surname", "güreşci");
            return View();
        }

        public async Task<IActionResult> Show()
        {
            //string name = _distributedCache.GetString("name");
            //string surname = await _distributedCache.GetStringAsync("güreşci");
            //ViewBag.name = name;
            //ViewBag.surname = surname;

            Byte[] byteProduct = _distributedCache.Get("product:3");

            string jsonproducts = Encoding.UTF8.GetString(byteProduct);
            
            //string jsonproducts = _distributedCache.GetString("product:1");

            Product p = JsonConvert.DeserializeObject<Product>(jsonproducts);

            ViewBag.product = p;


            return View();
        }

        public async Task<IActionResult> Remove()
        {
            _distributedCache.Remove("name");
            await _distributedCache.RemoveAsync("surname");
            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] resimbyte = _distributedCache.Get("resim");

            return File(resimbyte, "image/jpg");
        }


        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/download.jpg"); //resimi yolunu verdik
            byte[] imageBye = System.IO.File.ReadAllBytes(path); //daha sonra byte çevirdik

            _distributedCache.Set("resim", imageBye); //image dosyasını binary olarak kaydettik.
            return View();
        }
    }
}
