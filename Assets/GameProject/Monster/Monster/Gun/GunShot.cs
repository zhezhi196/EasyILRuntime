using Module;
using UnityEngine;

[SkillDescript("机枪/射击")] 
public class GunShot : MonsterSkill
{
    public GunMonster gunMonster
    {
        get { return (GunMonster) (owner); }
    }

    public LayerMask layer;


    public override bool isReadyRelease
    {
        get { return gunMonster.target != null && isActive && station == SkillStation.Ready; }
    }

    public override void OnInit(ISkillObject owner)
    {
        base.OnInit(owner);
        PushAction(new ShotAction());
    }

    protected override void OnReleaseStart()
    {
        RaycastHit hit;
        Vector3 dir = gunMonster.target.targetPoint - gunMonster.eye.transform.position;
        if (Physics.Raycast(new Ray(gunMonster.eye.transform.position, dir), out hit,1000,layer))
        {
            Debug.DrawRay(gunMonster.eye.transform.position, dir, Color.red, 1);
            var damageValue = GetDamage();
            if (hit.collider.gameObject.layer == MaskLayer.monster)
            {
                AttackMonster attackMonster = hit.collider.transform.GetComponentInParent<AttackMonster>();
                Damage Monstdamage = new Damage() {damage = damageValue.damage};
                attackMonster.OnHurt(Monstdamage);
            }
            else if (hit.collider.gameObject.layer == MaskLayer.Playerlayer)
            {
                Player.player.OnHurt(damageValue);
            }
            else
            {
                GameDebug.LogFormat("机枪打到{0}", hit.collider.gameObject.name);
            }
        }
    }

    protected override void OnReleaseEnd(bool complete)
    {
    }
    protected override void OnDispose()
    {
    }
    private PlayerDamage[] damages;

    public override PlayerDamage GetDamage(int index = 0)
    {
        if (damages == null)
        {
            string[] dam = dbData.damage.Split(ConstKey.Spite0);
            damages = new PlayerDamage[dam.Length];
            if (owner is IMonster mm)
            {
                for (int i = 0; i < damages.Length; i++)
                {
                    damages[i] = new PlayerDamage(mm);
                    damages[i].damage = mm.currentLevel.dbData.att * dam[i].ToFloat();
                }
            }
        }
        return damages[index];        
    }

    public override float stopMoveDistance
    {
        get
        {
            return float.MaxValue;
        }
    }
}