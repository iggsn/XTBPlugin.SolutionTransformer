using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Relationships : ComponentBase
    {
        public Entities Entities;
        public Attributes Attributes;

        public Relationships(Entities entities, Attributes attributes) : base()
        {
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

        private void HandleOneToManyRelationships(List<string> publishers, EntityMetadata[] entityMetadata, EntityMetadata entity)
        {
            IEnumerable<OneToManyRelationshipMetadata> relationships = entity.OneToManyRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

            if (relationships.Any())
            {
                foreach (OneToManyRelationshipMetadata relationship in relationships)
                {
                    EntityMetadata entitySearch = entityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity);
                    if (!Entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entitySearch.MetadataId.Value)))
                    {
                        Entities.ComponentDescriptions.Add(new MetadataDescription(entitySearch));
                    }

                    AttributeMetadata attributeSearch = entityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity).Attributes.First(a => a.LogicalName == relationship.ReferencingAttribute);
                    if (!Attributes.ComponentDescriptions.Any(e => e.ComponentId.Equals(attributeSearch.MetadataId.Value)))
                    {
                        Attributes.ComponentDescriptions.Add(new MetadataDescription(attributeSearch));
                    }

                    ComponentDescriptions.Add(new MetadataDescription(relationship));
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
                    EntityMetadata entity1Search = entityMetadata.First(e => e.LogicalName == relationship.Entity1LogicalName);
                    if (!Entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity1Search.MetadataId.Value)))
                    {
                        Entities.ComponentDescriptions.Add(new MetadataDescription(entity1Search));
                    }

                    EntityMetadata entity2Search = entityMetadata.First(e => e.LogicalName == relationship.Entity1LogicalName);
                    if (!Entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity2Search.MetadataId.Value)))
                    {
                        Entities.ComponentDescriptions.Add(new MetadataDescription(entity2Search));
                    }

                    if (!ComponentDescriptions.Any(r => r.ComponentId.Equals(relationship.MetadataId.Value)))
                    {
                        ComponentDescriptions.Add(new MetadataDescription(relationship));
                    }
                }
            }
        }
    }
}
