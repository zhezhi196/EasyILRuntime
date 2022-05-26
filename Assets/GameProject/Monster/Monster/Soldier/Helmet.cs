using Module;
using UnityEngine;

public class Helmet : MonoBehaviour, IHurtObject
{
    public GameObject helmet;
    public Hurtmaterial hurtMaterial
    {
        get
        {
            return Hurtmaterial.Mental;
        }
    }

    public Damage OnHurt(ITarget target, Damage damage)
    {
        if (damage.damage > 0)
        {
            gameObject.OnActive(false);
            helmet.OnActive(false);
        }

        return damage;
    }

    public void ResetToBorn()
    {
        gameObject.OnActive(true);
        helmet.OnActive(true);
    }
}