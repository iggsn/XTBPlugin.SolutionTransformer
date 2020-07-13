﻿using System.ComponentModel;

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
        [Category("Settings")]
        [DisplayName("Use ExecuteMultiple")]
        [Description("Will use an Execute Multiple Request. Be carefull. Only 2 can be happening in parallel per Organization if not specified different.")]
        public bool UseExecuteMultiple { get; set; }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Settings")]
        [DisplayName("Execute Batch Size")]
        [Description("Size of the batches, when using Execute Multiple. Normal max is 1.000.")]
        public int ExecuteMultipleBatchSize { get; set; }

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