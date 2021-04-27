using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure
{
    public static class PipelineRegistration
    {
        public static IServiceCollection ConfigureMediatRPipeline(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestTrackingPipe<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestingUserPipe<,>));

            return services;
        }
    }
}