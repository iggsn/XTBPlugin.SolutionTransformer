namespace XTBPlugin.SolutionTransformer
{
    public class MetadataCache
    {
        public string TimeStamp { get; set; }

        public string OrganizationName { get; set; }

        public bool IsCached { get; set; }

        public string Metadata { get; set; }

        public MetadataCache()
        {
            TimeStamp = null;
            IsCached = false;
        }
    }
}
