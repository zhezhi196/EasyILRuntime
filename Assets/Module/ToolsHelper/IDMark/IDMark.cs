namespace Module
{
    public interface IDMark<T, K>
    {
        T ID { get; set; }
        K SetID(T ID, string tag = null);
    }
}