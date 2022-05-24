using System;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int maxAttackCount;
    public int currAttack;
    public Damage[] damage;
    public Monster monster;

    private void OnTriggerEnter(Collider other)
    {
        if (currAttack < maxAttackCount)
        {
            currAttack++;
            IHurtObject hurtObject = other.GetComponent<IHurtObject>();
            if (hurtObject is MonsterPart part)
            {
                
            }
            for (int i = 0; i < damage.Length; i++)
            {
                var dam = hurtObject.OnHurt(damage[i]);
            }


        }
    }

    public void PreAttack(int attack, params Damage[] damages)
    {
        currAttack = 0;
        maxAttackCount = attack;
        this.damage = damages;
    }
}