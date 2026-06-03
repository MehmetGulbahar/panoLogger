using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PanoLogger.Application.Common.Behaviors;
using PanoLogger.Application.Common.Clock;
using PanoLogger.Application.Common.Mapping;

namespace PanoLogger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddSingleton<IConfigurationProvider>(provider => new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<ApplicationMappingProfile>();
        }, provider.GetRequiredService<ILoggerFactory>()));
        services.AddSingleton(provider => provider.GetRequiredService<IConfigurationProvider>().CreateMapper());

        return services;
    }
}
