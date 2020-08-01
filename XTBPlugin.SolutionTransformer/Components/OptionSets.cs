using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class OptionSets : ComponentBase
    {
        //public Dictionary<Guid, OptionSetMetadataBase> Components;

        public OptionSets() : base()
        {
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            RetrieveAllOptionSetsRequest req = new RetrieveAllOptionSetsRequest();

            RetrieveAllOptionSetsResponse res = (RetrieveAllOptionSetsResponse)service.Execute(req);

            foreach (OptionSetMetadataBase optionset in res.OptionSetMetadata)
            {
                if (optionset.IsCustomizable.Value && publishers.Any(p => optionset.Name.StartsWith(p)))
                {
                    ComponentDescriptions.Add(new MetadataDescription(optionset));
                }
            }
        }
    }
}
