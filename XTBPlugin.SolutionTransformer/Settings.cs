using System.ComponentModel;

namespace XTBPlugin.SolutionTransformer
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        [Browsable(false)]
        public string LastUsedOrganizationWebappUrl { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("EntityTypes")]
        [DisplayName("Entity (1)")]
        [Description("Will add Entities to the Target Solution.")]
        public bool IncludeEntites { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("EntityTypes")]
        [DisplayName("Attributes (2)")]
        [Description("Will add Attributes to the Target Solution.")]
        public bool IncludeAttributes { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("EntityTypes")]
        [DisplayName("WebResources (61)")]
        [Description("Will add WebResources to the Target Solution.")]
        public bool IncludeWebResource { get; set; }
    }
}