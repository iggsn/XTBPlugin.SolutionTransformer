using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

using XrmToolBox.Extensibility;

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
            if (settings.IncludeEntites || settings.IncludeAttributes || settings.IncludeRelationships || settings.IncludeSystemforms)
            {
                reportProgress(0, "Reading Entity Metadata");
                EntityMetadata[] entityMetadata = GetEntityMetadata(settings);
                if (!entityMetadata.Any())
                {
                    reportProgress(0, "Failed reading EntityMetadata!");
                    return false;
                }

                Entities entities = new Entities(entityMetadata);
                Attributes attributes = new Attributes(entityMetadata);
                if (settings.IncludeEntites)
                {
                    reportProgress(0, "Collecting Entities...");
                    entities.FetchComponents(service, publisher);
                }
                ComponentDictionary.Add(ComponentType.Entity, entities);


                if (settings.IncludeAttributes)
                {
                    reportProgress(0, "Collecting Attributes...");

                    attributes.FetchComponents(service, publisher, entities);
                }
                ComponentDictionary.Add(ComponentType.Attributes, attributes);

                if (settings.IncludeRelationships)
                {
                    reportProgress(0, "Collecting Relationships...");
                    Relationships relationships = new Relationships(entityMetadata);
                    relationships.FetchComponents(service, publisher, entities, attributes);
                    ComponentDictionary.Add(ComponentType.Relationships, relationships);
                }

                if (settings.IncludeSystemforms)
                {
                    reportProgress(0, "Collecting SystemForms...");
                    SystemForms systemforms = new SystemForms(entityMetadata);
                    systemforms.FetchComponents(service, publisher);
                    ComponentDictionary.Add(ComponentType.SystemForms, systemforms);
                }

            }

            if (settings.IncludeOptionsets)
            {
                reportProgress(0, "Collecting OptionSets...");
                OptionSets optionsets = new OptionSets();
                optionsets.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.OptionSets, optionsets);
            }

            if (settings.IncludeWebResource)
            {
                reportProgress(0, "Collecting WebResources...");
                WebResources webResources = new WebResources();
                webResources.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.WebResources, webResources);
            }


            if (settings.IncludePluginAssembly)
            {
                PluginAssemblies pluginAssemblies = new PluginAssemblies();
                reportProgress(0, "Collecting PluginAssemblies...");
                pluginAssemblies.FetchComponents(service, publisher);
                ComponentDictionary.Add(ComponentType.PluginAssemblies, pluginAssemblies);
            }

            return true;
        }

        public EntityMetadata[] GetEntityMetadata(Settings settings)
        {
            EntityMetadata[] result = null;

            MetadataCache metadataCache;

            if (!SettingsManager.Instance.TryLoad(GetType(), out metadataCache, $"Metadata-{settings.LastUsedOrganizationName}"))
            {
                metadataCache = new MetadataCache();
                metadataCache.OrganizationName = settings.LastUsedOrganizationName;
            }

            MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("IsManaged", MetadataConditionOperator.Equals, false));
            entityFilter.Conditions.Add(new MetadataConditionExpression("IsCustomizable", MetadataConditionOperator.Equals, true));
            entityFilter.Conditions.Add(new MetadataConditionExpression("IsIntersect", MetadataConditionOperator.Equals, false));
            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter
            };
            RetrieveMetadataChangesRequest retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression,
                ClientVersionStamp = metadataCache.IsCached && !string.IsNullOrEmpty(metadataCache.TimeStamp) ? metadataCache.TimeStamp : null
            };
            RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

            if (metadataCache.IsCached)
            {
                if (response.ServerVersionStamp != metadataCache.TimeStamp)
                {
                    throw new Exception("Please handle Metadata Changes");
                }
                else
                {
                    result = JsonConvert.DeserializeObject<EntityMetadataCollection>(metadataCache.Metadata).ToArray();
                }
            }
            else
            {
                result = response.EntityMetadata.ToArray();
                metadataCache.TimeStamp = response.ServerVersionStamp;
                metadataCache.IsCached = true;
                metadataCache.Metadata = JsonConvert.SerializeObject(response.EntityMetadata, response.EntityMetadata.GetType(), new JsonSerializerSettings() { });
            }

            SettingsManager.Instance.Save(GetType(), metadataCache, $"Metadata-{settings.LastUsedOrganizationName}");

            return result;
        }

        public bool AddComponentsToSolution(string targetSolution, Settings mySettings, Action<int, string> reportProgress)
        {
            reportProgress(0, "Prepare AddSolutionComponentRequests...");

            List<Tuple<AddSolutionComponentRequest, MetadataDescription>> fullRequests = new List<Tuple<AddSolutionComponentRequest, MetadataDescription>>();

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

                    requestWithResults.Requests.AddRange(fullRequests.Skip((pageNumber - 1) * mySettings.ExecuteMultipleBatchSize).Take(mySettings.ExecuteMultipleBatchSize).Select(i => i.Item1));

                    ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)service.Execute(requestWithResults);

                    if (responseWithResults.IsFaulted)
                    {
                        var errors = responseWithResults.Responses.Where(r => r.Fault != null);
                        throw new Exception(errors.First().Fault.Message);
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
                    service.Execute(request.Item1);
                }
            }

            return true;
        }
    }
}
