using Brevo.Umbraco.Helpers;
using Brevo.Umbraco.Models;
using Brevo.Umbraco.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Brevo.Umbraco.Workflows;

public sealed class SendToBrevoWorkflow : WorkflowType
{
    private readonly IBrevoService _brevoService;
    private readonly ILogger<SendToBrevoWorkflow> _logger;

    [Setting("Email Field Alias",
        Description = "The alias of the form field that contains the email address. Find it in the Forms editor under each field's settings. Example: email",
        IsMandatory = true)]
    public string EmailField { get; set; } = string.Empty;

    [Setting("Field Mappings",
        Description = "Map Brevo contact attributes to form fields, one per line. Format: BREVO_ATTRIBUTE:formFieldAlias — Example: FIRSTNAME:firstName | LASTNAME:lastName | PHONE:mobileNumber — BREVO_ATTRIBUTE must match exactly the attribute name in your Brevo account (Contacts → Settings → Contact attributes).")]
    public string FieldMapping { get; set; } = string.Empty;

    [Setting("List IDs",
        Description = "Comma-separated numeric Brevo list IDs. The contact will be added to each list. Find IDs in Brevo under Contacts → Lists. Optional — leave empty to create the contact without list assignment. Example: 3,7,12")]
    public string ListIds { get; set; } = string.Empty;

    public SendToBrevoWorkflow(IBrevoService brevoService, ILogger<SendToBrevoWorkflow> logger)
    {
        _brevoService = brevoService;
        _logger = logger;

        Id = new Guid("e3f7a1b2-4c5d-4e6f-9a0b-1c2d3e4f5a6b");
        Name = "Send to Brevo";
        Description = "Adds or updates a Brevo contact from a form submission.";
        Icon = "icon-paper-plane";
        Group = "Services";
    }

    public override List<Exception> ValidateSettings()
    {
        var errors = new List<Exception>();

        if (string.IsNullOrWhiteSpace(EmailField))
            errors.Add(new Exception("Email Field Alias is required."));

        return errors;
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        var record = context.Record;

        var email = GetFieldValue(record, EmailField);
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning(
                "SendToBrevoWorkflow: email field '{EmailField}' is empty or missing on record {RecordId}",
                EmailField, record.Id);
            return WorkflowExecutionStatus.Failed;
        }

        var attributes = new Dictionary<string, object>();
        foreach (var mapping in BrevoFieldMappingParser.Parse(FieldMapping, _logger))
        {
            var value = GetFieldValue(record, mapping.FormFieldAlias);
            if (!string.IsNullOrEmpty(value))
                attributes[mapping.BrevoAttributeName] = value;
        }

        var request = new BrevoContactRequest
        {
            Email = email,
            Attributes = attributes,
            ListIds = ParseListIds(record.Id),
        };

        try
        {
            await _brevoService.CreateOrUpdateContactAsync(request);
            return WorkflowExecutionStatus.Completed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SendToBrevoWorkflow: failed to create/update Brevo contact {Email} on record {RecordId}",
                email, record.Id);
            return WorkflowExecutionStatus.Failed;
        }
    }

    private List<int> ParseListIds(int recordId)
    {
        var result = new List<int>();

        if (string.IsNullOrWhiteSpace(ListIds))
            return result;

        foreach (var part in ListIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(part, out var id))
                result.Add(id);
            else
                _logger.LogWarning(
                    "SendToBrevoWorkflow: invalid list ID '{Part}' on record {RecordId} — skipping",
                    part, recordId);
        }

        return result;
    }

    private static string? GetFieldValue(Record record, string alias)
    {
        var field = record.RecordFields.Values
            .FirstOrDefault(f => f.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));

        if (field is null || field.Values.Count == 0)
            return null;

        var joined = string.Join(", ", field.Values
            .Where(v => v is not null)
            .Select(v => v!.ToString())
            .Where(s => !string.IsNullOrEmpty(s)));

        return string.IsNullOrEmpty(joined) ? null : joined;
    }
}
