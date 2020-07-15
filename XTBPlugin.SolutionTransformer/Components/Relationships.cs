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
        public Dictionary<Guid, RelationshipMetadataBase> Components;
        public Entities Entities;
        public Attributes Attributes;

        public Relationships(Entities entities, Attributes attributes) : base(ComponentType.Relationships)
        {
            Components = new Dictionary<Guid, RelationshipMetadataBase>();
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

            foreach (EntityMetadata entity in entityMetadata)
            {
                if (entity.IsCustomizable.Value && entity.IsIntersect == false)
                {
                    HandleOneToManyRelationships(publishers, entityMetadata, entity);
                    HandleManyToManyRelationships(publishers, entityMetadata, entity);
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

        private void HandleOneToManyRelationships(List<string> publishers, EntityMetadata[] entityMetadata, EntityMetadata entity)
        {
            IEnumerable<OneToManyRelationshipMetadata> relationships = entity.OneToManyRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

            if (relationships.Any())
            {
                foreach (OneToManyRelationshipMetadata relationship in relationships)
                {
                    EntityMetadata entitySearch = entityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity);
                    if (!Entities.Components.ContainsKey(entitySearch.MetadataId.Value))
                    {
                        Entities.Components.Add(entitySearch.MetadataId.Value, entitySearch);
                    }

                    AttributeMetadata attributeSearch = entityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity).Attributes.First(a => a.LogicalName == relationship.ReferencingAttribute);
                    if (!Attributes.Components.ContainsKey(attributeSearch.MetadataId.Value))
                    {
                        Attributes.Components.Add(attributeSearch.MetadataId.Value, attributeSearch);
                    }

                    Components.Add(relationship.MetadataId.Value, relationship);
                }
            }
        }

        private void HandleManyToManyRelationships(List<string> publishers, EntityMetadata[] entityMetadata, EntityMetadata entity)
        {
            IEnumerable<ManyToManyRelationshipMetadata> relationships = entity.ManyToManyRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

            if (relationships.Any())
            {
                foreach (ManyToManyRelationshipMetadata relationship in relationships)
                {
                    if (!Entities.Components.ContainsKey(entity.MetadataId.Value))
                    {
                        Entities.Components.Add(entity.MetadataId.Value, entity);
                    }

                    if (!Components.ContainsKey(relationship.MetadataId.Value))
                    {
                        Components.Add(relationship.MetadataId.Value, relationship);
                    }
                }
            }
        }
    }
}
