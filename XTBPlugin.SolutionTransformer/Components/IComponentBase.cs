using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

using System;
using System.Collections.Generic;

namespace XTBPlugin.SolutionTransformer.Components
{
    public interface IComponentBase
    {
        List<Tuple<AddSolutionComponentRequest, MetadataDescription>> GetRequestList(string solutionUniqueName);

        void FetchComponents(IOrganizationService service, List<string> publishers);

        void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase components1);

        void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase components1, ComponentBase components2);
    }
}
