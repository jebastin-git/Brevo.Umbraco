using Brevo.Umbraco.Models;
using Brevo.Umbraco.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;

namespace Brevo.Umbraco.Extensions;

public static class BrevoUmbracoBuilderExtensions
{
    private const string BrevoBaseUrl = "https://api.brevo.com/v3/";

    public static IUmbracoBuilder AddBrevo(this IUmbracoBuilder builder)
    {
        builder.Services.Configure<BrevoSettings>(
            builder.Config.GetSection("Brevo"));

        builder.Services.AddHttpClient<IBrevoService, BrevoService>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<BrevoSettings>>().Value;
            client.BaseAddress = new Uri(BrevoBaseUrl);
            client.DefaultRequestHeaders.Add("api-key", settings.ApiKey);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });

        return builder;
    }
}
