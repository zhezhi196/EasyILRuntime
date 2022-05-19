namespace Module
{
    public interface IAntiCrackingField<T>
    {
        T value { get; }
        FieldAesType aesType { get; }
    }
}