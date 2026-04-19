using Brevo.Umbraco.Extensions;
using Brevo.Umbraco.Workflows;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Brevo.Umbraco.Composers;

public class BrevoComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var apiKey = builder.Config["Brevo:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Brevo:ApiKey is not configured. Add it to appsettings.json: " +
                "{ \"Brevo\": { \"ApiKey\": \"xkeysib-...\" } }");

        builder.AddBrevo();

        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<SendToBrevoWorkflow>();
    }
}
