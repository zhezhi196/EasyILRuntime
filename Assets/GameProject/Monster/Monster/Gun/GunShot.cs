using Module;
using Module.SkillAction;
using Project.Data;
using UnityEngine;

[SkillDescript("机枪/射击")] 
public class GunShot : MonsterSkill
{
    public GunMonster gunMonster
    {
        get { return (GunMonster) (owner); }
    }

    public override AttackMonster monster
    {
        get
        {
            GameDebug.LogError("请使用gunMonster");
            return null;
        }
    }

    public Vector3 rayOri
    {
        get
        {
            return gunMonster.line.transform.parent.position;
        }
    }
    public LayerMask layer;

    public override void OnInit(ISkillObject owner)
    {
        if (dbData == null) dbData = DataMgr.Instance.GetSqlService<MonsterSkillData>().WhereID(dbId);
        PushAction(new TimeAction(dbData.cd, owner));
        PushAction(new EmptyAction());
    }

    public override bool isReadyRelease
    {
        get
        {
            bool b1 = gunMonster.target != null && isActive && station == SkillStation.Ready && gunMonster.isAim;
            return b1 && Physics.Raycast(rayOri, gunMonster.target.targetPoint - rayOri, 1000);
        }
    }

    protected override void OnActionUpdate(ISkillAction arg1, float percent)
    {
        if (arg1 is TimeAction && gunMonster.target != null && gunMonster.isAim)
        {
            gunMonster.line.gameObject.OnActive(true);
            Vector3 target = Vector3.zero;
            if (gunMonster.target is Player)
            {
                target = 2 * gunMonster.target.targetPoint - rayOri;
            }
            else
            {
                target = gunMonster.target.targetPoint;
            }

            gunMonster.line.SetPositions(new[] {rayOri, target});
        }
        else
        {
            gunMonster.line.gameObject.OnActive(false);
        }        
    }


    protected override void OnActionEnter(ISkillAction skillAction)
    {
        base.OnActionEnter(skillAction);
        if (skillAction is EmptyAction)
        {
            RaycastHit hit;
            Vector3 dir = gunMonster.target.targetPoint - rayOri;
            if (Physics.Raycast(new Ray(rayOri, dir), out hit, 1000, layer))
            {
                Debug.DrawRay(rayOri, dir, Color.red, 1);
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
    }

    protected override void OnReleaseStart()
    {
    }

    protected override void OnReleaseEnd(bool complete)
    {
        gunMonster.line.gameObject.OnActive(false);
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

    public override void UpdateTry()
    {
    }
}