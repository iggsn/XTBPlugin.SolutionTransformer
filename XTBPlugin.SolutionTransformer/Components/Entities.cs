using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Entities : ComponentBase
    {
        public Dictionary<Guid, EntityMetadata> Components;

        public Entities() : base(ComponentType.Entity)
        {
            Components = new Dictionary<Guid, EntityMetadata>();
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };

            RetrieveAllEntitiesResponse entities = (RetrieveAllEntitiesResponse)service.Execute(request);

            if (entities.EntityMetadata.Any())
            {
                foreach (EntityMetadata entity in entities.EntityMetadata)
                {
                    if (entity.IsManaged == false && publishers.Any(p => entity.SchemaName.StartsWith(p)) && entity.IsIntersect == false)
                    {
                        Components.Add(entity.MetadataId.Value, entity);
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
