using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class OptionSets : ComponentBase
    {
        public Dictionary<Guid, OptionSetMetadataBase> Components;

        public OptionSets() : base(ComponentType.OptionSets)
        {
            Components = new Dictionary<Guid, OptionSetMetadataBase>();
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            RetrieveAllOptionSetsRequest req = new RetrieveAllOptionSetsRequest();

            RetrieveAllOptionSetsResponse res = (RetrieveAllOptionSetsResponse)service.Execute(req);

            foreach (OptionSetMetadataBase optionset in res.OptionSetMetadata)
            {
                if (optionset.IsCustomizable.Value && publishers.Any(p => optionset.Name.StartsWith(p)))
                {
                    Components.Add(optionset.MetadataId.Value, optionset);
                }
            }
        }

        public override List<AddSolutionComponentRequest> GetRequestList(string solutionUniqueName)
        {
            List<AddSolutionComponentRequest> list = new List<AddSolutionComponentRequest>();
            foreach (var component in Components)
            {
                list.Add(new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = component.Key,
                    ComponentType = (int)SubType,
                    SolutionUniqueName = solutionUniqueName
                });
            }

            return list;
        }
    }
}
