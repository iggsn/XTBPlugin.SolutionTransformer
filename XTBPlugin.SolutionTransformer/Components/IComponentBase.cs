using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

using System.Collections.Generic;

namespace XTBPlugin.SolutionTransformer.Components
{
    public interface IComponentBase
    {
        List<AddSolutionComponentRequest> GetRequestList(string solutionUniqueName);

        void FetchComponents(IOrganizationService service, List<string> publishers);
    }
}
