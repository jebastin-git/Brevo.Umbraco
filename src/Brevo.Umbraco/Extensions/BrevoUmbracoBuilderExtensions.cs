using Brevo.Umbraco.Models;
using Brevo.Umbraco.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Brevo.Umbraco.Extensions;

public static class BrevoUmbracoBuilderExtensions
{
    private const string BrevoBaseUrl = "https://api.brevo.com/v3/";

    public static IUmbracoBuilder AddBrevo(this IUmbracoBuilder builder)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IBrevoService)))
            return builder;

        builder.Services.Configure<BrevoSettings>(
            builder.Config.GetSection("Brevo"));

        builder.Services.AddHttpClient<IBrevoService, BrevoService>(client =>
        {
            client.BaseAddress = new Uri(BrevoBaseUrl);
        });

        return builder;
    }
}
