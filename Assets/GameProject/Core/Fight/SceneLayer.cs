using Module;
using UnityEngine;

public class SceneLayer: MonoBehaviour,IHurtObject
{
    [SerializeField]
    private Hurtmaterial _hurtMaterial;

    public Hurtmaterial hurtMaterial
    {
        get { return _hurtMaterial; }
        set { _hurtMaterial = value; }
    }

    public Damage OnHurt(ITarget target, Damage damage)
    {
        return damage;
    }
}