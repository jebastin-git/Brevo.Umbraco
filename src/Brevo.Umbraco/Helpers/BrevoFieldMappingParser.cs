using Brevo.Umbraco.Models;
using Microsoft.Extensions.Logging;

namespace Brevo.Umbraco.Helpers;

public static class BrevoFieldMappingParser
{
    public static IEnumerable<BrevoFieldMapping> Parse(string? fieldMapping, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(fieldMapping))
            yield break;

        foreach (var line in fieldMapping.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var sep = line.IndexOf(':');

            if (sep <= 0 || sep == line.Length - 1)
            {
                logger.LogWarning("SendToBrevoWorkflow: skipping invalid mapping line '{Line}'", line);
                continue;
            }

            yield return new BrevoFieldMapping(
                BrevoAttributeName: line[..sep].Trim(),
                FormFieldAlias: line[(sep + 1)..].Trim());
        }
    }
}
