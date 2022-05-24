using Module;
using UnityEngine;

public interface IHurtObject
{
    Damage OnHurt(Damage damage);
}