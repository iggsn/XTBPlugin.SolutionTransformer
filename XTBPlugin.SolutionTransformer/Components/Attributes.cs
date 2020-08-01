using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Attributes : ComponentBase
    {
        public Entities Entities;

        public Attributes(Entities entities) : base()
        {
            Entities = entities;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, EntityMetadata[] entityMetadata)
        {
            foreach (EntityMetadata entity in entityMetadata)
            {
                if (entity.IsCustomizable.Value && entity.IsIntersect == false)
                {
                    IEnumerable<AttributeMetadata> attributes = entity.Attributes.Where(a => a.IsCustomAttribute.Value && publishers.Any(p => a.SchemaName.StartsWith(p)));

                    if (attributes.Any())
                    {
                        if (!Entities.ComponentDescriptions.Any(e => e.ComponentId.Equals(entity.MetadataId.Value)))
                        {
                            Entities.ComponentDescriptions.Add(new MetadataDescription(entity));
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
