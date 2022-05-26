public interface IMonsterAttribute<T>
{
    T hp { get; set; }
    T att { get; set; }
    
    T moveSpeed { get; set; }
    T rotateSpeed { get; set; }
}