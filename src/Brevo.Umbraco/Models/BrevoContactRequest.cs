using System.Text.Json.Serialization;

namespace Brevo.Umbraco.Models;

public sealed class BrevoContactRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = [];

    [JsonPropertyName("listIds")]
    public List<int> ListIds { get; set; } = [];

    [JsonPropertyName("updateEnabled")]
    public bool UpdateEnabled { get; set; };
}
