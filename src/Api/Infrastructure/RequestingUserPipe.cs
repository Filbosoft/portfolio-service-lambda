using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Business;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Api.Infrastructure
 {
    public class RequestingUserPipe<TIn, TOut> : IPipelineBehavior<TIn, TOut>
    {
        private readonly HttpContext _httpContext;
        public RequestingUserPipe(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }
        public async Task<TOut> Handle(TIn request, CancellationToken cancellationToken, RequestHandlerDelegate<TOut> next)
        {
            // var userId = _httpContext.User.Claims
            //     .FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))
            //     .Value;

            if (request is BusinessRequest br) br.RequestingUserId = 1;

            return await next();
        }
    }
}