using System.Net.Http.Json;
using System.Text.Json;
using Brevo.Umbraco.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brevo.Umbraco.Services;

public sealed class BrevoService : IBrevoService
{
    // DictionaryKeyPolicy = null ensures BREVO attribute names (e.g. FIRSTNAME) are never transformed
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        DictionaryKeyPolicy = null,
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<BrevoService> _logger;
    private readonly BrevoSettings _settings;

    public BrevoService(HttpClient httpClient, IOptions<BrevoSettings> options, ILogger<BrevoService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task CreateOrUpdateContactAsync(BrevoContactRequest request, CancellationToken ct = default)
    {
        _logger.LogTrace("Sending contact to Brevo {Email} ({AttributeCount} attributes, {ListCount} lists, UpdateEnabled: {UpdateEnabled})",
            request.Email, request.Attributes.Count, request.ListIds.Count, request.UpdateEnabled);

        using var message = new HttpRequestMessage(HttpMethod.Post, "contacts");
        message.Headers.Add("api-key", _settings.ApiKey);
        message.Headers.Accept.ParseAdd("application/json");
        message.Content = JsonContent.Create(request, options: SerializerOptions);

        using var response = await _httpClient.SendAsync(message, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);

            _logger.LogError("Brevo API rejected contact {Email} [{StatusCode}]: {ResponseBody}",
                request.Email, (int)response.StatusCode, body);

            throw new HttpRequestException(
                $"Brevo API returned {(int)response.StatusCode} for {request.Email}: {body}",
                inner: null,
                statusCode: response.StatusCode);
        }

        _logger.LogInformation("Brevo contact created/updated {Email} [{StatusCode}]",
            request.Email, (int)response.StatusCode);
    }
}
