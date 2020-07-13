using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

using System.Collections.Generic;

using XTBPlugin.SolutionTransformer.Components;

namespace XTBPlugin.SolutionTransformer
{
    public class SolutionBuilder
    {
        private readonly IOrganizationService service;

        public Dictionary<ComponentType, IComponentBase> ComponentDictionary = new Dictionary<ComponentType, IComponentBase>();

        public SolutionBuilder(IOrganizationService orgService)
        {
            service = orgService;
        }

        public bool CollectComponents(Settings settings, List<string> publisher)
        {
            Entities entities = new Entities();
            if (settings.IncludeEntites)
            {
                entities.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.Entity, entities);
            }

            if (settings.IncludeAttributes)
            {
                Attributes attributes = new Attributes(entities);
                attributes.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.Attributes, attributes);
            }

            if (settings.IncludeWebResource)
            {
                WebResources webResources = new WebResources();
                webResources.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.WebResources, webResources);
            }

            return true;
        }

        public bool AddComponentsToSolution(string targetSolution)
        {
            List<AddSolutionComponentRequest> fullRequests = new List<AddSolutionComponentRequest>();

            foreach (KeyValuePair<ComponentType, IComponentBase> componentTypes in ComponentDictionary)
            {
                fullRequests.AddRange(componentTypes.Value.GetRequestList(targetSolution));
            }

            foreach (var request in fullRequests)
            {
                service.Execute(request);
            }

            return true;
        }
    }
}
