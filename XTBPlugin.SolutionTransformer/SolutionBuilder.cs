using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

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

        public bool CollectComponents(Settings settings, List<string> publisher, Action<int, string> reportProgress, Action<string, object[]>logInfo)
        {
            if (settings.IncludeEntites || settings.IncludeAttributes || settings.IncludeRelationships || settings.IncludeSystemforms)
            {
                reportProgress(0, "Reading Entity Metadata");
                EntityMetadata[] entityMetadata = GetEntityMetadata();
                if (!entityMetadata.Any())
                {
                    reportProgress(0, "Failed reading EntityMetadata!");
                    return false;
                }

                Entities entities = new Entities();
                if (settings.IncludeEntites)
                {
                    logInfo("Collecting Entities", null);
                    reportProgress(0, "Collecting Entities...");
                    entities.FetchComponents(service, publisher, entityMetadata);
                    ComponentDictionary.Add(entities.SubType, entities);
                    logInfo($"Collected {entities.Components.Count} entities", null);
                }
                else
                {
                    ComponentDictionary.Add(entities.SubType, entities);
                }

                Attributes attributes = new Attributes(entities);
                if (settings.IncludeAttributes)
                {
                    reportProgress(0, "Collecting Attributes...");

                    attributes.FetchComponents(service, publisher, entityMetadata);
                    ComponentDictionary.Add(attributes.SubType, attributes);
                }
                else
                {
                    ComponentDictionary.Add(attributes.SubType, attributes);
                }

                if (settings.IncludeRelationships)
                {
                    reportProgress(0, "Collecting Relationships...");
                    Relationships relationships = new Relationships(entities, attributes);
                    relationships.FetchComponents(service, publisher, entityMetadata);
                    ComponentDictionary.Add(relationships.SubType, relationships);
                }

                if (settings.IncludeSystemforms)
                {
                    reportProgress(0, "Collecting SystemForms...");
                    SystemForms systemforms = new SystemForms();
                    systemforms.FetchComponents(service, publisher, entityMetadata);
                    ComponentDictionary.Add(systemforms.SubType, systemforms);
                }

            }

            if (settings.IncludeOptionsets)
            {
                reportProgress(0, "Collecting OptionSets...");
                OptionSets optionsets = new OptionSets();
                optionsets.FetchComponents(service, publisher);
                ComponentDictionary.Add(optionsets.SubType, optionsets);
            }

            if (settings.IncludeWebResource)
            {
                reportProgress(0, "Collecting WebResources...");
                WebResources webResources = new WebResources();
                webResources.FetchComponents(service, publisher);
                ComponentDictionary.Add(webResources.SubType, webResources);
            }

            
            if (settings.IncludePluginAssembly)
            {
                PluginAssemblies pluginAssemblies = new PluginAssemblies();
                reportProgress(0, "Collecting PluginAssemblies...");
                pluginAssemblies.FetchComponents(service, publisher);
                ComponentDictionary.Add(pluginAssemblies.SubType, pluginAssemblies);
            }

            return true;
        }

        public EntityMetadata[] GetEntityMetadata()
        {
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.All
            };

            RetrieveAllEntitiesResponse entities = (RetrieveAllEntitiesResponse)service.Execute(request);

            if (entities.EntityMetadata.Any())
            {
                return entities.EntityMetadata;
            }
            else
            {
                return new EntityMetadata[0];
            }
        }

        public bool AddComponentsToSolution(string targetSolution, Settings mySettings, Action<int, string> reportProgress)
        {
            reportProgress(0, "Prepare AddSolutionComponentRequests...");

            List<AddSolutionComponentRequest> fullRequests = new List<AddSolutionComponentRequest>();

            foreach (KeyValuePair<ComponentType, IComponentBase> componentTypes in ComponentDictionary)
            {
                fullRequests.AddRange(componentTypes.Value.GetRequestList(targetSolution));
            }

            if (mySettings.UseExecuteMultiple)
            {
                int pageNumber = 0;
                int maxPages = fullRequests.Count() / mySettings.ExecuteMultipleBatchSize + 1;

                do
                {
                    pageNumber++;

                    reportProgress(maxPages / 100 * pageNumber, $"Adding page {pageNumber} of {maxPages} with {mySettings.ExecuteMultipleBatchSize} records...");

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

                    requestWithResults.Requests.AddRange(fullRequests.Skip((pageNumber - 1) * mySettings.ExecuteMultipleBatchSize).Take(mySettings.ExecuteMultipleBatchSize));

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
