using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string LastUsedOrganizationWebappUrl { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("EntityTypes")]
        [DisplayName("WebResources (61)")]
        [Description("Will add WebResources to the Target Solution.")]
        public bool IncludeWebResource { get; set; }
    }
}