using Module;
using UnityEngine;

public enum Hurtmaterial
{
    None,
    Mental,
    Wood,
    Stone,
    Meat,
    Close,
    Paper,
    Glass,
    Plastic,
    Eggs,
    TeaPot
}
public interface IHurtObject
{
    Transform transform { get; }
    Hurtmaterial hurtMaterial { get; }
    Damage OnHurt(ITarget target, Damage damage);
}