using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TmaAuth;
using TmaAuth.Abstractions;

namespace TmaAuthentication.AspNetCore;

public static class TmaAuthenticationExtensions
{
    public static AuthenticationBuilder AddTmaAuthentication(this IServiceCollection services)
        => services.AddTmaAuthentication(TmaAuthenticationDefaults.AuthenticationScheme, _ => { });

    public static AuthenticationBuilder AddTmaAuthentication(
        this IServiceCollection services,
        Action<TmaAuthenticationOptions> configureOptions)
        => services.AddTmaAuthentication(TmaAuthenticationDefaults.AuthenticationScheme, configureOptions);

    public static AuthenticationBuilder AddTmaAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        Action<TmaAuthenticationOptions> configureOptions)
        => services.AddTmaAuthentication(authenticationScheme, displayName: null, configureOptions: configureOptions);

    public static AuthenticationBuilder AddTmaAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        string? displayName,
        Action<TmaAuthenticationOptions> configureOptions)
    {
        services.TryAddSingleton<ITmaInitDataValidator, TmaInitDataValidator>();
        services.TryAddSingleton<ITmaInitDataParser, TmaInitDataParser>();
        services.TryAddSingleton<ITmaInitDataSigner, TmaInitDataSigner>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddScoped<IUserAccessor, UserAccessor>();

        return services.AddAuthentication()
            .AddScheme<TmaAuthenticationOptions, TmaAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this IServiceCollection services,
        Action<TmaJwtOptions> configureOptions)
        => services.AddTmaJwtAuthentication(TmaJwtDefaults.AuthenticationScheme, configureOptions);

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        Action<TmaJwtOptions> configureOptions)
        => services.AddTmaJwtAuthentication(authenticationScheme, displayName: null, configureOptions: configureOptions);

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        string? displayName,
        Action<TmaJwtOptions> configureOptions)
    {
        services.TryAddSingleton<ITmaInitDataValidator, TmaInitDataValidator>();
        services.TryAddSingleton<ITmaInitDataParser, TmaInitDataParser>();
        services.TryAddSingleton<ITmaInitDataSigner, TmaInitDataSigner>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddScoped<IUserAccessor, UserAccessor>();
        services.TryAddScoped<ITmaJwtService, TmaJwtService>();

        var authBuilder = services.AddAuthentication()
            .AddScheme<TmaJwtOptions, TmaJwtAuthenticationHandler>(authenticationScheme, displayName, configureOptions);

        services.Configure<TmaJwtOptions>(authenticationScheme, options =>
        {
            configureOptions(options);
            
            if (options.EnableBuiltInEndpoint)
            {
                services.AddControllers()
                    .AddApplicationPart(typeof(TmaTokenController).Assembly);
            }
        });

        return authBuilder;
    }

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this AuthenticationBuilder builder,
        Action<TmaJwtOptions> configureOptions)
        => builder.AddTmaJwtAuthentication(TmaJwtDefaults.AuthenticationScheme, configureOptions);

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<TmaJwtOptions> configureOptions)
        => builder.AddTmaJwtAuthentication(authenticationScheme, displayName: null, configureOptions: configureOptions);

    public static AuthenticationBuilder AddTmaJwtAuthentication(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<TmaJwtOptions> configureOptions)
    {
        builder.Services.TryAddSingleton<ITmaInitDataValidator, TmaInitDataValidator>();
        builder.Services.TryAddSingleton<ITmaInitDataParser, TmaInitDataParser>();
        builder.Services.TryAddSingleton<ITmaInitDataSigner, TmaInitDataSigner>();
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.TryAddScoped<IUserAccessor, UserAccessor>();
        builder.Services.TryAddScoped<ITmaJwtService, TmaJwtService>();

        builder.Services.Configure<TmaJwtOptions>(authenticationScheme, options =>
        {
            configureOptions(options);
            
            if (options.EnableBuiltInEndpoint)
            {
                builder.Services.AddControllers()
                    .AddApplicationPart(typeof(TmaTokenController).Assembly);
            }
        });

        return builder.AddScheme<TmaJwtOptions, TmaJwtAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }
}