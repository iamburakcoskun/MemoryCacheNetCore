using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace IMemoryCacheDemo.Helpers
{
    public class CustomCacheAttribute : ActionFilterAttribute
    {
        ActionDescriptor actionDescriptor;
        private string _cacheKey;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expiration;

        public CustomCacheAttribute(int expirationInSeconds)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _expiration = TimeSpan.FromSeconds(expirationInSeconds);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            actionDescriptor = context.ActionDescriptor;

            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                _cacheKey = controllerActionDescriptor.ActionName;
            }

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
                actionDescriptor = context.ActionDescriptor;

                if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    _cacheKey = controllerActionDescriptor.ActionName;
                }

                _cache.Set(_cacheKey, (context.Result as ObjectResult).Value, _expiration);
            }

            base.OnActionExecuted(context);
        }
    }
}
