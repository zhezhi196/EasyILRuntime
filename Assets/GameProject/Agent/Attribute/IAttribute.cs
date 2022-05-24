public interface IAttribute<T>
{
    T hp { get; set; }
    T att { get; set; }
    T moveSpeed { get; set; }
    T anger { get; set; }
    T critical { get; set; }
    T criticalDamage { get; set; }
    T rotateSpeed { get; set; }
    T angerRadius { get; set; }
    T angerUpSpeed { get; set; }
    T angerDownSpeed { get; set; }
    T angryTime { get; set; }
}