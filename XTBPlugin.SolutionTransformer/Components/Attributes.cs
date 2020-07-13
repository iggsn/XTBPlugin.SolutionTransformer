using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;

namespace XTBPlugin.SolutionTransformer.Components
{
    public class Attributes : ComponentBase
    {
        public Dictionary<Guid, AttributeMetadata> Components;
        public Entities Entities;

        public Attributes(Entities entities) : base(ComponentType.Attributes)
        {
            Components = new Dictionary<Guid, AttributeMetadata>();
            Entities = entities;
        }

        public override void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Attributes
            };

            RetrieveAllEntitiesResponse entities = (RetrieveAllEntitiesResponse)service.Execute(request);

            if (entities.EntityMetadata.Any())
            {
                foreach (EntityMetadata entity in entities.EntityMetadata)
                {
                    if (entity.IsCustomizable.Value && entity.IsIntersect == false)
                    {
                        IEnumerable<AttributeMetadata> attributes = entity.Attributes.Where(a => a.IsCustomAttribute.Value && publishers.Any(p => a.SchemaName.StartsWith(p)));

                        if (attributes.Any())
                        {
                            if (!Entities.Components.ContainsKey(entity.MetadataId.Value))
                            {
                                Entities.Components.Add(entity.MetadataId.Value, entity);
                            }

                            foreach (AttributeMetadata attribute in attributes)
                            {
                                //if (attribute.IsCustomAttribute.Value && publishers.Any(p => attribute.SchemaName.StartsWith(p)))
                                //{
                                Components.Add(attribute.MetadataId.Value, attribute);
                                //}
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
