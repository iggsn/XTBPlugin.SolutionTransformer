using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class WebResources : ComponentBase
    {
        public Dictionary<Guid, Entity> Components;

        public WebResources() : base(ComponentType.WebResources)
        {
            Components = new Dictionary<Guid, Entity>();
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            QueryExpression queryWebResources = new QueryExpression("webresource")
            {
                ColumnSet = new ColumnSet("componentstate", "ishidden", "iscustomizable", "ismanaged", "webresourcetype", "name"),
                Criteria =
                        {
                            Conditions = {
                                new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("name", ConditionOperator.DoesNotBeginWith,  "cc_shared/")
                            }
                        }
            };

            foreach (string publisher in publishers)
            {
                queryWebResources.Criteria.AddCondition("name", ConditionOperator.BeginsWith, publisher);
            }

            EntityCollection webResources = service.RetrieveMultiple(queryWebResources);

            if (webResources.Entities.Any())
            {
                Components = webResources.Entities.ToList().ToDictionary(x => x.Id);
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
