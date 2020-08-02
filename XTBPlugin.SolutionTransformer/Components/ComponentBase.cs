using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

using System;
using System.Collections.Generic;

namespace XTBPlugin.SolutionTransformer.Components
{
    public abstract class ComponentBase : IComponentBase
    {
        public List<MetadataDescription> ComponentDescriptions;

        public ComponentBase()
        {
            ComponentDescriptions = new List<MetadataDescription>();
        }

        public virtual void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            throw new NotImplementedException();
        }

        public virtual void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase components1)
        {
            throw new NotImplementedException();
        }

        public virtual void FetchComponents(IOrganizationService service, List<string> publishers, ComponentBase components1, ComponentBase components2)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<AddSolutionComponentRequest, MetadataDescription>> GetRequestList(string solutionUniqueName)
        {
            List<Tuple<AddSolutionComponentRequest, MetadataDescription>> list = new List<Tuple<AddSolutionComponentRequest, MetadataDescription>>();
            foreach (var component in ComponentDescriptions)
            {
                list.Add(new Tuple<AddSolutionComponentRequest, MetadataDescription>(new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = component.ComponentId,
                    ComponentType = (int)component.Type,
                    SolutionUniqueName = solutionUniqueName,
                    DoNotIncludeSubcomponents = true
                },
                component));
            }

            return list;
        }
    }
}
