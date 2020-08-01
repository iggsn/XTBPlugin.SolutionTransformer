using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Relationships : ComponentBase
    {
        public EntityMetadata[] EntityMetadata;

        public Relationships(EntityMetadata[] entityMetadata) : base()
        {
            EntityMetadata = entityMetadata;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase entities, ComponentBase attributes)
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

            foreach (EntityMetadata entity in EntityMetadata)
            {
                if (entity.IsCustomizable.Value && !entity.IsManaged.Value && entity.IsIntersect == false && publishers.Any(p => entity.SchemaName.StartsWith(p)))
                {
                    HandleOneToManyRelationships(publishers, entity, (Entities)entities, (Attributes)attributes);
                    HandleManyToManyRelationships(publishers, entity, (Entities)entities);
                }
            }
        }

        private void HandleOneToManyRelationships(List<string> publishers, EntityMetadata entity, Entities entities, Attributes attributes)
        {
            IEnumerable<OneToManyRelationshipMetadata> relationships = entity.IsCustomEntity.Value
                ? entity.OneToManyRelationships
                : entity.OneToManyRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

            if (relationships.Any())
            {
                foreach (OneToManyRelationshipMetadata relationship in relationships)
                {
                    EntityMetadata entitySearch = EntityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity);
                    if (!entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entitySearch.MetadataId.Value)))
                    {
                        entities.ComponentDescriptions.Add(new MetadataDescription(entitySearch));
                    }

                    AttributeMetadata attributeSearch = EntityMetadata.First(e => e.LogicalName == relationship.ReferencingEntity).Attributes.First(a => a.LogicalName == relationship.ReferencingAttribute);
                    if (!attributes.ComponentDescriptions.Any(e => e.ComponentId.Equals(attributeSearch.MetadataId.Value)))
                    {
                        attributes.ComponentDescriptions.Add(new MetadataDescription(attributeSearch));
                    }

                    ComponentDescriptions.Add(new MetadataDescription(relationship));
                }
            }
        }

        private void HandleManyToManyRelationships(List<string> publishers, EntityMetadata entity, Entities entities)
        {
            IEnumerable<ManyToManyRelationshipMetadata> relationships = entity.IsCustomEntity.Value
                ? entity.ManyToManyRelationships
                : entity.ManyToManyRelationships.Where(r => r.IsCustomRelationship.Value && publishers.Any(p => r.SchemaName.StartsWith(p)));

            if (relationships.Any())
            {
                foreach (ManyToManyRelationshipMetadata relationship in relationships)
                {
                    EntityMetadata entity1Search = EntityMetadata.First(e => e.LogicalName == relationship.Entity1LogicalName);
                    if (entity1Search.IsCustomizable.Value && !entity1Search.IsManaged.Value && !entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity1Search.MetadataId.Value)))
                    {
                        entities.ComponentDescriptions.Add(new MetadataDescription(entity1Search));
                    }

                    EntityMetadata entity2Search = EntityMetadata.First(e => e.LogicalName == relationship.Entity1LogicalName);
                    if (entity2Search.IsCustomizable.Value && !entity2Search.IsManaged.Value && !entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity2Search.MetadataId.Value)))
                    {
                        entities.ComponentDescriptions.Add(new MetadataDescription(entity2Search));
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
