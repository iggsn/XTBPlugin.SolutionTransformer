using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Entities : ComponentBase
    {
        public EntityMetadata[] EntityMetadata;

        public Entities(EntityMetadata[] entityMetadata) : base()
        {
            EntityMetadata = entityMetadata;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            foreach (EntityMetadata entity in EntityMetadata)
            {
                if (entity.IsManaged == false && publishers.Any(p => entity.SchemaName.StartsWith(p)) && entity.IsIntersect == false && entity.IsCustomEntity == true && entity.IsBPFEntity == false)
                {
                    ComponentDescriptions.Add(new MetadataDescription(entity));
                }
            }
        }
    }
}
