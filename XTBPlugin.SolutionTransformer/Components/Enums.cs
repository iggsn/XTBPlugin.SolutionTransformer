namespace XTBPlugin.SolutionTransformer.Components
{
    /// <summary>
    /// Reflects the Component Types
    /// https://bettercrm.blog/2017/04/26/solution-component-types-in-dynamics-365/
    /// </summary>
    public enum ComponentType
    {
        Entity = 1,
        Attributes = 2,
        OptionSets = 9,
        Relationships = 10,
        WebResources = 61,
        PluginTypes = 90,
        PluginAssemblies = 91
    }
}
