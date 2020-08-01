using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Entities : ComponentBase
    {
        public Entities() : base() {  }

        public override void FetchComponents(IOrganizationService service, List<string> publishers, EntityMetadata[] entityMetadata)
        {
            foreach (EntityMetadata entity in entityMetadata)
            {
                if (entity.IsManaged == false && publishers.Any(p => entity.SchemaName.StartsWith(p)) && entity.IsIntersect == false)
                {
                    ComponentDescriptions.Add(new MetadataDescription(entity));
                }
            }
        }
    }
}
