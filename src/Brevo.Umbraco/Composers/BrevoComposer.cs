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
        builder.AddBrevo();

        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<SendToBrevoWorkflow>();
    }
}
