using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

using System;
using System.Collections.Generic;

namespace XTBPlugin.SolutionTransformer.Components
{
    public abstract class ComponentBase : IComponentBase
    {
        internal readonly ComponentType SubType;

        public ComponentBase() { }

        protected ComponentBase(ComponentType subType)
        {
            SubType = subType;
        }

        public virtual void FetchComponents(IOrganizationService service, List<string> publishers)
        {
            throw new NotImplementedException();
        }

        public virtual void FetchComponents(IOrganizationService service, List<string> publishers, EntityMetadata[] entityMetadata)
        {
            throw new NotImplementedException();
        }

        public virtual List<AddSolutionComponentRequest> GetRequestList(string solutionUniqueName)
        {
            throw new NotImplementedException();
        }
    }
}
