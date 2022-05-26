using Module;
using UnityEngine;

public class WeaponSight : MonoBehaviour
{
    public bool isShow = false;
    protected Weapon weapon;
    public void SetWeapon(Weapon w)
    {
        weapon = w;
    }

    public virtual void ShowSight()
    {
        isShow = true;
        gameObject.OnActive(true);
    }

    public virtual void HideSight()
    {
        isShow = false;
        gameObject.OnActive(false);
    }

    public virtual void Fire()
    {

    }
}

