using Module;
using Sirenix.OdinInspector;
using UnityEngine;

public enum Hurtmaterial
{
    None,
    [LabelText("金属")]
    Mental,
    [LabelText("木头")]
    Wood,
    [LabelText("石头")]
    Stone,
    [LabelText("肉")]
    Meat,
    [LabelText("布料")]
    Close,
    [LabelText("纸")]
    Paper,
    [LabelText("玻璃")]
    Glass,
    [LabelText("塑料")]
    Plastic,
}
public interface IHurtObject
{
    Transform transform { get; }
    Hurtmaterial hurtMaterial { get; }
    Damage OnHurt(ITarget target, Damage damage);
}