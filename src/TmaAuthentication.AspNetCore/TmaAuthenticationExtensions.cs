using Microsoft.AspNetCore.Authentication;
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

        return services.AddAuthentication()
            .AddScheme<TmaAuthenticationOptions, TmaAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }
}