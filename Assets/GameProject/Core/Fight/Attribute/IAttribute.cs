public interface IAttribute<T> 
{
    T hp { get; set; }
    T gunAtt { get; set; }
    T gunHeadAttK { get; set; }
    T meleeAtt { get; set; }
    T paralysis { get; set; }
    T moveSpeed { get; set; }
    T strength { get; set; }
    T energy { get; set; }
    T violentAttP { get; set; }
    T violentAttK { get; set; }
    T crouchMovek { get; set; }
    T aimMoveK { get; set; }
    T runMoveK { get; set; }
    T hardAttP { get; set; }
    T hardAttK { get; set; }
    T shotInterval { get; set; }
    T bulletCount { get; set; }
    T bulletBag { get; set; }
    T accurate { get; set; }
    T recoilForce { get; set; }
    T attRange { get; set; }
    T assDamage { get; set; }
}