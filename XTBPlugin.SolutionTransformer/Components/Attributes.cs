using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Attributes : ComponentBase
    {
        public EntityMetadata[] EntityMetadata;

        public Attributes(EntityMetadata[] entityMetadata) : base()
        {
            EntityMetadata = entityMetadata;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase entities)
        {
            foreach (EntityMetadata entity in EntityMetadata)
            {
                if (entity.IsCustomizable.Value && entity.IsIntersect == false)
                {
                    IEnumerable<AttributeMetadata> attributes = null;

                    if (entity.IsCustomEntity.Value)
                    {
                        if (entities.ComponentDescriptions.Any(e => e.Name.Equals(entity.LogicalName)))
                        {
                            attributes = entity.Attributes;
                        }
                    }
                    else if (entity.IsCustomizable.Value)
                    {
                        attributes = entity.Attributes.Where(a => a.IsCustomAttribute.Value && publishers.Any(p => a.SchemaName.StartsWith(p)));
                    }

                    if (attributes != null && attributes.Any())
                    {
                        if (!entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity.MetadataId.Value)))
                        {
                            entities.ComponentDescriptions.Add(new MetadataDescription(entity));
                        }

                        foreach (AttributeMetadata attribute in attributes)
                        {
                            ComponentDescriptions.Add(new MetadataDescription(attribute));
                        }
                    }
                }
            }
        }
    }
}
