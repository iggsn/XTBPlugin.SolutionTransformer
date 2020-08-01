using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class WebResources : ComponentBase
    {
        public WebResources() : base()
        {
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            QueryExpression queryWebResources = new QueryExpression("webresource")
            {
                ColumnSet = new ColumnSet("componentstate", "ishidden", "iscustomizable", "ismanaged", "webresourcetype", "name"),
                Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("name", ConditionOperator.DoesNotBeginWith,  "cc_shared/")
                            },
                            Filters =
                                {
                                     new FilterExpression
                                     {
                                         FilterOperator =LogicalOperator.Or,
                                         Conditions = {}
                                     }
                                }
                        }
            };

            foreach (string publisher in publishers)
            {
                queryWebResources.Criteria.Filters[0].AddCondition("name", ConditionOperator.BeginsWith, publisher);
            }

            EntityCollection webResources = service.RetrieveMultiple(queryWebResources);

            if (webResources.Entities.Any())
            {
                ComponentDescriptions.AddRange(from Entity entity in webResources.Entities
                                               select new MetadataDescription(entity, ComponentType.WebResources));
            }
        }
    }
}
