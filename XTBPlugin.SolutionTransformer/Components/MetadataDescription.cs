using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class MetadataDescription
    {
        public Guid ComponentId;
        public ComponentType Type;
        public string Name;

        public MetadataDescription(EntityMetadata entityMetadata)
        {
            Type = ComponentType.Entity;
            ComponentId = entityMetadata.MetadataId.Value;
            Name = entityMetadata.LogicalName;
        }

        public MetadataDescription(AttributeMetadata attributeMetadata)
        {
            Type = ComponentType.Attributes;
            ComponentId = attributeMetadata.MetadataId.Value;
            Name = attributeMetadata.LogicalName;
        }

        public MetadataDescription(OptionSetMetadataBase optionset)
        {
            Type = ComponentType.OptionSets;
            ComponentId = optionset.MetadataId.Value;
            Name = optionset.Name;
        }

        public MetadataDescription(Entity entity, ComponentType componentType)
        {
            Type = componentType;
            ComponentId = entity.Id;
            Name = entity.LogicalName;
        }

        public MetadataDescription(RelationshipMetadataBase relationship)
        {
            Type = ComponentType.Relationships;
            ComponentId = relationship.MetadataId.Value;
            Name = relationship.SchemaName;
        }
    }
}
