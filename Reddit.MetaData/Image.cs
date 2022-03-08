namespace Reddit.MetaData
{
    public class Image
    {
        public Source Source { get; set; }
        public Resolution[] Resolutions { get; set; }
        public Variants Variants { get; set; }
        public string Id { get; set; }
    }
}
