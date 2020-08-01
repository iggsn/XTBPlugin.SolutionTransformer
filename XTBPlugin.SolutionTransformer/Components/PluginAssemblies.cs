using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class PluginAssemblies : ComponentBase
    {
        public PluginAssemblies() : base()
        {
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            QueryExpression queryPluginAssemblies = new QueryExpression("pluginassembly")
            {
                ColumnSet = new ColumnSet("pluginassemblyid", "name", "version"),
                Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("name", ConditionOperator.DoesNotEndWith, "none_profiled"),
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

            EntityCollection pluginAssemblies = service.RetrieveMultiple(queryPluginAssemblies);

            if (pluginAssemblies.Entities.Any())
            {
                ComponentDescriptions.AddRange(from Entity entity in pluginAssemblies.Entities
                                               select new MetadataDescription(entity, ComponentType.PluginAssemblies));
            }
        }
    }
}
