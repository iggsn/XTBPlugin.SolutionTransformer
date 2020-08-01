using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class SystemForms : ComponentBase
    {
        public SystemForms() : base()
        {
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, EntityMetadata[] entityMetadata)
        {
            QueryExpression querySystemForms = new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("systemformid", "solutionid", "publishedon", "iscustomizable"),
                Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("objecttypecode", ConditionOperator.NotEqual, "none"),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("iscustomizable", ConditionOperator.Equal, true),
                                new ConditionExpression("componentstate", ConditionOperator.Equal, 0),
                                new ConditionExpression("objecttypecode", ConditionOperator.Equal, "account")
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

            EntityCollection systemForms = service.RetrieveMultiple(querySystemForms);

            if (systemForms.Entities.Any())
            {
                ComponentDescriptions.AddRange(from Entity entity in systemForms.Entities
                                               select new MetadataDescription(entity, ComponentType.SystemForms));
            }
        }
    }
}
