using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class PluginTypes : ComponentBase
    {
        public Dictionary<Guid, Entity> Components;

        public PluginTypes() : base(ComponentType.PluginTypes)
        {
            Components = new Dictionary<Guid, Entity>();
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            QueryExpression queryPluginTypes = new QueryExpression("plugintype")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                //new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("componentstate", ConditionOperator.Equal, 0)
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

            /*foreach (string publisher in publishers)
            {
                queryWebResources.Criteria.Filters[0].AddCondition("name", ConditionOperator.BeginsWith, publisher);
            }*/

            EntityCollection pluginTypes = service.RetrieveMultiple(queryPluginTypes);

            if (pluginTypes.Entities.Any())
            {
                Components = pluginTypes.Entities.ToList().ToDictionary(x => x.Id);
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
