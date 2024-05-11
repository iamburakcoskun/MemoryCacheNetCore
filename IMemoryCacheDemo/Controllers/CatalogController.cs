using IMemoryCacheDemo.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IMemoryCacheDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        List<Product> productList;

        const string cacheKey = "productKey";
        private readonly IMemoryCache _memoryCache;

        public CatalogController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public class Product
        {
            public string Name { get; set; }

            public decimal Price { get; set; }
        }

        [HttpGet(Name = "GetProducts")]
        [CustomCache("CachedData", 10)]
        public IActionResult GetProducts()
        {
            productList = new List<Product> { new Product { Name = "Bahce", Price = 4000 }, new Product { Name = "Spor", Price = 10000 } };

            return Ok(productList);
        }
    }
}
