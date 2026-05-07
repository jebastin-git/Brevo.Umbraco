using System.Text.Json.Serialization;

namespace Brevo.Umbraco.Models;

public sealed class BrevoDoubleOptInContactRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = [];

    [JsonPropertyName("includeListIds")]
    public List<long> IncludeListIds { get; set; } = [];

    [JsonPropertyName("excludeListIds")]
    public List<long> ExcludeListIds { get; set; } = [];

    [JsonPropertyName("redirectionUrl")]
    public string RedirectionUrl { get; set; } = string.Empty;

    [JsonPropertyName("templateId")]
    public long TemplateId { get; set; }
}
