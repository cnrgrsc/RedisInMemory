using InMemory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Controllers
{
    public class ProductController : Controller
    {

        private readonly IMemoryCache _cache;

        public ProductController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {

            //1.yol
            //if (String.IsNullOrEmpty(_cache.Get<string>("zaman")))
            //{
            //    _cache.Set<string>("zaman", DateTime.Now.ToString());

            //}
            //out keyword bir mettota birden fazla değer dönebilmek için.
            //2.yol
            //if (!_cache.TryGetValue("zaman", out string zamancache))
            //{
            //} //bu method ile gelen zaman keyini al ve zamancache ata dedik 


            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10); //adminites
            //options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High;
            options.RegisterPostEvictionCallback((key, value, reason, state) => //bir data silindiği zaman artık hangi keyivalue ve sebep ile silindiğini görebiliriz.
            {
                _cache.Set("callback", $"{key} -->{value} => sebep:{reason}");
            });
            _cache.Set<string>("zaman", DateTime.Now.ToString(), options);
            Product p = new Product { Id = 1, Name = "kalem", Price=200 };
            _cache.Set<Product>("product:1",p);

            return View();
        }
        //AbsoluteExpiration bu method cache verdiğimiz zamanı alır. Cach oluşturulduğunda 5 dk verirsek 5 dk sonracahce siler

        //SlidingExpiration 3 dk verdik süre oalrak 3 dk içinde dataya erişemez ise silinir eğerki ulaşırsa o zaman 3 dk uzatır. slidenn verilince mutlaka absolute verilir. yokise bayat veriye ulaşam durumu olur.
        public IActionResult Show()
        {
            //bu method ile zaman diye bir kyword varmı diye kontrol et yok ise git o zaman aç ve kaydet dedik. bu kullanımın amacıda entry.diyip bunun özeeliklerini vermek içinç
            //_cache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});

            _cache.TryGetValue("zaman", out string zamancache);
            _cache.TryGetValue("callback", out string callback);
            ViewBag.zaman = zamancache;
            ViewBag.callback = callback;
            ViewBag.product = _cache.Get<Product>("product:1");
            /*ViewBag.zaman = _cache.Get<string>("zaman")*/
            ; //Viewbag ile veriyi view gönderdik
            return View();

        }

    }
}
