namespace Module
{
    public interface Identify
    {
        object ID { get; }
    }
    public interface Identify<T>
    {
        T ID { get; }
    }
}