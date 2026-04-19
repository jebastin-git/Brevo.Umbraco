# Brevo for Umbraco Forms

An Umbraco Forms workflow that creates or updates [Brevo](https://www.brevo.com) (formerly Sendinblue) contacts on form submission.

**Supports:** field attribute mapping · list assignment · multi-value fields · structured logging

---

## Requirements

- Umbraco 17+
- Umbraco Forms 17+
- .NET 10

---

## Installation

```bash
dotnet add package Brevo.Umbraco
```

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .AddBrevo()   // ← add this
    .Build();
```

---

## Configuration

Add your Brevo API key to `appsettings.json`:

```json
{
  "Brevo": {
    "ApiKey": "xkeysib-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
  }
}
```

Use environment-specific overrides (`appsettings.Production.json`) or an environment variable to keep the key out of source control:

```
Brevo__ApiKey=xkeysib-...
```

---

## Workflow Setup

1. Open the Umbraco back-office and navigate to **Forms**
2. Edit a form and go to the **Workflows** tab
3. Click **Add workflow** → select **Send to Brevo**
4. Configure the three settings:

| Setting | Description |
|---|---|
| **Email Field Alias** | The alias of the form field that holds the email address. Example: `email` |
| **Field Mappings** | One mapping per line (see below) |
| **List IDs** | Comma-separated Brevo list IDs to subscribe the contact to (optional) |

---

## Field Mapping

Map Brevo contact attributes to form field values using the format:

```
BREVO_ATTRIBUTE:formFieldAlias
```

Each line is one mapping. Lines that cannot be parsed are skipped with a log warning.

**Example:**

```
FIRSTNAME:firstName
LASTNAME:lastName
PHONE:mobileNumber
COMPANY:companyName
```

- `BREVO_ATTRIBUTE` — the Brevo contact attribute name (case-sensitive, as configured in your Brevo account)
- `formFieldAlias` — the alias of the Umbraco Forms field

Multi-value fields (checkboxes, multi-select) are joined as a comma-separated string.

---

## List IDs

To add the contact to one or more Brevo lists, enter the numeric list IDs in the **List IDs** setting:

```
3
```

```
3, 7, 12
```

List IDs are found in your Brevo account under **Contacts → Lists**. The workflow uses Brevo's create-or-update endpoint, so existing contacts will have their list membership updated.

---

## Logging

The workflow logs at these levels:

| Level | Event |
|---|---|
| `Trace` | Before each API call (email, attribute count, list count) |
| `Information` | Successful contact creation / update |
| `Warning` | Missing email field · invalid mapping line · invalid list ID |
| `Error` | API rejection with status code and Brevo error body |

Workflow log entries (warning and error) include the form record ID for traceability. Service-level logs (trace and information) include email, attribute count, and list count.

---

## Versioning

This package follows **Semantic Versioning**:

| Change | Version bump |
|---|---|
| Bug fixes, logging improvements | Patch (`1.0.x`) |
| New settings, new optional behaviour | Minor (`1.x.0`) |
| Breaking config or API changes | Major (`x.0.0`) |

Umbraco version compatibility is tracked per minor release.

---

## Common Issues

**Contact not added to a list**
Check that **List IDs** is set in the workflow and that the IDs are numeric and match lists in your Brevo account under *Contacts → Lists*. Non-numeric values are silently skipped with a log warning.

**No contact created after form submission**
Check that **Email Field Alias** exactly matches the alias of your email field (visible in the Forms editor under *Fields → [field] → Alias*). A mismatch produces a `Warning` log entry with the configured alias and record ID.

**Attributes not appearing on the Brevo contact**
Check that each `FieldMapping` line follows the format `BREVO_ATTRIBUTE:formFieldAlias` exactly. The Brevo attribute name is case-sensitive and must match what is configured in *Brevo → Contacts → Settings → Contact attributes*. Invalid lines produce a `Warning` log entry showing the offending line.

**API returning 400**
Check the Umbraco logs for an `Error` entry — it includes the full Brevo error body (e.g. invalid email format, unknown attribute name, list not found). Resolve the issue indicated there.

---

## License

MIT
