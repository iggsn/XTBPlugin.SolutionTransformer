using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool CollectComponents(Settings settings, List<string> publisher, Action<int, string> reportProgress)
        {
            Entities entities = new Entities();
            if (settings.IncludeEntites)
            {
                reportProgress(0, "Collecting Entities...");
                entities.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.Entity, entities);
            }

            if (settings.IncludeAttributes)
            {
                reportProgress(0, "Collecting Attributes...");
                Attributes attributes = new Attributes(entities);
                attributes.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.Attributes, attributes);
            }

            if (settings.IncludeWebResource)
            {
                reportProgress(0, "Collecting WebResources...");
                WebResources webResources = new WebResources();
                webResources.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.WebResources, webResources);
            }

            return true;
        }

        public bool AddComponentsToSolution(string targetSolution, Settings mySettings)
        {
            List<AddSolutionComponentRequest> fullRequests = new List<AddSolutionComponentRequest>();

            foreach (KeyValuePair<ComponentType, IComponentBase> componentTypes in ComponentDictionary)
            {
                fullRequests.AddRange(componentTypes.Value.GetRequestList(targetSolution));
            }

            if (mySettings.UseExecuteMultiple)
            {
                int pageNumber = 0;

                do
                {
                    pageNumber++;

                    var requestWithResults = new ExecuteMultipleRequest()
                    {
                        // Assign settings that define execution behavior: continue on error, return responses.
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = false,
                            ReturnResponses = true
                        },
                        // Create an empty organization request collection.
                        Requests = new OrganizationRequestCollection()
                    };

                    requestWithResults.Requests.AddRange(fullRequests.Skip(pageNumber * mySettings.ExecuteMultipleBatchSize).Take(mySettings.ExecuteMultipleBatchSize));

                    ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)service.Execute(requestWithResults);

                    if (responseWithResults.Responses.Any(r => r.Fault != null))
                    {
                        return false;
                    }

                    //// Display the results returned in the responses.
                    //foreach (var responseItem in responseWithResults.Responses)
                    //{
                    //    // A valid response.
                    //    if (responseItem.Response != null)
                    //        DisplayResponse(requestWithResults.Requests[responseItem.RequestIndex], responseItem.Response);


                    //    // An error has occurred.
                    //    else if (responseItem.Fault != null)
                    //        DisplayFault(requestWithResults.Requests[responseItem.RequestIndex],
                    //        responseItem.RequestIndex, responseItem.Fault);
                    //}

                } while (fullRequests.Count() > pageNumber * mySettings.ExecuteMultipleBatchSize);
            }
            else
            {
                foreach (var request in fullRequests)
                {
                    service.Execute(request);
                }
            }

            return true;
        }
    }
}
