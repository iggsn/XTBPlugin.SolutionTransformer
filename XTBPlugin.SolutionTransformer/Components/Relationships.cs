using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Relationships : ComponentBase
    {
        public Dictionary<Guid, OneToManyRelationshipMetadata> Components;
        public Entities Entities;
        public Attributes Attributes;

        public Relationships(Entities entities, Attributes attributes) : base(ComponentType.Relationships)
        {
            Components = new Dictionary<Guid, OneToManyRelationshipMetadata>();
            Entities = entities;
            Attributes = attributes;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, EntityMetadata[] entityMetadata)
        {
            QueryExpression queryRelationships = new QueryExpression("relationship")
            {
                ColumnSet = new ColumnSet("name", "relationshipid"),
                Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("componentstate", ConditionOperator.Equal, 0),
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
                queryRelationships.Criteria.Filters[0].AddCondition("name", ConditionOperator.BeginsWith, publisher);
            }

            EntityCollection result = service.RetrieveMultiple(queryRelationships);
            Dictionary<string, Entity> relationshipEntityList = result.Entities.ToList().ToDictionary(x => x.GetAttributeValue<string>("name"));

            foreach (EntityMetadata entity in entityMetadata)
            {
                if (entity.IsCustomizable.Value && entity.IsIntersect == false)
                {
                    IEnumerable<OneToManyRelationshipMetadata> relationships = entity.ManyToOneRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

                    if (relationships.Any())
                    {
                        if (!Entities.Components.ContainsKey(entity.MetadataId.Value))
                        {
                            Entities.Components.Add(entity.MetadataId.Value, entity);
                        }

                        foreach (OneToManyRelationshipMetadata relationship in relationships)
                        {
                            if (relationshipEntityList.ContainsKey(relationship.SchemaName))
                            {
                                Guid relationshipId = relationshipEntityList[relationship.SchemaName].Id;

                                AttributeMetadata attribute = entityMetadata.First(e => e.LogicalName == relationship.ReferencedEntity).Attributes.First(a => a.LogicalName == relationship.ReferencedAttribute);
                                if (!Attributes.Components.ContainsKey(attribute.MetadataId.Value))
                                {
                                    Attributes.Components.Add(attribute.MetadataId.Value, attribute);
                                }

                                Components.Add(relationshipId, relationship);
                            }
                        }
                    }


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
                    SolutionUniqueName = solutionUniqueName,
                    DoNotIncludeSubcomponents = true
                });
            }

            return list;
        }
    }
}
