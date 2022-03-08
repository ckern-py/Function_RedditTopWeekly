namespace Reddit.MetaData
{
    public class Data
    {
        public object Modhash { get; set; }
        public int Dist { get; set; }
        public Child[] Children { get; set; }
        public string After { get; set; }
        public object Before { get; set; }
    }
}
