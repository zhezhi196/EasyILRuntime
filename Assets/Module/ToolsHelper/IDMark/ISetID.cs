namespace Module
{
    public interface ISetID<T, K>
    {
        T ID { get; set; }
        K SetID(T ID);
    }
}