using Brevo.Umbraco.Models;

namespace Brevo.Umbraco.Services;

public interface IBrevoService
{
    Task CreateOrUpdateContactAsync(BrevoContactRequest request, CancellationToken ct = default);
}
