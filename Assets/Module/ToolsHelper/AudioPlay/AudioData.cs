namespace Module
{
    public class AudioData: ISqlData
    {
        public string name { get; set; }
        public string path { get; set; }
        public int importWeight { get; set; }
        public int ID { get; set; }
    }
}