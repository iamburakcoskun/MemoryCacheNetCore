using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace IMemoryCacheDemo.Helpers
{
    public class CustomCacheAttribute : ActionFilterAttribute
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey;
        private readonly TimeSpan _expiration;

        public CustomCacheAttribute(string cacheKey, int expirationInSeconds)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheKey = cacheKey;
            _expiration = TimeSpan.FromSeconds(expirationInSeconds);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_cache.TryGetValue(_cacheKey, out object cachedValue))
            {
                context.Result = new ObjectResult(cachedValue);
                return;
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(_cache.TryGetValue(_cacheKey, out _)))
            {
                //Bu kisimda cache onem derecesi ve suresi gibi ayarlamalar yapilmaktadir.
                var cacheExpOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(_cacheKey, (context.Result as ObjectResult).Value, _expiration);
            }

            base.OnActionExecuted(context);
        }
    }
}
